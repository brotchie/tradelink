using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TradeLink.API;
using TradeLink.AppKit;
using TradeLink.Common;
using System.Xml.Serialization;
using Microsoft.VisualBasic;
using System.Reflection;

namespace ASP
{
    public partial class ASP : Form
    {
        public const string PROGRAM = "ASP";
        const string SKINEXT = ".skn";
        public string SKINPATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+"\\";

        // working variables
        Dictionary<string, SecurityImpl> _seclist = new Dictionary<string, SecurityImpl>();
        Dictionary<string, int[]> _symidx = new Dictionary<string, int[]>();
        List<Response> _reslist = new List<Response>();
        TickArchiver _ta = new TickArchiver();
        BasketImpl _mb = new BasketImpl();
        ASPOptions _ao = new ASPOptions();
        TLTracker _tlt;
        MessageTracker _mtexec;
        MessageTracker _mtquote;

        Dictionary<int, string> _resskinidx = new Dictionary<int, string>();
        Dictionary<string, string> _class2dll = new Dictionary<string, string>();
        PositionTracker _pt = new PositionTracker();
        string[] _acct = new string[0];
        AsyncResponse _ar = new AsyncResponse();
        Log _log = new Log(PROGRAM);
        DebugWindow _dw = new DebugWindow();
        const int REMOVERES = 0;
        const int ENABLED = 1;
        const int SAVESKIN = 2;
        const int EDITSYM = 3;
        const int MAXRESPONSEPERASP = 100;
        const int MAXASPINSTANCE = 4;
        int _ASPINSTANCE = 0;
        int _INITIALRESPONSEID = 0;
        int _NEXTRESPONSEID = 0;
        BackgroundWorker bw = new BackgroundWorker();

        public ASP()
        {
            // read designer options for gui
            InitializeComponent();
            // show status
            status(Util.TLSIdentity());
            debug(Util.TLSIdentity());
            // count instances of program
            _ASPINSTANCE = getprocesscount(PROGRAM)-1;
            // ensure have not exceeded maximum
            if ((_ASPINSTANCE + 1) > MAXASPINSTANCE)
            {
                MessageBox.Show("You have exceeded maximum # of running ASPs (" + MAXASPINSTANCE + ")." + Environment.NewLine + "Please close some.", "too many ASPs");
                status("Too many ASPs.  Disabled.");
                debug("Too many ASPs.  Disabled.");
                return;
            }
            else
            {
                status("ASP " + (_ASPINSTANCE+1) + "/" + MAXASPINSTANCE);
                debug("ASP " + (_ASPINSTANCE+1) + "/" + MAXASPINSTANCE);
            }
            // set next response id
            _NEXTRESPONSEID = _ASPINSTANCE * MAXRESPONSEPERASP;
            _INITIALRESPONSEID = _NEXTRESPONSEID;
            _remskin.Click+=new EventHandler(_remskin_Click);
            _saveskins.Click+=new EventHandler(_saveskins_Click);
            _skins.SelectedIndexChanged+=new EventHandler(_skins_SelectedIndexChanged);
            _ar.GotTick += new TickDelegate(tl_gotTick);
            _bf = new BrokerFeed(Properties.Settings.Default.prefquote, Properties.Settings.Default.prefexecute,_ao._providerfallback.Checked,false,PROGRAM);
            _bf.SendDebug+=new DebugDelegate(debug);
            // get providers
            initfeeds();
            // get asp option events
            _ao.TimeoutChanged += new Int32Delegate(_ao_TimeoutChanged);
            _ao._datasel.SelectedIndexChanged += new EventHandler(_prefquot_SelectedIndexChanged);
            _ao._execsel.SelectedIndexChanged += new EventHandler(_prefexec_SelectedIndexChanged);
            // setup right click menu
            _resnames.ContextMenu= new ContextMenu();
            _resnames.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            _resnames.ContextMenu.MenuItems.Add("remove response", new EventHandler(remresp));
            _resnames.ContextMenu.MenuItems.Add("enabled", new EventHandler(toggleresponse));
            _resnames.ContextMenu.MenuItems.Add("save to skin", new EventHandler(add2skin));
            _resnames.ContextMenu.MenuItems.Add("edit symbols", new EventHandler(editsyms));
            // make sure we exit properly
            this.FormClosing += new FormClosingEventHandler(ASP_FormClosing);
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();
            // get last loaded response library
            LoadResponseDLL(Properties.Settings.Default.boxdll);
            // load any skins we can find
            findskins();
            // process command line
            processcommands();

        }

        void processcommands()
        {
            string [] args = Environment.GetCommandLineArgs();
            if (args.Length <2) return;
            loadskin(args[1]);
        }

