using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 自記温度計Tester
{
    public class GeneralTimer
    {
        public bool FlagTimeout { get; private set; }
        public int Time { get; set; }

        public bool IsRunning { get; set; }


        private System.Timers.Timer TmTimeOut;

        public GeneralTimer(int time)
        {
            TmTimeOut = new System.Timers.Timer();
            this.Time = time;
            TmTimeOut.Elapsed += (sender, e) =>
            {
                TmTimeOut.Stop();
                State.VmTestStatus.IsActiveRing = false;
                FlagTimeout = true;
            };
        }

        public void start(int NewTime = 0)
        {
            if (NewTime == 0)
            {
                TmTimeOut.Interval = Time;
            }
            else
            {
                TmTimeOut.Interval = NewTime;
            }

            TmTimeOut.Stop();
            FlagTimeout = false;
            TmTimeOut.Start();
            State.VmTestStatus.IsActiveRing = true;
            IsRunning = true;
        }

        public void stop()
        {
            State.VmTestStatus.IsActiveRing = false;
            TmTimeOut.Stop();
            IsRunning = false;
        }
    }
}
