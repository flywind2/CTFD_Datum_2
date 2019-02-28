using System;

namespace CTFD.Global.Common
{
    public class GlobalEventArgs : EventArgs
    {
        public GlobalEvent GlobalEvent { get; private set; }
        public object Value { get; private set; }

        public GlobalEventArgs(GlobalEvent globalEvent, object value)
        {
            this.GlobalEvent = globalEvent;
            this.Value = value;
        }
    }
}