        void initfeeds()
        {

            _bf.Reset();
            _ao._execsel.DataSource = _bf.ProvidersAvailable;
            _ao._datasel.DataSource = _bf.ProvidersAvailable;
            // if we have quotes
            // don't save ticks from replay since they're already saved
            _ao.archivetickbox.Checked = ( _bf.ProvidersAvailable.Length>0) && !_bf.FeedClient.RequestFeatureList.Contains(MessageTypes.HISTORICALDATA);
            // monitor quote feed
            if (_bf.isFeedConnected)
            {
                int poll = (int)((double)Properties.Settings.Default.brokertimeoutsec * 1000 / 2);
                _tlt = new TLTracker(poll, (int)Properties.Settings.Default.brokertimeoutsec, _bf.FeedClient, Providers.Unknown, true);
                _tlt.GotConnectFail += new VoidDelegate(_tlt_GotConnectFail);
                _tlt.GotConnect += new VoidDelegate(_tlt_GotConnect);
                _tlt.GotDebug += new DebugDelegate(_tlt_GotDebug);
                _ao._datasel.SelectedIndex = _bf.FeedClient.ProviderSelected;
                _ao._datasel.Text = _bf.FeedClient.BrokerName.ToString();
                status("Connected: " + _bf.Feed);
            }

            if (_bf.isBrokerConnected)
            {
                // if we have execs
                _ao._execsel.SelectedIndex = _bf.BrokerClient.ProviderSelected;
                _ao._execsel.Text = _bf.BrokerName.ToString();

            }
            // hook up events
            _bf.gotFill += new FillDelegate(tl_gotFill);
            _bf.gotOrder += new OrderDelegate(tl_gotOrder);
            _bf.gotOrderCancel += new UIntDelegate(tl_gotOrderCancel);
            _bf.gotPosition += new PositionDelegate(tl_gotPosition);
            _bf.gotTick += new TickDelegate(quote_gotTick);
            _bf.gotUnknownMessage += new MessageDelegate(tl_gotUnknownMessage);


            // pass messages through
            _mtquote = new MessageTracker(_bf.FeedClient);
            _mtquote.SendMessageResponse += new MessageDelegate(tl_gotUnknownMessage);
            _mtquote.SendDebug += new DebugDelegate(_mt_SendDebug);
            _mtexec = new MessageTracker(_bf.BrokerClient);
            _mtexec.SendMessageResponse += new MessageDelegate(tl_gotUnknownMessage);
            _mtexec.SendDebug += new DebugDelegate(_mt_SendDebug);

            // startup
            _tlt_GotConnect();



        }



        void quote_gotTick(Tick t)
        {
            _ar.newTick(t);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            TLClient tl = new TLClient_WM(false);
            // check for new versions
            Versions.UpgradeAlert(tl,true);
            tl.Disconnect();
        }


        void _mt_SendDebug(string msg)
        {
            debug(msg);
        }

        void _ao_TimeoutChanged(int val)
        {
            _tlt.AlertThreshold = val;
        }

        void _tlt_GotDebug(string msg)
        {

        }

        void _tlt_GotConnect()
        {
            try
            {
                if (_tlt.tw.RecentTime != 0)
                {
                    debug(_bf.FeedClient.BrokerName + " " + _bf.FeedClient.ServerVersion + " connected.");
                    status(_bf.FeedClient.BrokerName + " connected.");
                }
                // if we have a quote provider
                if (_bf.FeedClient!= null)
                {
                    // disable timeouts on tradelink provider
                    _ao._brokertimeout.Enabled = _bf.FeedClient.BrokerName != Providers.TradeLink;
                    // don't track tradelink
                    if (_bf.FeedClient.BrokerName == Providers.TradeLink)
                    {
                        _tlt.Stop();
                    }

                    // if we have a quote provid
                    if (_mb.Count > 0)
                        _bf.Subscribe(_mb);
                }
            }
            catch { }
        }

        void _tlt_GotConnectFail()
        {
            if (_tlt == null) return;
            if (_tlt.tw.RecentTime != 0)
            {
                status("Quotes disconnected.");
                debug("Quotes disconnected");
            }
        }

        void tl_gotUnknownMessage(MessageTypes type, uint source, uint dest, uint id, string request, ref string response)
        {
            // send unknown messages to valid responses
            for (int idx= 0; idx<_reslist.Count; idx++)
                if (!isBadResponse(idx))
                    _reslist[idx].GotMessage(type, source, dest, id, request, ref response);
        }


        Dictionary<int, int> _rid2local = new Dictionary<int, int>();
        /// <summary>
        /// gets local storage location from response id
        /// </summary>
        /// <param name="responseid"></param>
        /// <returns></returns>
        int r2r(uint responseid)
        {
            int idx = -1;
            if (_rid2local.TryGetValue((int)responseid, out idx))
                return idx;
            return -1;
        }
        int r2r(int responseid) { return r2r((uint)responseid); }

