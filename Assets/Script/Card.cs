using UnityEngine;

public class Card
{
    public int id { get; set; }

    public int rank { get; set; }
    public string suit { get; set; }

    public bool isFacedUp { get; set; }
    public Vector3 cardPosition { get; set; }
    public Vector3 cardRotation { get; set; }

    public void FlipCard()
    {
        isFacedUp = !isFacedUp;
    }
    public Card()
    {
        isFacedUp = false;
    }
}