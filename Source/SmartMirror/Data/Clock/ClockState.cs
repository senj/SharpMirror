using System;
using System.Threading;

namespace SmartMirror.Data.Clock
{
    public class ClockState : StateBase, IDisposable
    {
        public ClockState() : base("Clock", typeof(Shared.Clock))
        {
            // NOP
        }

        public TimeSpan TimerDuration { get; private set; }

        public string TimerName { get; private set; }

        public Timer Timer { get; private set; }

        public event Action OnTimerChange;

        public void SetTimer(string name, double durationSeconds)
        {
            TimerName = name;
            TimerDuration = TimeSpan.FromSeconds(durationSeconds);

            Timer = new Timer((_) =>
            {
                TimerDuration = TimerDuration.Subtract(TimeSpan.FromSeconds(1));
                if (TimerDuration.TotalSeconds <= 0)
                {
                    Timer.Dispose();
                    Timer = null;
                }

                OnTimerChange?.Invoke();
            }, null, 0, 1000);
        }

        public void StopTimer()
        {
            TimerName = string.Empty;
            TimerDuration = TimeSpan.FromSeconds(0);
            Timer?.Dispose();
            Timer = null;
        }

        public void Dispose()
        {
            TimerDuration = TimeSpan.FromSeconds(0);
            Timer?.Dispose();
        }
    }
}