        void tl_gotTickasync(Tick t)
        {
            // on multi-core machines, this will be invoked to write ticks
            // to a cache where they will be processed by a seperate thread
            // asynchronously
            _ar.newTick(t);
        }

        void tl_gotAccounts(string msg)
        {
            // save accounts found connected
            _acct = msg.Split(',');
        }



        void ContextMenu_Popup(object sender, EventArgs e)
        {
            // make sure a single response is selected
            if (_resnames.SelectedIndices.Count != 1)
            {
                // disable stuff that only makes sense in context of one symbols
                _resnames.ContextMenu.MenuItems[ENABLED].Visible = false;
                _resnames.ContextMenu.MenuItems[EDITSYM].Visible = false;
                _resnames.ContextMenu.MenuItems[SAVESKIN].Visible = false;
                _resnames.ContextMenu.MenuItems[REMOVERES].Visible = false;
                return;
            }
            // enable stuff that makes sense in context of one symbol
            _resnames.ContextMenu.MenuItems[ENABLED].Visible = true;
            _resnames.ContextMenu.MenuItems[EDITSYM].Visible = true;
            _resnames.ContextMenu.MenuItems[SAVESKIN].Visible = true;
            _resnames.ContextMenu.MenuItems[REMOVERES].Visible = true;
            // update check to reflect validity of response
            foreach (int dindex in _resnames.SelectedIndices)
            {
                if ((dindex < 0) || (dindex > _resnames.Items.Count)) continue;
                int index = getrindx(dindex);
                _resnames.ContextMenu.MenuItems[ENABLED].Checked = !isBadResponse(index);
            }

        }

        void add2skin(object sender, EventArgs e)
        {
            // make sure something is selected
            if (_resnames.SelectedIndices.Count == 0) return;
            // get name
            string name = Interaction.InputBox("What is the skin name for these responses?", "Skin name", "Skin" + DateTime.Now.Ticks.ToString(), 0, 0);
            // get next available index for this name
            int startidx = nextskinidx(SKINPATH,name);
            // count successes
            int succount = 0;
            // go through all selected responses
            foreach (int didx in _resnames.SelectedIndices)
            {
                //get response index from display index
                int idx = getrindx(didx);
                // save them as skin
                bool worked = SkinImpl.SkinFile(_reslist[idx], _reslist[idx].FullName, _class2dll[_reslist[idx].FullName], SKINPATH + name + "." + startidx.ToString() + SKINEXT, new DebugDelegate(debug));
                // notify errors
                if (!worked)
                    debug("skin failed on: " + _reslist[idx].FullName + " " + _reslist[idx].ID);
                // add index as part of skin
                string sn = string.Empty;
                if (_resskinidx.TryGetValue(idx, out sn))
                    if (sn != name)
                        _resskinidx[idx] = sn + " " + name;
                    else ;
                else
                    _resskinidx.Add(idx, name);
                // increment next filename index
                startidx++;
                // count as good
                succount++;
            }
            status("added " + succount + " responses to " + name);
            // find any new names
            findskins();
        }



        void findskins()
        {
            // clear existing skins
            _skins.Items.Clear();

            // go through every skin file
            foreach (string fn in skinfiles(SKINPATH))
            {
                // get skin name
                string sk = skinfromfile(fn);
                // if we don't have it, display as an option
                if (!_skins.Items.Contains(sk))
                    _skins.Items.Add(sk);
            }
            // refresh screen
            _skins.Invalidate(true);
        }

        string[] skinfiles(string path)
        {
            List<string> files = new List<string>();
            // get info for this directory
            DirectoryInfo di = new DirectoryInfo(path);
            // find all skins in this directory
            FileInfo[] skins = di.GetFiles("*" + SKINEXT);
            // build list of their names
            foreach (FileInfo skin in skins)
                files.Add(skin.Name);
            // return results
            return files.ToArray();
        }

        int nextskinidx(string path, string skinname)
        {
            // no matching skins
            int count = 0;
            // get all skins 
            string[] files = skinfiles(path);
            // go through and find only skins with matching name
            foreach (string fn in files)
                if (skinfromfile(fn) == skinname)
                    count++; // count matches
            // return total matches
            return count;
        }

        string skinfromfile(string filename)
        {
            string name = Path.GetFileNameWithoutExtension(filename);
            string[] r = name.Split('.');
            return r[0];
        }




