using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TradeLib
{
    public class TickArchiver
    {
        string _path;
        System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();

        Dictionary<string,StreamWriter> filedict = new Dictionary<string,StreamWriter>();
        public TickArchiver() : this(Util.TLTickDir,60) { }
        public TickArchiver(string folderpath, int FlushAfterSeconds)
        {
            _path = folderpath;
            _timer.Interval = 1000 * FlushAfterSeconds;
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }
        public void CloseArchive()
        {
            foreach (string file in filedict.Keys)
                filedict[file].Close();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            foreach (string sym in filedict.Keys)
                filedict[sym].Flush();
        }

        bool SaveTick(Tick t)
        {
            if ((t.sym==null) || (t.sym=="")) return false;
            if (filedict.ContainsKey(t.sym))
            {
                try 
                {
                   filedict[t.sym].WriteLine(eSigTick.ToEPF(t));
                }
                catch (IOException) { return false; }
            }
            else
            {
                string fn = _path + @"/" + t.sym + t.date + ".EPF";
                bool hasheader = false;
                try 
                {
                    if (File.Exists(fn))
                    {
                        StreamReader sr = new StreamReader(fn);
                        Stock s = eSigTick.InitEpf(sr);
                        if (s.isValid)
                            hasheader = true;
                        sr.Close();
                    }
                    filedict.Add(t.sym, new StreamWriter(fn, true));
                    if (!hasheader)
                        filedict[t.sym].Write(eSigTick.EPFheader(t.sym, t.date));
                    filedict[t.sym].WriteLine(eSigTick.ToEPF(t));
                }
                catch (IOException) { return false; }
                catch (Exception) { return false; }
            }

            return true;
        }

        bool SaveIndex(Index i)
        {
            if ((i == null) || (i.Name == null) || (i.Name == "") || !Index.isIdx(i.Name)) return false;
            if (!filedict.ContainsKey(i.Name))
            {
                try
                {
                    string fn = _path + @"/" + i.Name + i.Date + ".IDX";
                    filedict.Add(i.Name, new StreamWriter(fn, true));
                }
                catch (Exception) { return false; }
            }
            try
            {
                filedict[i.Name].WriteLine(i.Serialize());
            }
            catch (Exception) { return false; }
            filedict[i.Name].Flush();
            return true;
        }

        public bool Save(Index i) { return SaveIndex(i); }
        public bool Save(Tick t) { return SaveTick(t); }

    }
}
