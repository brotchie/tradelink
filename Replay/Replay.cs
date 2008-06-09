using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLib;

namespace TLReplay
{
    public partial class Replay : Form
    {
        TradeLink_Server_WM tl;
        DayPlayback play = null;
        Broker broker = new Broker();


        public Replay()
        {
            InitializeComponent();
            tl = new TradeLink_Server_WM(Text);
        }



        ~Replay()
        {
            Properties.Settings.Default.Save();
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            int date = (e.Start.Year * 10000) + (e.Start.Month * 100) + e.Start.Day;
        }

        private void inputselectbut_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            f.SelectedPath = TLReplay.Properties.Settings.Default.tickfolderpath;
            f.ShowNewFolderButton = false;
            f.RootFolder = Environment.SpecialFolder.ProgramFiles;
            f.Description = "Current Tick Source: " + TLReplay.Properties.Settings.Default.tickfolderpath + Environment.NewLine+"New Tick Source:";
            f.ShowDialog();
            TLReplay.Properties.Settings.Default.tickfolderpath = f.SelectedPath;
        }


        private void gobut_Click(object sender, EventArgs e)
        {
            int date = (10000*monthCalendar1.SelectionStart.Year) + (100*monthCalendar1.SelectionStart.Month) + monthCalendar1.SelectionStart.Day;
            
            try
            {
                int[] d = BarMath.Date(date);
                currdate.Text = d[1].ToString() + "/" + d[2].ToString() + "/" + d[0].ToString();
            }
            catch (Exception) { MessageBox.Show("Your date is invalid.", "Replay can't start."); return; }

            play = new DayPlayback(Properties.Settings.Default.tickfolderpath + "\\", date.ToString());
            play.TLInst = tl;

            gobut.Enabled = false;
            nowplayinggrp.Enabled = true;
            speedbar.Enabled = false;
            inputselectbut.Enabled = false;
            monthCalendar1.Enabled = false;

            play.ExchFilter("NYS");
            play.DelayMult(speedbar.Value/5);

            play.RunWorkerCompleted += new RunWorkerCompletedEventHandler(play_RunWorkerCompleted);
            play.RunWorkerAsync();
            Refresh();
        }

        void play_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            gobut.Enabled = true;
            nowplayinggrp.Enabled = false;
            speedbar.Enabled = true;
            inputselectbut.Enabled = true;
            monthCalendar1.Enabled = true;
        }


        private void pausebut_Click(object sender, EventArgs e)
        {
            play.CancelAsync();

        }

        private void stopbut_Click(object sender, EventArgs e)
        {
            play.CancelAsync();
        }

        private void speedbar_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Clicks > 0) && (e.Button == MouseButtons.Middle)) 
                speedbar.Value = 5;
        }


    }
}