        private void _skins_SelectedIndexChanged(object sender, EventArgs e)
        {
            // user has selected a new skin

            // get the name
            string skin = _skins.SelectedItem.ToString();
            //confirm loading
            if (MessageBox.Show("Load skin " + skin + "?", "confirm skin load", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            // try to set it up for trading
            if (tradeskins(skin))
            {
                // update screen
                _resnames.Invalidate(true);
                // notify user
                status("loaded skin: " + skin);
            }
            _ao.Hide();
        }

        bool tradeskins(string name)
        {
            // get skin files available
            string[] files = skinfiles(SKINPATH);
            // set status variable
            bool worked = true;
            try
            {
                // loop through every file
                foreach (string fn in files)
                {
                    // if it's the skin we want to trade
                    if (skinfromfile(fn) == name)
                    {
                        worked &= loadskin(SKINPATH+fn);
                    }
                }
                return worked;
            }
            catch (Exception) { }
            return false;
        }

        bool loadskin(string skinfullpath)
        {
            // get it along with it's persisted settings
            Response r = (Response)SkinImpl.DeskinFile(skinfullpath,new DebugDelegate(debug));
            // add it
            int id = addresponse(r);
            // check if it was added
            bool added = id != -1;
            // mark it as loaded
            if (added)
                _resskinidx.Add(id, skinfromfile(skinfullpath));
            if (added)
                debug("Loaded skin: " + skinfullpath);
            else
                debug("unable to load skin: " + skinfullpath);
            return added;
        }

        int addresponse(Response r)
        {
            int id = _NEXTRESPONSEID;
            try
            {
                // set the id
                r.ID = id;
                // ensure it has a full name
                if (r.FullName == string.Empty)
                    r.FullName = r.GetType().ToString();
                // get local response index
                int idx = _reslist.Count;
                // bind events
                bindresponseevents(r);
                // show it to user
                _resnames.Items.Add(r.FullName);
                // add it to trade list
                lock (_reslist)
                {
                    _reslist.Add(r);
                }
                // save id to local relationship
                _rid2local.Add(r.ID, idx);
                // setup place for it's symbols
                _rsym.Add(id, string.Empty);
                // map name to response
                _disp2real.Add(idx);
                // save id
                _NEXTRESPONSEID++;
                // reset response
                _reslist[idx].Reset();
                // send it current positions
                foreach (Position p in _pt)
                    _reslist[idx].GotPosition(p);
                // update everything
                IndexBaskets();
                // show we added response
                status(r.FullName + getsyms(idx));
            }
            catch (Exception ex)
            {
                return -1;
            }
            return id;
        }

        private void Boxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // make sure something is selected
            if (_availresponses.SelectedIndex == -1) return;
            // make sure we haven't maxed our responses
            if (_NEXTRESPONSEID - _INITIALRESPONSEID >= MAXRESPONSEPERASP)
            {
                status("Exceeded maximum responses: " + MAXRESPONSEPERASP);
                debug("Exceeded maximum responses: " + MAXRESPONSEPERASP);
                return;
            }
            // get selected response
            string resname = (string)_availresponses.SelectedItem;
            // load it into working response
            Response tmp = new InvalidResponse();
            try
            {
                tmp = ResponseLoader.FromDLL(resname, Properties.Settings.Default.boxdll);
            }
            catch (Exception ex)
            {
                // log it
                bigexceptiondump(ex);
                // unselect response
                _availresponses.SelectedIndex = -1;
                return;
            }
            // add it
            int idx = addresponse(tmp);
            // make sure it worked
            if (idx==-1)
            {
                return;
            }
            // save the dll that contains the class for use with skins
            string dll = string.Empty;
            // if we don't have this class, add it
            if (!_class2dll.TryGetValue(resname, out dll))
                _class2dll.Add(resname, Properties.Settings.Default.boxdll);
            else // otherwise replace current dll as providing this class
                _class2dll[resname] = Properties.Settings.Default.boxdll;
            // unselect response
            _availresponses.SelectedIndex = -1;

        }

        void bigexceptiondump(Exception ex)
        {
            status("response failed.  see messages.");
            debug("exception: " + ex.Message);
            debug("stack: " + ex.StackTrace);
            if (ex.InnerException != null)
            {
                debug("inner: " + ex.InnerException.Message);
                debug("inner stack: " + ex.InnerException.StackTrace);
            }
        }

        void IndexBaskets()
        {
            // purpose of this function :  update symidx from _rsym 
            // _rsym roughly equivalent to TLServer::stocks
            // create new index
            Dictionary<string, List<int>> newidx = new Dictionary<string, List<int>>(_mb.Count);
            foreach (int r in _rsym.Keys)
            {
                
                // get all syms response is subscribed to
                string [] syms = _rsym[r].Split(',');
                // go through every sym
                foreach (string sym in syms)
                {
                    // keep track of clients for this symbol
                    List<int> responseclients;
                    // make sure we have symbol
                    if (!newidx.TryGetValue(sym, out responseclients))
                    {
                        responseclients = new List<int>(20);
                        newidx.Add(sym, responseclients);
                    }
                    // make sure we have this response subscribing to this symbol
                    if (!responseclients.Contains(r))
                        responseclients.Add(r);
                    // save it back
                    newidx[sym] = responseclients;
                }
            }
            // save it
            _symidx.Clear();
            foreach (string sym in newidx.Keys)
                _symidx.Add(sym, newidx[sym].ToArray());

            // update _mb from contents of symidx
            updateMB();
            // update screen to match symidx
            updateScreenResponses();
        }

        void updateScreenResponses()
        {
            // go through every displayed response
            for (int disp = 0; disp<_disp2real.Count; disp++)
            {
                // make sure display index good
                if ((disp < 0) || (disp >= _disp2real.Count)) continue;
                // get real index
                int real = _disp2real[disp];
                // if real index is bad
                if ((real < 0) || (real >= _reslist.Count))
                {
                    continue;
                }
                // update the displayed name and symlist
                _resnames.Items[disp] = getrstat(real);
            }
            // refresh screen
            _resnames.Invalidate(true);
        }

        void updateMB()
        {
            List<string> syms = new List<string>(_symidx.Count);
            foreach (string sym in _symidx.Keys)
                syms.Add(sym);
            string old = _mb.ToString();
            _mb = new BasketImpl(syms.ToArray());
            bool subscribe = old != _mb.ToString();

            if (!subscribe) return;
            if (_bf.FeedClient == null) return;

            try
            {
                // resubscribe
                _bf.Subscribe(_mb);
            }
            catch (TLServerNotFound)
            {
                debug("symbol subscribe failed as no TL server was found.");
            }
        }

        void remresp(object sender, EventArgs e)
        {
            // mark UI entry for removal
            List<int> remdidx = new List<int>();
            // process each selected response
            foreach (int dispidx in _resnames.SelectedIndices)
            {
                // make sure we're still trading it
                if ((dispidx<0) || (dispidx>_disp2real.Count))
                    continue;
                // get actual index
                int selbox = getrindx(dispidx);
                // get name
                string name = _reslist[selbox].FullName;
                // remove id to local association
                int responseID = _reslist[selbox].ID;
                _rid2local.Remove(responseID);
                // remove the response
                _reslist[selbox] = new InvalidResponse();
                // clear it's symbols
                _rsym[selbox] = string.Empty;
                // mark it's UI element for removal
                remdidx.Add(dispidx);
                // notify user
                debug("removed #"+dispidx+": "+name+" id:"+responseID);
            }
            // remove response from screen
            for (int i = remdidx.Count -1; i>=0; i--)
            {
                // remove it
                _resnames.Items.RemoveAt(remdidx[i]);
                // remove name map
                _disp2real.RemoveAt(remdidx[i]);
            }

            // update everything
            IndexBaskets();
        }

        int contains(string sym, int response)
        {
            int [] v;
            if (!_symidx.TryGetValue(sym, out v))
                return -1;
            for (int i = 0; i<v.Length; i++)
                if (v[i] == response) return i;
            return -1;

        }

        void toggleresponse(object sender, EventArgs e)
        {
            // process each selected response
            foreach (int dbox in _resnames.SelectedIndices)
            {
                // make sure it's valid
                if ((dbox < 0) || (dbox > _resnames.Items.Count)) continue;

                // get selected box
                int selbox = getrindx(dbox);
                // ensure index is good
                if ((selbox < 0) || (selbox > _reslist.Count)) continue;
                // invert current response's validity
                bool valid = !_reslist[selbox].isValid;
                // save it back
                _reslist[selbox].isValid = valid;
                // notify
                debug(_reslist[selbox].FullName + " " + (valid ? "set valid." : "set invalid."));
            }
            // update display
            _resnames.Invalidate(true);
        }

        void tl_gotOrderCancel(uint number)
        {
            // see if we need to remap
            if (_ao._virtids.Checked)
            {
                uint master = number;
                number = aspid2responseid(number);
                // if we don't have one, assign one
                if (number == 0)
                {
                    // assign
                    number = _masteridt.AssignId;
                    // map
                    _r2a.Add(number, master);
                    // other way
                    _a2r.Add(master, number);
                }
            }
            // send order cancel notification to every valid box
            for (int idx = 0; idx<_reslist.Count; idx++)
                if (!isBadResponse(idx))
                    _reslist[idx].GotOrderCancel(number);
        }

        void tl_gotOrder(Order o)
        {
            // see if we need to remap order ids
            if (_ao._virtids.Checked && (o.id!=0))
            {
                // see if we already have a map
                uint rorderid = aspid2responseid(o.id);
                // if we don't create one
                if (rorderid == 0)
                {
                    // get an id
                    rorderid = _masteridt.AssignId;
                    // save it
                    _r2a.Add(rorderid, o.id);
                    // save other way
                    _a2r.Add(o.id, rorderid);

                }
                // remap it
                o.id = rorderid;
            }
            // send order notification to any valid responses
            for (int i = 0; i<_reslist.Count; i++)
                if (!isBadResponse(i))
                    _reslist[i].GotOrder(o);
        }

        void ASP_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_resskinidx.Count>0) 
                _saveskins_Click(null, null);
            // save ASP properties
            Properties.Settings.Default.Save();

