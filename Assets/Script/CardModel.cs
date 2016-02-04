using UnityEngine;

public class CardModel : MonoBehaviour
{
    private Card card;
    private float startTime;
    private bool _flipCard;

    private Vector3 mousePosition;
    private Vector3 screenPoint;
    private Vector3 offset;

    // Use this for initialization
    void Awake ()
    {
        card = new Card();
    }
    
	// Use this for initialization
	void Start () {
        _flipCard = false;
        flipZoom = false;
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

        if(flipZoom)
        {
            Flip(flipZoom);
        }
    }

    void OnMouseDown()
    {
        mousePosition = Input.mousePosition;

        screenPoint = Camera.main.WorldToScreenPoint(transform.parent.position);
        offset = transform.parent.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (transform.parent.ToString().Contains("Player1") && transform.parent.GetComponent<Player>().isPlayerTurn)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

            curPosition.y = transform.parent.position.y;
            curPosition.z = transform.parent.position.z;
            transform.parent.position = curPosition;
        }
    }

    void OnMouseUp()
    {
        if (mousePosition != Input.mousePosition)
        {
            return;
        }
        //Debug.Log("Id: " + card.id);
        //Debug.Log("Rank: " + card.rank);
        //Debug.Log("Suit: " + card.suit);

        if (transform.parent.ToString().Contains("Player")) { 
            CardStack dealer = GameObject.Find("Dealer").GetComponent<CardStack>();
            Player player = transform.parent.GetComponent<Player>();

            if (player.isPlayerTurn 
                && player.localPlayer
                && !player.playerStack.isGameStack)
            {
                //dealer.Add(player.playerStack.Remove(card));
                dealer.Add(player.playerStack.TransferCard(card, dealer));
            }
        }
    }

    void PlayCard()
    {
        // for bot only
        if (transform.parent.ToString().Contains("Player"))
        {
            CardStack dealer = GameObject.Find("Dealer").GetComponent<CardStack>();
            Player player = transform.parent.GetComponent<Player>();

            if (player.isPlayerTurn 
                && !player.isHuman
                && !player.playerStack.isGameStack)
            {
                //dealer.Add(player.playerStack.Remove(card));
                dealer.Add(player.playerStack.TransferCard(card, dealer));
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

    public bool flipZoom { get; set; }

    public void Flip(bool zoom)
    {
        Vector3 angle = transform.rotation.eulerAngles;
        angle.y = card.isFacedUp ? 0f : 180f;

        if(zoom)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(3f, 3f, 3f), (Time.time - startTime) / 10f);
        }

        transform.rotation = Quaternion.Slerp(
                Quaternion.Euler(transform.rotation.eulerAngles), 
                Quaternion.Euler(angle), 
                (Time.time - startTime) / 10f);
    }
}
