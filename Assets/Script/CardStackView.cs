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

    private Vector3 startingPos;
    private float startTime;

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
        startingPos = transform.position;
        startTime = Time.time;

        ShowCards(false);
    }
    void OnEnable()
    {
        //cardStack.cardRemoved += CardStack_cardRemoved;
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

    // Update is called once per frame
    void Update()
    {
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
            ArrangeCard(card, cardCopy, startingPos, offset);
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
                }
                else
                {
                    cardCopy = Instantiate(cardPrefab);
                }

                card.isFacedUp = cardStack.isGameStack ? false : true;
                if(cardStack.transform.GetComponent<Player>() != null)
                {
                    card.isFacedUp = cardStack.transform.GetComponent<Player>().isHuman ? true : false;
                }

                cardCopy.AddComponent<CardModel>();
                cardCopy.GetComponent<CardModel>().id = card.id;
                cardCopy.GetComponent<CardModel>().rank = card.rank;
                cardCopy.GetComponent<CardModel>().suit = card.suit;
                cardCopy.GetComponent<CardModel>().isFacedUp = card.isFacedUp;

                GameObject cardFaceTexture = cardCopy.transform.Find("CardFace").gameObject;
                cardFaceTexture.GetComponent<SpriteRenderer>().sprite = cardTextures[card.id];

                ArrangeCard(card, cardCopy, startingPos, offset);
                fetchedCard.Add(card.id, cardCopy);
            }
        }
    }

    void ArrangeCard(Card cardAdd, GameObject cardCopyAdd, Vector3 startingPosition, int offset)
    {
        Vector3 position = new Vector3(startingPosition.x + 0.005f * offset, startingPosition.y, startingPosition.z - 0.0001f * offset);
        Vector3 angle = transform.rotation.eulerAngles;

        //Vector3 localScale = cardPrefab.transform.localScale * 1.25f;
        //float localScaleY = transform.localScale.y;

        float cardWidth = cardPrefab.GetComponent<BoxCollider2D>().size.x;
        float maxWidth = transform.GetComponent<MeshRenderer>().bounds.size.x;
        float leftPos = startingPosition.x - (maxWidth / 2);
        float margin = cardWidth + 0.2f;

        if(margin * (cardStack.Size - 1) > maxWidth)
        {
            margin = maxWidth / (cardStack.Size - 1);
        }

        if (!cardStack.isGameStack)
        {
            position = new Vector3(leftPos + margin * offset, startingPosition.y, startingPosition.z - 0.0001f * offset);
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
        //cardCopyAdd.transform.localScale = Vector3.Lerp(cardCopyAdd.transform.localScale, localScale, (Time.time - startTime) / 1f);

        cardAdd.cardPosition = position;
        cardAdd.cardRotation = angle;

        // group them up
        // make them as the children to the game object with this script attached
        cardCopyAdd.transform.SetParent(transform, true);
    }
}
