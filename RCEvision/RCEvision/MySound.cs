using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RCEvision
{
    class MySound
    {
        
        WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
        public void PlaySound()
        {
            if(new FileInfo(RemedyAlarm.soundStorage).Length == 0)
            {
                wplayer.URL = RemedyAlarm.defaultSound;
                wplayer.controls.play();
            }
            else
            {
                wplayer.URL = File.ReadAllText(RemedyAlarm.soundStorage);
                wplayer.controls.play();
            }
        }
        public void StopSound()
        {
            wplayer.controls.stop();
        }
    }
}
