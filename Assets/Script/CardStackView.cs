using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardStack))]

public class CardStackView : MonoBehaviour
{
    public GameObject cardPrefab;
    private Sprite[] cardTextures;
    private CardStack cardStack;

    private Dictionary<int, GameObject> fetchedCard;
    private int stackLastCount;
    
    private float startTime;
    private Vector3 startingPos;

    private Vector3 position;
    private Vector3 angle;
    private Vector3 scale;
    float cardWidth;
    float maxWidth;
    float leftPos;
    float margin;

    void Awake()
    {
        //cardPrefab = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Prefab/Card.prefab", typeof(GameObject));
        cardTextures = Resources.LoadAll<Sprite>("card_sheet_small");
    }

    // Use this for initialization
    void Start()
    {
        cardStack = GetComponent<CardStack>();

        fetchedCard = new Dictionary<int, GameObject>();
        stackLastCount = cardStack.Size;
        cardStack.cardRemoved += CardStack_cardRemoved;

        // Get the game object's position to put the cards on
        startTime = Time.time;
        startingPos = transform.position;

        ShowCards(false);
    }

    void OnEnable()
    {
        //if(cardStack != null) { 
        //    cardStack.cardRemoved += CardStack_cardRemoved;
        //}
    }
    void OnDisable()
    {
        cardStack.cardRemoved -= CardStack_cardRemoved;
    }

    private void CardStack_cardRemoved(object sender, CardRemovedEventArgs e)
    {
        if (fetchedCard.ContainsKey(e.CardIndex))
        {
            Destroy(fetchedCard[e.CardIndex]);
            fetchedCard.Remove(e.CardIndex);
        }
    }

    bool isZooming = false;
    // Update is called once per frame
    void Update()
    {
        if(cardStack.zoomCard)
        {
            startTime = Time.time;
            cardStack.zoomCard = false;
            isZooming = true;
        }
    }

    void FixedUpdate()
    {
        if (stackLastCount != cardStack.Size)
        {
            startTime = Time.time;
            stackLastCount = cardStack.Size;
            ShowCards(true);
        }

        int offset = -1;
        foreach (Card card in cardStack.GetCards())
        {
            offset++;

            if (offset >= transform.childCount)
            {
                break;
            }

            GameObject cardCopy = transform.GetChild(offset).gameObject;
            ArrangeCard(card, cardCopy, transform.position, offset);
        }


        // make the cards stand next to each other
        if (transform.ToString().Contains("Player1") && isZooming)
        {
            if (GetComponent<Player>().isPlayerTurn)
            {
                cardWidth = cardPrefab.GetComponent<BoxCollider2D>().size.x * 1.75f;
                maxWidth = 16f;// transform.GetComponent<BoxCollider2D>().size.x;
                leftPos = startingPos.x - (maxWidth / 2);

                float newWidth = (cardWidth + 0.2f) * cardStack.Size;
                float newPosX = leftPos + (newWidth / 2);

                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, -5.23f, -1f), (Time.time - startTime) / 1f);
                transform.GetComponent<BoxCollider2D>().size = new Vector2(newWidth, 3.5f);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, startingPos, (Time.time - startTime) / 1f);
                transform.GetComponent<BoxCollider2D>().size = new Vector2(16f, 3.5f);
            }

            if((Time.time - startTime) / 1f > 1) { isZooming = false; }
        }
    }

    private void ShowCards(bool hasCardPosition)
    {
        int offset = -1;
        if (cardStack.HasCard)
        {
            foreach (Card card in cardStack.GetCards())
            {
                offset++;

                if (fetchedCard.ContainsKey(card.id))
                {
                    continue;
                }

                GameObject cardCopy;
                if (hasCardPosition)
                {
                    cardCopy = (GameObject)Instantiate(cardPrefab, card.cardPosition, Quaternion.Euler(card.cardRotation));
                    cardCopy.transform.localScale = card.scaleValue;
                }
                else
                {
                    cardCopy = Instantiate(cardPrefab);
                }

                card.isFacedUp = cardStack.isGameStack ? false : true;

                if(cardStack.transform.GetComponent<Player>() != null)
                {
                    card.isFacedUp = cardStack.transform.GetComponent<Player>().localPlayer ? true : false;
                }

                cardCopy.AddComponent<CardModel>();
                cardCopy.GetComponent<CardModel>().id = card.id;
                cardCopy.GetComponent<CardModel>().rank = card.rank;
                cardCopy.GetComponent<CardModel>().suit = card.suit;
                cardCopy.GetComponent<CardModel>().isFacedUp = card.isFacedUp;

                GameObject cardFaceTexture = cardCopy.transform.Find("CardFace").gameObject;
                cardFaceTexture.GetComponent<SpriteRenderer>().sprite = cardTextures[card.id];

                ArrangeCard(card, cardCopy, transform.position, offset);
                fetchedCard.Add(card.id, cardCopy);
            }
        }
    }

    void ArrangeCard(Card cardAdd, GameObject cardCopyAdd, Vector3 startingPosition, int offset)
    {
        position = new Vector3(startingPosition.x + 0.005f * offset, startingPosition.y, startingPosition.z - 0.0001f * offset - 0.0001f);
        angle = transform.rotation.eulerAngles;
        scale = new Vector3(1f, 1f, 1f);

        if (cardStack.isGameStack)
        {
            scale = new Vector3(1.25f, 1.25f, 1.25f);
        }
        else if (cardStack.transform.ToString().Contains("Player1"))
        {
            scale = new Vector3(1.75f, 1.75f, 1.75f);
        }
        else
        {
            scale = new Vector3(0.75f, 0.75f, 0.75f);
        }

        cardWidth = cardPrefab.GetComponent<BoxCollider2D>().size.x * scale.x;
        maxWidth = transform.GetComponent<BoxCollider2D>().size.x;
        leftPos = startingPosition.x - (maxWidth / 2) + (cardWidth / 2);
        margin = cardWidth + 0.2f;

        if (margin * cardStack.Size > maxWidth)
        {
            margin = (maxWidth - cardWidth) / (cardStack.Size - 1);
        }

        if (!cardStack.isGameStack)
        {
            position = new Vector3(leftPos + margin * offset, startingPosition.y, startingPosition.z - 0.0001f * offset - 0.0001f);
        }

        // face down = 180f, face up = 0f (for my card prefab)
        angle.y = cardAdd.isFacedUp ? 0f : 180f;

        // move the cards to their respective position
        // instant position and rotation, no "moving animation"
        //cardCopyAdd.transform.position = position;
        //cardCopyAdd.transform.rotation = Quaternion.Euler(angle);

        // "moving and rotation animation"
        cardCopyAdd.transform.position = Vector3.Lerp(cardCopyAdd.transform.position, position, (Time.time - startTime) / 1f);
        cardCopyAdd.transform.rotation = Quaternion.Slerp(Quaternion.Euler(cardCopyAdd.transform.rotation.eulerAngles), Quaternion.Euler(angle), (Time.time - startTime) / 1f);

        // scale card
        cardCopyAdd.transform.localScale = Vector3.Lerp(cardCopyAdd.transform.localScale, scale, (Time.time - startTime) / 1f);

        cardAdd.cardPosition = position;
        cardAdd.cardRotation = angle;
        cardAdd.scaleValue = scale;

        // group them up
        // make them as the children to the game object with this script attached
        cardCopyAdd.transform.SetParent(transform, true);
        cardCopyAdd.layer = transform.gameObject.layer;
        foreach(Transform child in cardCopyAdd.transform)
        {
            child.gameObject.layer = cardCopyAdd.layer;
        }
    }
}
