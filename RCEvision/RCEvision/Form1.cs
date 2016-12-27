using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RCEvision
{
    public partial class RemedyAlarm : MaterialForm
    {
        public static string rootFolder = "c:/RemedyAlarm";
        public static string defaultSound = "sound.mp3";
        public static string currentSound = "";
        public static string soundStorage = "c:/RemedyAlarm/storeSoundFile.txt";
        static PowerStatus ps = new PowerStatus();
        static NetworkStatus ns = new NetworkStatus();
        static TicketObservation to = new TicketObservation();
        static CancellationTokenSource ts = new CancellationTokenSource();
        CancellationToken token = ts.Token;
        static bool start = true;
        

        public RemedyAlarm()
        {
            InitializeComponent();
   
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }
            if (!File.Exists(soundStorage))
            {
                File.Create(soundStorage).Close();
            }
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        async Task Listen(CancellationToken token)
        {
            Task.Factory.StartNew(() => ps.MonitorPowerAC(token), token);
            Task.Factory.StartNew(() => ns.MonitorNetwork(token), token);
            Task.Factory.StartNew(() => to.Listen(token), token);
        }

        async void RunOrStop()
        {
            if (start == true)
            {
                ts.Dispose();
                ts = new CancellationTokenSource();
                await Listen(ts.Token);
            }
            else
            {
                ts.Cancel();
            }
        }

        private void RemedyAlarm_Load(object sender, EventArgs e)
        {

        }

        

        private void materialFlatButton2_Click(object sender, EventArgs e)
        {
            start = false;
            RunOrStop();
            PowerStatus.ms.StopSound();
            NetworkStatus.ms.StopSound();
            TicketObservation.ms.StopSound();
            toolStripStatusLabel1.Text = "Alarm is deactivated";
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Alarm is activated";
            start = true;
            RunOrStop();
            ps.Msg += (object send, StatusMessages ars) => { toolStripStatusLabel1.Text = ars.messageIs; };
            ns.Msg += (object send, StatusMessages ars) => { toolStripStatusLabel1.Text = ars.messageIs; };
            to.Msg += (object send, StatusMessages ars) => { toolStripStatusLabel1.Text = ars.messageIs; };
        }

        private void materialFlatButton3_Click(object sender, EventArgs e)
        {
            var fdlg = new OpenFileDialog();
            fdlg.Title = "Open a file";
            fdlg.InitialDirectory = "c:/";
            fdlg.Filter = "only mp3 Files (*.mp3)| *.mp3";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                currentSound = fdlg.FileName;
            }
            File.WriteAllText(soundStorage, currentSound);
            MsgBox.Show("mp3 file path: " + currentSound, "New alarm theme is selected", MsgBox.Buttons.OK, MsgBox.Icon.Info, MsgBox.AnimateStyle.FadeIn);
        }
    }
}
