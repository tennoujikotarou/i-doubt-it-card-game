using UnityEngine;
using System.Collections;

public class Card
{
    private int _id;
    private int _rank;
    private string _suit;

    private bool faceUp;

    public Card() {
        faceUp = false;
    }

    public int id
    {
        get { return _id; }
        set { _id = value; }
    }

    public int rank
    {
        get { return _rank; }
        set { _rank = value; }
    }
    public string suit
    {
        get { return _suit; }
        set { _suit = value; }
    }

    public bool isFacedUp
    {
        get { return faceUp; }
        set { faceUp = value; }
    }

    public void FlipCard()
    {
        faceUp = !faceUp;
    }
}
