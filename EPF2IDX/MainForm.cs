/*
 * Created by SharpDevelop.
 * User: josh
 * Date: 2/4/2008
 * Time: 2:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using TradeLib;


namespace EPF2IDX
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
            Map("SP", "SPX");
            d("Default prefix: " + prefix);
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

        Dictionary<string, string> symmap = new Dictionary<string, string>();
        void Map(string s, string de)
        {
            if (symmap.ContainsKey(s)) symmap[s] = de.ToUpper();
            else symmap.Add(s.ToUpper(), de.ToUpper());
            d("Added SymbolMap:  " + s.ToUpper() + " -> " + de.ToUpper());
        }
				
		void d(string m)
		{
			msg.AppendText(m + Environment.NewLine);
		}
		
		void Convert(string file)
		{
			StreamReader epf = new StreamReader(file);
			Stock s = eSigTick.InitEpf(epf);
            string symbol = s.Symbol.Replace("//", "");
            if (symmap.ContainsKey(symbol))
                symbol = symbol.Replace(symbol, symmap[symbol]);
			string filename  =  symbol + s.Date + ".IDX";
			d("Attempting to convert: "+file + " -> " + filename);
			StreamWriter idx;
			try 
			{
				idx = new StreamWriter(filename,false);
			}
			catch (Exception ex) { d(ex.Message); return; }

			decimal o = 0;
			decimal h = 0;
			decimal l = 100000000000;
			while (!epf.EndOfStream)
			{
                eSigTick et = new eSigTick();
                et.sym = prefix + symbol;
				try
				{
					string line = epf.ReadLine();  // get our tick from EPF
                    et.Load(line);
                    if (et.FullQuote) continue;
				}
				catch (Exception ex) { d(ex.Message); continue; }
				
				// set o/h/l/c
				decimal v = et.trade;
				if (o==0) o = v; 
				if (v>h) h = v;
				if (v<l) l = v;
				Index i = new Index(et.sym,v,o,h,l,v,et.date,et.time); // map to our index
				idx.WriteLine(i.Serialize()); // write our index
			}
			idx.Close();
			epf.Close();
			d("Finished converting "+filename);
		}

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            OpenFileDialog o = new OpenFileDialog();
            o.Multiselect = true;
            o.InitialDirectory = "c:\\program files\\tradelink\\tickdata\\";
            o.ShowDialog();
            string[] files = o.FileNames;
            for (int i = 0; i < files.Length; i++)
                Convert(files[i]);

        }

        private void symdest_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if ((symsource.Text != null) && (symdest.Text != null))
                Map(symsource.Text, symdest.Text);
        }

        string prefix = "/";

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            prefix = "$";
            d("Set default prefix to: " + prefix);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            prefix = "/";
            d("Set default prefix to: " + prefix);
        }




		

	}
}
