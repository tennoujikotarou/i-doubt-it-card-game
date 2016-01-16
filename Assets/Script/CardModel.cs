using UnityEngine;

public class CardModel : MonoBehaviour
{
    private Card card;
    private Vector3 mousePosition;
    private float startTime;
    private bool _flipCard;

    // Use this for initialization
    void Awake ()
    {
        card = new Card();
    }
    
	// Use this for initialization
	void Start () {
        _flipCard = false;
        startTime = 100f;
    }
	
	// Update is called once per frame
	void Update () {

	}

    void FixedUpdate ()
    {
        if (_flipCard)
        {
            startTime = Time.time;
            _flipCard = false;
        }
        Flip();
    }

    void OnMouseDown()
    {
        mousePosition = Input.mousePosition;
    }

    void OnMouseUp()
    {
        //if(mousePosition != Input.mousePosition)
        //{
        //    return;
        //}
        Debug.Log("Id: " + card.id);
        Debug.Log("Rank: " + card.rank);
        Debug.Log("Suit: " + card.suit);

        if(transform.parent.ToString().Contains("Player")) { 
            CardStack dealer = GameObject.Find("Dealer").GetComponent<CardStack>();
            Player player = transform.parent.GetComponent<Player>();
            CardStack playerStack = player.playerStack;

            if (player.isPlayerTurn && !playerStack.isGameStack)
            {
                dealer.Add(playerStack.Remove(card));
            }
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

    public bool flipCard {
        get { return _flipCard; }
        set { _flipCard = value; card.FlipCard(); }
    }

    public void Flip()
    {
        Vector3 angle = transform.rotation.eulerAngles;
        angle.y = card.isFacedUp ? 0f : 180f;

        transform.rotation = Quaternion.Slerp(Quaternion.Euler(transform.rotation.eulerAngles), Quaternion.Euler(angle), (Time.time - startTime) / 1.5f);
    }
}