            Stop();
            
        }

        public void Stop()
        {
            try
            {
                // stop gui-safe broker-feed operations
                _bf.Stop();
                // stop watching ticks
                _tlt.Stop();
                // stop tick thread
                _ar.Stop();
                // stop archiving ticks
                _ta.Stop();
                // stop logging
                _log.Stop();

            }
            catch { }
        }

        void skinexit()
        {

        }




       
        void tl_gotTick(Tick t)
        {
            // see if we are tracking this symbol
            int[] idxs = new int[0];
            if (!_symidx.TryGetValue(t.symbol, out idxs) )
                return;

            // see if we should save this tick
            if (_ao.archivetickbox.Checked)
                _ta.newTick(t);

            // send tick to any valid requesting responses
            foreach (int idx in idxs)
                if (_reslist[idx].isValid) 
                    _reslist[idx].GotTick(t);
            // watch for timeout
            _tlt.newTick(t);
        }



        void tl_gotFill(Trade t)
        {
            // keep track of position
            _pt.Adjust(t);
            // see if we're using virtual ids
            if (_ao._virtids.Checked && (t.id != 0))
            {
                // get the map
                uint rorderid = aspid2responseid(t.id);
                // if we don't have a map, create one
                if (rorderid == 0)
                {
                    // get id
                    rorderid = _masteridt.AssignId;
                    // map it
                    _r2a.Add(rorderid, t.id);
                    // map other way
                    _a2r.Add(t.id, rorderid);
                }
                // apply map
                t.id = rorderid;
            }
            // send trade notification to any valid requesting responses
            for (int i = 0; i < _reslist.Count; i++)
                if (_reslist[i].isValid)
                    _reslist[i].GotFill(t);
        }

