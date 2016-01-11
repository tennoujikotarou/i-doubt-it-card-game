using UnityEngine;
using System.Collections;

public class CardModel : MonoBehaviour
{
    private Card card;

    // Use this for initialization
    void Awake ()
    {
        card = new Card();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown()
    {
        Debug.Log(card.id);
        Debug.Log(card.rank);
        Debug.Log(card.suit);

        CardStack dealer = GameObject.Find("Dealer").GetComponent<CardStack>();
        CardStack player = transform.parent.GetComponent<CardStack>();

        if (!player.isGameStack)
        {
            dealer.Add(player.Remove(card));
        }
    }

    public int id
    {
        get { return card.id; }
        set { card.id = value; }
    }

    public int rank
    {
        get { return card.rank; }
        set { card.rank = value; }
    }
    public string suit
    {
        get { return card.suit; }
        set { card.suit = value; }
    }

    public bool isFacedUp
    {
        get { return card.isFacedUp; }
        set { card.isFacedUp = value; }
    }

    public void FlipCard()
    {
        card.FlipCard();
    }
}
