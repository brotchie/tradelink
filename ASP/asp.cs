using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using System.IO;
using TradeLink.API;
using System.Xml.Serialization;
using Microsoft.VisualBasic;
using System.Reflection;

namespace ASP
{
    public partial class ASP : Form
    {
        public const string PROGRAM = "ASP";
        const string SKINEXT = ".skn";

        // working variables
        TLClient_WM tl = new TLClient_WM();
        Dictionary<string, SecurityImpl> _seclist = new Dictionary<string, SecurityImpl>();
        Dictionary<string, int[]> _symidx = new Dictionary<string, int[]>();
        List<Response> _reslist = new List<Response>();
        TickArchiver _ta = new TickArchiver();
        BasketImpl _mb = new BasketImpl();
        Response _workingres = new InvalidResponse();
        StreamWriter _log;
        Dictionary<int, string> _resskinidx = new Dictionary<int, string>();
        Dictionary<string, string> _class2dll = new Dictionary<string, string>();
        PositionTracker _pt = new PositionTracker();
        string[] _acct = new string[0];
        AsyncResponse _ar = new AsyncResponse();


        public ASP()
        {
            // read designer options for gui
            InitializeComponent();
            // try to open log file
            try
            {
                _log = new StreamWriter(PROGRAM + Util.ToTLDate(DateTime.Now) + ".txt", true);
                _log.AutoFlush = true;
            }
            catch (Exception ex) { debug("unable to open log file"); }
                
            // don't save ticks from replay since they're already saved
            archivetickbox.Checked = tl.LinkType != TLTypes.HISTORICALBROKER;
            // if our machine is multi-core we use seperate thread to process ticks
            if (Environment.ProcessorCount == 1)
                tl.gotTick += new TickDelegate(tl_gotTick);
            else
            {
                tl.gotTick += new TickDelegate(tl_gotTickasync);
                _ar.GotTick+=new TickDelegate(tl_gotTick);
            }
            // handle other tradelink events
            tl.gotFill += new FillDelegate(tl_gotFill);
            tl.gotOrder += new OrderDelegate(tl_gotOrder);
            tl.gotOrderCancel += new UIntDelegate(tl_gotOrderCancel);
            tl.gotPosition += new PositionDelegate(tl_gotPosition);
            tl.gotAccounts += new DebugDelegate(tl_gotAccounts);
            // if we have a server
            if (tl.LinkType != TLTypes.NONE)
            {
                // get accounts
                tl.RequestAccounts();
                // request positions
                foreach (string acct in _acct)
                    tl.RequestPositions(acct);
            }
            // setup right click menu
            _resnames.ContextMenu= new ContextMenu();
            _resnames.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            _resnames.ContextMenu.MenuItems.Add("isValid", new EventHandler(toggleresponse));
            _resnames.ContextMenu.MenuItems.Add("+Skin", new EventHandler(add2skin));
            // make sure we exit properly
            this.FormClosing += new FormClosingEventHandler(ASP_FormClosing);
            // show version to user
            status(Util.TLSIdentity());
            // check for new versions
            Util.ExistsNewVersions(tl);
            // get last loaded response library
            LoadResponseDLL(Properties.Settings.Default.boxdll);
            // load any skins we can find
            findskins();
        }


        void tl_gotTickasync(Tick t)
        {
            // on multi-core machines, this will be invoked to write ticks
            // to a cache where they will be processed by a seperate thread
            // asynchronously
            _ar.WriteIt(t);
        }

        void tl_gotAccounts(string msg)
        {
            // save accounts found connected
            _acct = msg.Split(',');
        }



        void ContextMenu_Popup(object sender, EventArgs e)
        {
            // make sure a response is selected
            if (_resnames.SelectedIndices.Count == 0) return;
            const int VALID = 0;
            // update check to reflect validity of response
            foreach (int index in _resnames.SelectedIndices)
                _resnames.ContextMenu.MenuItems[VALID].Checked = _reslist[index].isValid;

        }

        void add2skin(object sender, EventArgs e)
        {
            // make sure something is selected
            if (_resnames.SelectedIndices.Count == 0) return;
            // get name
            string name = Interaction.InputBox("What is the skin name for these responses?", "Skin name", "Skin" + DateTime.Now.Ticks.ToString(), 0, 0);
            // get next available index for this name
            int startidx = nextskinidx(Environment.CurrentDirectory,name);
            // go through all selected responses
            foreach (int idx in _resnames.SelectedIndices)
            {
                // save them as skin
                SkinImpl.SkinFile(_reslist[idx], _reslist[idx].FullName, _class2dll[_reslist[idx].FullName], name + "." + startidx.ToString() + SKINEXT);
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
            }
            status("added " + _resnames.SelectedIndices.Count + " responses to " + name);
            // find any new names
            findskins();
        }