        void tl_gotPosition(Position pos)
        {
            // keep track of position
            _pt.Adjust(pos);
        }

        void debug(string message)
        {
            _dw.GotDebug(message);
            _log.GotDebug(message);
        }





        void LoadResponseDLL(string filename)
        {
            // make sure response library exists
            if (!System.IO.File.Exists(filename))
            {
                status("file does not exist: " + filename);
                return;
            }

            // set response library to current library
            Properties.Settings.Default.boxdll = filename;

            // get names of responses in library
            List<string> list = Util.GetResponseList(filename);
            // clear list of available responses
            _availresponses.Items.Clear();
            // add each response to user
            foreach (string res in list)
                _availresponses.Items.Add(res);
            // update display
            _availresponses.Invalidate(true);
        }

        private void LoadDLL_Click(object sender, EventArgs e)
        {
            // get a dialog box to load a DLL
            OpenFileDialog of = new OpenFileDialog();
            of.DefaultExt = ".dll";
            of.Filter = "Response DLL|*.dll|All Files|*.*";
            // one dll at a time
            of.Multiselect = false;
            // if they choose one
            if(of.ShowDialog() == DialogResult.OK) 
                LoadResponseDLL(of.FileName); // load it
        }

        void bindresponseevents(Response tmp)
        {
            // handle all the outgoing events from the response
            tmp.SendOrderEvent += new OrderDelegate(workingres_SendOrder);
            tmp.SendDebugEvent += new DebugFullDelegate(workingres_GotDebug);
            tmp.SendCancelEvent += new UIntDelegate(workingres_CancelOrderSource);
            tmp.SendMessageEvent += new MessageDelegate(tmp_SendMessage);
            tmp.SendBasketEvent += new BasketDelegate(_workingres_SendBasket);
            tmp.SendChartLabelEvent += new ChartLabelDelegate(tmp_SendChartLabel);
            tmp.SendIndicatorsEvent += new StringParamDelegate(tmp_SendIndicators);
        }

        void tmp_SendMessage(MessageTypes type, uint source, uint dest, uint msgid, string request, ref string response)
        {
            _mtquote.SendMessage(type, source, dest, msgid, request, response);
            _mtexec.SendMessage(type, source, dest, msgid, request, response);
        }

        bool _inderror = false;
        void tmp_SendIndicators(string param)
        {
            if (!_inderror)
            {
                debug(PROGRAM + " does not support sendindicator.");
                _inderror = true;
            }
        }

