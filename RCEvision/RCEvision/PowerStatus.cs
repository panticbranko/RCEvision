using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCEvision
{
    class PowerStatus
    {
        public delegate void StatusMessageHandler(object sender, StatusMessages args);
        public event StatusMessageHandler Msg;
        public static MySound ms = new MySound();
        public void MonitorPowerAC(CancellationToken token)
        {
            if (token.IsCancellationRequested)  ms.StopSound(); 
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(100);
                bool checkStatus = (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online);
                if (checkStatus)
                {
                    ;
                }
                else
                {
                    Msg(this, new StatusMessages { messageIs = "Power loss" });
                    ms.PlaySound();
                    MsgBox.Show("Check Power Cable", "Power problem", MsgBox.Buttons.OK, MsgBox.Icon.Warning, MsgBox.AnimateStyle.FadeIn);
                    break;
                }
            }
            
        }
    }
}
