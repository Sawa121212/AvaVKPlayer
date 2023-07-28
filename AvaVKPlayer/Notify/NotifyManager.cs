using System.Collections.Generic;
using System.Threading;

namespace AvaVKPlayer.Notify
{
    public class NotifyManager
    {
        private Thread _thread;

        private Queue<NotifyData> _notifyDataQueUe = new Queue<NotifyData>();

        private static NotifyManager _notifyManager;
        private INotifyControl NotifyControl { get; set; }

        public static NotifyManager Instance
        {
            get => _notifyManager = (_notifyManager ?? new NotifyManager());
        }
        public void SetNotifyControl(INotifyControl notifyControl) =>
                NotifyControl = notifyControl;
        private void Process()
        {
            while (_notifyDataQueUe.Count > 0)
            {

                var q = _notifyDataQueUe.Dequeue();

                Thread.Sleep((int)q.ShowDelayTime.TotalMilliseconds);

                NotifyControl.ShowNotify(q.Title, q.Message);

                Thread.Sleep((int)q.ShowTIme.TotalMilliseconds);
                NotifyControl.Hide();


            };

        }


        public void PopMessage(NotifyData data)
        {
            _notifyDataQueUe.Enqueue(data);

            if (_thread == null
                || _thread.ThreadState == ThreadState.Stopped
                || _thread.ThreadState == ThreadState.Suspended)
            {
                _thread = new Thread(Process);
                _thread.IsBackground = true;
                _thread.Start();
            }


        }



    }
}