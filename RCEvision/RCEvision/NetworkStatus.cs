using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCEvision
{
    class NetworkStatus
    {
        public delegate void StatusMessageHandler(Object sender, StatusMessages args);
        public event StatusMessageHandler Msg;
        public static MySound ms = new MySound();
        public static bool PingHost(string nameOrAdress)
        {
            bool testPing = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(nameOrAdress);
                testPing = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
            return testPing;
        }

        public void MonitorNetwork(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                bool test = PingHost("8.8.8.8");
                Thread.Sleep(1000);
                if (!test)
                {
                    ms.PlaySound();
                    Msg( this, new StatusMessages { messageIs = "Network cable is unpluged"});
                    MsgBox.Show("Check network connection", "Connection problem", MsgBox.Buttons.OK, MsgBox.Icon.Error);
                    break;
                }
            }
        }
    }
}