        bool _charterror = false;
        void tmp_SendChartLabel(decimal price, int bar, string label, System.Drawing.Color c)
        {
            if (!_charterror)
            {
                debug(PROGRAM + " does not support sendchart.");
                _charterror = true;
            }
        }



        List<int> _disp2real = new List<int>();
        
        /// <summary>
        /// gets storage location from displayed location
        /// </summary>
        /// <param name="nameidx"></param>
        /// <returns></returns>
        int getrindx(int nameidx)
        {
            if ((nameidx<_disp2real.Count) && (nameidx>=0))
                return _disp2real[nameidx];
            return -1;
        }

        string getrstat(int idx)
        {
            if ((idx < 0) || (idx >= _reslist.Count)) 
                return "Response status error: " + idx;
            Response tmp = _reslist[idx];
            return tmp.FullName + getsyms(idx);
        }

        string getsyms(int idx) { return getsyms(idx, true); }
        string getsyms(int idx,bool brackets)
        {
            string s = string.Empty;
            if (_rsym.TryGetValue(idx, out s))
                return brackets ? " [" +s+"] " : s;
            return string.Empty;
        }

        Dictionary<int, string> _rsym = new Dictionary<int, string>();

        void editsyms(object sender, EventArgs e)
        {
            int didx = _resnames.SelectedIndex;
            if (didx == -1) return;
            int idx = getrindx(didx);
            string rname = _reslist[idx].FullName;
            string syms = Interaction.InputBox("Enter symbols seperated by commas", rname + " Symbols", getsyms(idx,false), 0, 0);
            newsyms(syms.Split(','), idx);
        }

        void newsyms(string[] syms,int idx)
        {
            // save contents
            string basket = string.Join(",", syms);
            // make sure there's a response there
            if ((idx<0) || (idx>_reslist.Count))
            {
                debug("ignoring basket "+basket+" from: " + idx);
                return;
            }
            // if good response, notify 
            if (!isBadResponse(idx))
                debug("got basket request: " + basket+ " from: " + _reslist[idx].FullName+ " "+_reslist[idx].ID);
            // save symbols
            _rsym[idx] = basket;
            // update everything
            IndexBaskets();
        }

        bool isBadResponse(int idx)
        {

            return ((idx<0) || (idx>=_reslist.Count) || (_reslist[idx] == null) || !_reslist[idx].isValid ||
                !_disp2real.Contains(idx));
        }

        void _workingres_SendBasket(Basket b, int id)
        {
            // get storage index of response from response id
            int idx = r2r(id);
            // update symbols for response
            newsyms(b.ToString().Split(','), idx);
        }

        



        uint responseid2asp(uint responseorderid)
        {
            uint id = 0;
            if (_r2a.TryGetValue(responseorderid, out id))
                return id;
            return 0;

        }

        const int EXPECTORDERS = 3000;
        IdTracker _masteridt = new IdTracker();
        Dictionary<uint, uint> _r2a = new Dictionary<uint, uint>(EXPECTORDERS);
        Dictionary<uint, uint> _a2r = new Dictionary<uint, uint>(EXPECTORDERS);

        uint aspid2responseid(uint aspid)
        {
            uint id = 0;
            if (_a2r.TryGetValue(aspid, out id))
                return id;
            return 0;
        }

        void workingres_SendOrder(Order o)
        {
            // process order coming from a response
            if (_bf.BrokerClient== null)
            {
                debug("Can't send orders, no execution broker available.");
                status("No execution broker found.");
                return;
            }
            // set account on order
            if (o.Account==string.Empty)
                o.Account = _ao._account.Text;
            try
            {
                // set the security
                if (o.Security== SecurityType.NIL)
                    o.Security = _seclist[o.symbol].Type;
                // set the exchange
                if (o.Exchange== string.Empty)
                    o.Exchange = _seclist[o.symbol].DestEx;
            }
            catch (KeyNotFoundException) 
            {
                string sym = SecurityImpl.Parse(o.symbol).Symbol;
                SecurityImpl sec = null;
                if (_seclist.TryGetValue(sym, out sec))
                {
                    o.Security = sec.Type;
                    o.Exchange = sec.DestEx;
                }
                else
                    debug("security and exchange missing on: " + o.symbol); 
            }
            // set the local symbol
            if (o.LocalSymbol==string.Empty)
                o.LocalSymbol = o.symbol;
            // assign master order if necessary
            assignmasterorderid(ref o);
            // send order and get error message
            int res = _bf.SendOrder(o);
            // if error, display it
            if (res != (int)MessageTypes.OK)
                debug(Util.PrettyError(_bf.BrokerClient.BrokerName, res) + " " + o.ToString());
        }

