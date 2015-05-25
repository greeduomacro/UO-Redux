using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReduxLauncher.Modules
{
    public partial class PatcherInterface : Form
    {
        PatchHandler handler;

        public PatcherInterface()
        {
            InitializeComponent();

            Main_action_btn.Enabled = false;
            Main_action_btn.Text = "Please Wait..";

            handler = new PatchHandler(this);

            progressBar.Style = ProgressBarStyle.Continuous;

            BackColor = Color.WhiteSmoke;
            TransparencyKey = Color.WhiteSmoke;

            Background.Load(PatchHelper.BackgroundURL);
        }

        internal void ReadyLaunch()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    Main_action_btn.Enabled = true;
                    Main_action_btn.Text = "Launch";
                    ProgressBar().Value = ProgressBar().Maximum;
                }));
            }

            else
            {
                Main_action_btn.Enabled = true;
                Main_action_btn.Text = "Launch";
                ProgressBar().Value = ProgressBar().Maximum;
            }
        }

        internal void ReadyDownload()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    Main_action_btn.Enabled = true;
                    Main_action_btn.Text = "Download";
                    ProgressBar().Value = ProgressBar().Minimum;
                }));
            }

            else
            {
                Main_action_btn.Enabled = true;
                Main_action_btn.Text = "Download";
                ProgressBar().Value = ProgressBar().Minimum;
            }
        }

        internal void ReadyInstall()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    Main_action_btn.Enabled = true;
                    Main_action_btn.Text = "Install Redux";
                    ProgressBar().Value = ProgressBar().Minimum;
                }));
            }

            else
            {
                Main_action_btn.Enabled = true;
                Main_action_btn.Text = "Install Redux";
                ProgressBar().Value = ProgressBar().Minimum;
            }
        }

        internal ProgressBar ProgressBar()
        {
            return progressBar;
        }

        internal void UpdatePatchNotes(string notes)
        {
            LogHandler.LogActions(notes);
        }

        internal void UpdateProgressBar()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    if (ProgressBar().Value < ProgressBar().Maximum)
                        ProgressBar().PerformStep();

                    else
                        ProgressBar().Value = ProgressBar().Minimum;
                }));
            }

            else
            {
                if (ProgressBar().Value < ProgressBar().Maximum)
                    ProgressBar().PerformStep();

                else
                    ProgressBar().Value = ProgressBar().Minimum;
            }
        }

        private void Main_action_btn_Click(object sender, EventArgs e)
        {
            if (handler.isReady == false)
            {
                Main_action_btn.Enabled = false;

                Main_action_btn.Text = "Waiting..";

                if (handler.InitialDownload())
                    MessageBox.Show("The initial installation may take a while.");

                Task.Factory.StartNew(InitializeDownload);

                Main_action_btn.Text = "Downloading..";

                if (handler.InitialDownload())
                    MessageBox.Show("You may view the action log in the 'advanced' tab at the top.");
            }

            else if (handler.isReady)
            {
                handler.LaunchClient();
            }
        }

        private void toXML_btn_Click(object sender, EventArgs e)
        {
            handler.RelayXMLGeneration();
        }

        private async void InitializeDownload()
        {
            await handler.InitializeDownload();
        }
    }
}
