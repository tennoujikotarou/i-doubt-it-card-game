using System;

public class DoubtCallEventHandlerArgs : EventArgs
{
    public bool CallDoubt { get; private set; }

    public DoubtCallEventHandlerArgs(bool callingDoubt)
    {
        CallDoubt = callingDoubt;
    }
}