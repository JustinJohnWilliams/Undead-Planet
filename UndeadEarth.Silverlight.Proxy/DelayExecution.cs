using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UndeadEarth.Silverlight.Proxy
{
    public class DelayExecution
    {
        private DelayExecutionAction _delayExecutionAction;
        public void SetTimeout(int milliseconds, Action function)
        {
            if (_delayExecutionAction != null)
            {
                _delayExecutionAction.Stop();
                _delayExecutionAction = null;
            }

            _delayExecutionAction = new DelayExecutionAction
            {
                Interval = new TimeSpan(0, 0, 0, 0, milliseconds),
                Action = function
            };

            _delayExecutionAction.Tick += _onTimeout;
            _delayExecutionAction.Start();
        }

        private void _onTimeout(object sender, EventArgs arg)
        {
            var t = sender as DelayExecutionAction;
            t.Stop();
            t.Action();
            t.Tick -= _onTimeout;
        }

        public class DelayExecutionAction : DispatcherTimer
        {
            public Action Action { get; set; }
        }
    }
}