        void assignmasterorderid(ref Order o)
        {
            // see if we're remaping orders
            if (_ao._virtids.Checked && (o.id != 0))
            {
                // get master id for this order
                uint master = responseid2asp(o.id) ;
                // if we don't have a master, assign one
                if (master == 0)
                {
                    // get storage location for response
                    int idx = r2r(o.VirtualOwner);
                    // get a master id
                    master = _masteridt.AssignId;
                    // save association
                    _r2a.Add(o.id, master);
                    // save other way association
                    _a2r.Add(master, o.id);
                }
                // apply new id to order
                o.id = master;
            }
        }

        void workingres_CancelOrderSource(uint number)
        {
            // see if we need to remap
            if (_ao._virtids.Checked)
            {
                number = responseid2asp(number);
            }
            // pass cancels along to tradelink
            _bf.CancelOrder((long)number);
        }

        void workingres_GotDebug(Debug d)
        {
            // display to screen
            debug(d.Msg);
        }

        private void status(string msg)
        {
            // if called from background thread, invoke UI thread to perform update to screen
            if (InvokeRequired)
                Invoke(new DebugDelegate(status), new object[] { msg });
            else
            {
                // update status field
                toolStripStatusLabel1.Text = msg;
                // refresh screen area
                toolStripStatusLabel1.Invalidate();
            }
        }


        private void _togglemsgs_Click(object sender, EventArgs e)
        {
            // toggle debug msg box
            _dw.Toggle();
        }


        private void _remskin_Click(object sender, EventArgs e)
        {
            // make sure something is selected
            if (_skins.SelectedIndex == -1) return;
            // get name
            string name = _skins.SelectedItem.ToString();
            // confirm removal
            if (MessageBox.Show("remove skin " + name + "?", "confirm skin deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // remove skin and references
                remskin(name,false);
                // when done, update avail skins
                findskins();
            }
        }
        void remskin(string name) { remskin(name, true); }
        void remskin(string name, bool filesonly)
        {
            // get number of repsonses in skin
            int count = nextskinidx(SKINPATH, name);
            // remove file names
            for (int i = 0; i < count; i++)
            {
                try
                {
                    // remove skin file
                    File.Delete(SKINPATH+ name + "." + i.ToString() + SKINEXT);
                }
                catch (Exception) { continue; }
            }
            // if not processing references, quit
            if (filesonly) return;
            // remove references from loaded responses
            Dictionary<int, string> final = new Dictionary<int, string>();
            foreach (int idx in _resskinidx.Keys)
            {
                // get skins on response
                string[] names = _resskinidx[idx].Split(' ');
                // prepare final name list
                List<string> fnames = new List<string>();
                // go through each name
                for (int i = 0; i < names.Length; i++)
                    if (names[i] != name) // if it doesn't match
                        fnames.Add(names[i]); // add it
                // update the skin list for response, if we have any skins
                if (fnames.Count>0)
                    final.Add(idx,string.Join(" ", names));
            }
            // save final as our index
            _resskinidx = final;
                
        }

        private void _saveskins_Click(object sender, EventArgs e)
        {
            foreach (int idx in _resskinidx.Keys)
            {
                // get all skins this response is part of
                string[] names = _resskinidx[idx].Split(' ');
                // get response
                Response r = _reslist[idx];
                // don't save invalid responses
                if (r.Name == new InvalidResponse().Name) continue;
                // save status
                bool worked = true;
                foreach (string name in names)
                {
                    try
                    {
                        // remove skin first
                        remskin(name);
                        // then re-add it
                        worked &= SkinImpl.SkinFile(r, r.FullName, _class2dll[r.FullName], SKINPATH + name + "." + nextskinidx(SKINPATH, name).ToString() + SKINEXT, new DebugDelegate(debug));
                    }
                    catch (Exception ex)
                    {
                        debug("error saving skin: " + name);
                    }
                }
            }
            status("saved loaded skins");
        }

        private void _opttog_Click(object sender, EventArgs e)
        {
            _ao.Visible = !_ao.Visible;
            _ao.Invalidate(true);
        }

        private void _twithelp_Click_1(object sender, EventArgs e)
        {
            CrashReport.Report(PROGRAM, string.Empty, string.Empty, _dw.Content, null, null, false);
        }

        static int getprocesscount(string PROGRAM)
        {
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcesses();
            int count = 0;
            foreach (System.Diagnostics.Process p in ps)
                if (p.ProcessName.ToLower().Contains(PROGRAM.ToLower()))
                    count++;
            return count;
        }


        private void _prefquot_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_bf.ModifyFeed(_ao._datasel.SelectedIndex))
                Properties.Settings.Default.prefquote = _bf.Feed;
        }

        BrokerFeed _bf;

        private void _prefexec_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_bf.ModifyBroker(_ao._execsel.SelectedIndex))
                Properties.Settings.Default.prefexecute = _bf.Broker;
        }



                                         
    }
}