        void findskins()
        {
            // clear existing skins
            _skins.Items.Clear();

            // go through every skin file
            foreach (string fn in skinfiles(Environment.CurrentDirectory))
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
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
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
        }

        bool tradeskins(string name)
        {
            // get skin files available
            string[] files = skinfiles(Environment.CurrentDirectory);
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
                        // get it along with it's persisted settings
                        Response r = (Response)SkinImpl.DeskinFile(fn);
                        // add it to trade list
                        _reslist.Add(r);
                        // show it to user
                        _resnames.Items.Add(r.FullName);
                        // mark it as loaded
                        _resskinidx.Add(_reslist.Count - 1, name);
                        // route symbols to it?

                        // update status
                        worked &= true;
                    }
                }
                return true;
            }
            catch (Exception) { }
            return false;
        }




        void toggleresponse(object sender, EventArgs e)
        {
            // process each selected response
            foreach (int selbox in _resnames.SelectedIndices)
            {
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
            // send order cancel notification to every valid box
            foreach (string sym in _symidx.Keys)
                foreach (int idx in _symidx[sym])
                    if (_reslist[idx].isValid)
                        _reslist[idx].GotOrderCancel(number);
        }

        void tl_gotOrder(Order o)
        {
            // make sure we are tracking this notifications symbol somewhere
            int[] idxs = new int[0];
            if (!_symidx.TryGetValue(o.Sec.FullName, out idxs))
                return;
            // send order notification to every valid box
            foreach (int idx in idxs)
                if (_reslist[idx].isValid)
                    _reslist[idx].GotOrder(o);
        }

        void ASP_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((_resskinidx.Count>0) &&
                (MessageBox.Show("Save skins before quiting?", "Save skins", MessageBoxButtons.YesNo) == DialogResult.Yes))
                _saveskins_Click(null, null);
            // if we're using another thread to process ticks, stop it
            if (Environment.ProcessorCount>1)
                _ar.Stop();
            // stop archiving ticks
            _ta.CloseArchive();
            // if log file exists, close it
            if (_log != null)
            {
                _log.Close();
                _log = null;
            }
            // save ASP properties
            Properties.Settings.Default.Save();

            // shut down tradelink client
            try
            {
                tl.Disconnect();
            }
            catch (TLServerNotFound) { }
            
        }

        void skinexit()
        {

        }




       
        void tl_gotTick(Tick t)
        {
            // see if we are tracking this symbol
            int[] idxs = new int[0];
            if (!_symidx.TryGetValue(t.Sec.FullName, out idxs))
                return;

            // see if we should save this tick
            if (archivetickbox.Checked)
                _ta.Save(t);

            // send tick to any valid requesting responses
            foreach (int idx in idxs)
                if (_reslist[idx].isValid) 
                    _reslist[idx].GotTick(t);
        }



        void tl_gotFill(Trade t)
        {
            // keep track of position
            _pt.Adjust(t);
            // get requesting responses if any
            int[] idxs = new int[0];
            // if no requestors, ignore symbol
            if (!_symidx.TryGetValue(t.Sec.FullName, out idxs))
                return;
            // send trade notification to any valid requesting responses
            foreach (int idx in idxs)
                if (_reslist[idx].isValid)
                    _reslist[idx].GotFill(t);
        }

        void tl_gotPosition(Position pos)
        {
            // keep track of position
            _pt.Adjust(pos);
        }

        void debug(string message)
        {
            // get a timestamp
            string stamp = DateTime.Now.ToShortTimeString()+ " ";
            // if we have a logfile, log the debug
            if (_log != null)
                _log.WriteLine(stamp+message);

            // if we're running from a background thread, invoke GUI thread to update screen
            if (_msg.InvokeRequired)
                _msg.Invoke(new DebugDelegate(debug), new object[] { message });
            else
            {
                // add debug msg
                _msg.Items.Add(stamp+message);
                // select it
                _msg.SelectedIndex = _msg.Items.Count - 1;
                // refresh display
                _msg.Invalidate(true);
            }
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




        private void Boxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // make sure something is selected
            if (_availresponses.SelectedIndex == -1) return;
            // get selected response
            string resname = (string)_availresponses.SelectedItem;
            // load it into working response
            _workingres = ResponseLoader.FromDLL(resname, Properties.Settings.Default.boxdll);
            // handle all the outgoing events from the response
            _workingres.SendOrder += new OrderDelegate(workingres_SendOrder);
            _workingres.SendDebug+= new DebugFullDelegate(workingres_GotDebug);
            _workingres.SendCancel+= new UIntDelegate(workingres_CancelOrderSource);

            // save the dll that contains the class for use with skins
            string dll = string.Empty;
            // if we don't have this class, add it
            if (!_class2dll.TryGetValue(resname, out dll))
                _class2dll.Add(resname, Properties.Settings.Default.boxdll);
            else // otherwise replace current dll as providing this class
                _class2dll[resname] = Properties.Settings.Default.boxdll;

        }

        void workingres_SendOrder(Order o)
        {
            // process order coming from a response

            // set account on order
            o.Account = _account.Text;
            // set the security
            o.Security = _seclist[o.symbol].Type;
            // set the exchange
            o.Exchange = _seclist[o.symbol].DestEx;
            // set the local symbol
            o.LocalSymbol = o.symbol;
            // send order and get error message
            int res = tl.SendOrder(o);
            // if error, display it
            if (res != (int)MessageTypes.OK)
                debug(Util.PrettyError(tl.BrokerName, res) + " " + o.ToString());
        }

        void workingres_CancelOrderSource(uint number)
        {
            // pass cancels along to tradelink
            tl.CancelOrder((long)number);
        }

        void workingres_GotDebug(Debug d)
        {
            // see if we are processing debugs
            if (!debugon.Checked) return;
            // display to screen
            debug(d.Msg);
        }

        private void Trade_Click(object sender, EventArgs e)
        {
            // user clicks on trade button

            // make sure a response is selected
            if (_availresponses.SelectedIndex == -1)
            {
                status("Please select a response.");
                return;
            }


            // get all the provided symbols
            string[] syms = _symstraded.Text.Split(',');
            // prepare a list of valid symbols
            List<string> valid = new List<string>();
            // process every provided symbol
            foreach (string symt in syms)
            {
                // make it uppercase
                string sym = symt.ToUpper();
                // parse out security information
                SecurityImpl sec = SecurityImpl.Parse(sym);
                // if it's an invalid security, ignore it
                if (!sec.isValid)
                {
                    status("Security invalid: " + sec.ToString());
                    continue;
                }
                // otherwise add the security
                _mb.Add(sec);
                // save simple symbol as valid
                valid.Add(sec.Symbol);
                // if we don't have this security
                if (!_seclist.ContainsKey(sec.Symbol))
                {
                    // lock so other threads don't modify seclist at same time
                    lock (_seclist) 
                    {
                        // add security to our list
                        _seclist.Add(sec.Symbol,sec);
                    }
                }

            }
            // add working response to response list after obtaining a lock
            lock (_reslist) 
            {
                _reslist.Add(_workingres);
            }
            // get index to this response
            int idx = _reslist.Count -1;
            // send response current positions
            foreach (Position p in _pt)
                _reslist[idx].GotPosition(p);
            // subscribe to whatever symbols were requested
            try
            {
                tl.Subscribe(_mb);
            }
            catch (TLServerNotFound) { debug("subscribe failed because no TL server is running."); }
            // add name to user's screen
            _resnames.Items.Add(_workingres.FullName+" ["+string.Join(",",valid.ToArray())+"]");
            // update their screen
            _resnames.Invalidate(true);
            // process all securities and build  a quick index for a security's name to the response that requests it
            foreach (SecurityImpl sec in _seclist.Values)
                if (_symidx.ContainsKey(sec.FullName)) // we already had one requestor
                {
                    // get current length of request list for security
                    int len = _symidx[sec.FullName].Length;
                    // add one to it for our new requestor
                    int[] a = new int[len + 1];
                    // add our new requestor's index at the end
                    a[len] = idx;
                }
                else // otherwise it's just this guy so add him 
                    _symidx.Add(sec.FullName, new int[] { idx });
            // clear the symbol list
            _symstraded.Clear();
            // show we added response
            status(_workingres.FullName+" ["+string.Join(",",valid.ToArray())+"]");
            // unselect response
            _availresponses.SelectedIndex = -1;
            
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


        private void stock_KeyUp(object sender, KeyEventArgs e)
        {
            // in case they don't click trade, click  it for them when they press enter
            if (e.KeyData == Keys.Enter)
                Trade_Click(null, null);
        }

        private void _togglemsgs_Click(object sender, EventArgs e)
        {
            // toggle debug msg box
            _msg.Visible = !_msg.Visible;
            // refresh screen
            _msg.Invalidate();
        }

        private void _twithelp_Click(object sender, EventArgs e)
        {
            // popup twitter window
            TwitPopup.Twit();
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
            int count = nextskinidx(Environment.CurrentDirectory, name);
            // remove file names
            for (int i = 0; i < count; i++)
            {
                try
                {
                    // remove skin file
                    File.Delete(Environment.CurrentDirectory + "\\" + name + "." + i.ToString() + SKINEXT);
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
                // save status
                bool worked = true;
                foreach (string name in names)
                {
                    // remove skin first
                    remskin(name);
                    // then re-add it
                    worked &= SkinImpl.SkinFile(r, r.FullName, _class2dll[r.FullName], name + "." + nextskinidx(Environment.CurrentDirectory, name).ToString() + SKINEXT);
                }
            }
            status("saved loaded skins");
        }

                                         
    }
}