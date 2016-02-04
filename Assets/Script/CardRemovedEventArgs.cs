using System;

public class CardRemovedEventArgs : EventArgs
{
    public int CardIndex { get; private set; }
    public CardStack NewStack { get; private set; }

    public CardRemovedEventArgs(int cardIndex)
    {
        CardIndex = cardIndex;
    }
    public CardRemovedEventArgs(int cardIndex, CardStack newStack)
    {
        CardIndex = cardIndex;
        NewStack = newStack;
    }
}