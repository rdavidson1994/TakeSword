
using System;

namespace TakeSword
{
    public interface IEvent
    {
        void Happen();
    }

    public class CallbackEvent : IEvent
    {
        private System.Action callback;

        public CallbackEvent(Action callback)
        {
            this.callback = callback;
        }

        public void Happen()
        {
            callback();
        }
    }
}
