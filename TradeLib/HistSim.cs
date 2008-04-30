using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TradeLib
{
    // experimental
    public class HistSim 
    {
        public event TickDelegate GotTick;
        public event FillDelegate GotFill;
        public event DebugDelegate GotDebug;
        public event IndexDelegate GotIndex;
        int _simtime = 0;
        public int NumClients { get { return 0; } }
        string _folder;
        TickFileFilter _filter = new TickFileFilter();
        bool _ticksloaded = false;
        bool _loadindex = true;
        Broker _broker = new Broker();
        string[] _tickfiles;
        public Broker SimBroker { get { return _broker; } set { _broker = value; } }
        public HistSim() : this(Util.TLTickDir, null) { }
        public HistSim(string TickFolder, TickFileFilter tff)
        {
            _folder = TickFolder;
            if (tff != null)
                _filter = tff;
        }

        public void Load()
        {
            string[] files = Directory.GetFiles(_folder);
            _tickfiles = _filter.Allows(files);

        }
        private void show(string message) { if (GotDebug!=null) GotDebug(message); }

        private Dictionary<string, List<Index>> idxstream = new Dictionary<string, List<Index>>();

        private void LoadIndexFiles(int date)
        {
            idxstream.Clear();
            Dictionary<string, StreamReader> idxfile = new Dictionary<string, StreamReader>();
            DirectoryInfo di = new DirectoryInfo(_folder);
            FileInfo[] files = di.GetFiles("*"+date+"*.idx");
            for (int i = 0; i < files.Length; i++)
                if (!idxfile.ContainsKey(files[i].FullName))
                    idxfile.Add(files[i].FullName, new StreamReader(files[i].FullName));
                else idxfile[files[i].FullName] = new StreamReader(files[i].FullName);
            show(Environment.NewLine);
            show("Preparing "+idxfile.Count+ " indicies: ");
            foreach (string stock in idxfile.Keys)
            {
                if (!idxstream.ContainsKey(stock))
                    idxstream.Add(stock, new List<Index>());
                string sym = "";
                while (!idxfile[stock].EndOfStream)
                { // read every index into memory
                    string line = idxfile[stock].ReadLine();
                    Index fi = Index.Deserialize(line);
                    idxstream[stock].Add(fi);
                    sym = fi.Name;
                }
                show("Prepared "+sym+".");
            }
        }


        List<Index> FetchIdx(int time)
        {
            List<Index> res = new List<Index>();
            foreach (string file in idxstream.Keys)
            {
                for (int i = 0; i < idxstream[file].Count; i++)
                    if (idxstream[file][i].Time == time)
                        res.Add(idxstream[file][i]);
            }
            return res;
        }
    }
}
