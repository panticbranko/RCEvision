using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RCEvision
{
    class TicketObservation
    {
        public delegate void StatusMessageHandler(object sender, StatusMessages args);
        public event StatusMessageHandler Msg;
        Scanning oScann = new Scanning();
        public static MySound ms = new MySound();
        public void Listen(CancellationToken token)
        {
            Bitmap rce1 = Properties.Resources.RCE1;
            Bitmap rce2 = Properties.Resources.RCE2;
            Bitmap rce3 = Properties.Resources.RCE3;
            Bitmap rce4 = Properties.Resources.RCE4;
            Bitmap rce5 = Properties.Resources.RCE5;
            Bitmap rce6 = Properties.Resources.RCE6;
            Bitmap rce7 = Properties.Resources.RCE7;
            Bitmap rce8 = Properties.Resources.RCE8;
            Bitmap rce9 = Properties.Resources.RCE9;
            Bitmap rce10 = Properties.Resources.RCE10;
            Bitmap rce11 = Properties.Resources.RCE11;
            Bitmap rce12 = Properties.Resources.RCE12;
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(1000);
                oScann.screenShot();
                Bitmap screen = new Bitmap(oScann.getResult());
                if (Scanning.IsMatchFound(screen, rce1) || Scanning.IsMatchFound(screen, rce2) || Scanning.IsMatchFound(screen, rce3) || Scanning.IsMatchFound(screen, rce4) || Scanning.IsMatchFound(screen, rce5) || Scanning.IsMatchFound(screen, rce6) || Scanning.IsMatchFound(screen, rce7) || Scanning.IsMatchFound(screen, rce8) || Scanning.IsMatchFound(screen, rce9) || Scanning.IsMatchFound(screen, rce10) || Scanning.IsMatchFound(screen, rce11) || Scanning.IsMatchFound(screen, rce12))
                {
                    ms.PlaySound();
                    Msg(this, new StatusMessages { messageIs = "ticked arived" });
                    MsgBox.Show("New ticket arived", "ticket info", MsgBox.Buttons.OK, MsgBox.Icon.Info, MsgBox.AnimateStyle.ZoomIn);
                    break;
                }
            }
        }

    }
}
