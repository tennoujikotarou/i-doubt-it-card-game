using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CardStack))]

public class CardStackView : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite[] cardTextures;
    private CardStack cardStack;

    private Dictionary<int, GameObject> fetchedCard;
    private int stackLastCount;

    private Vector3 startingPos;

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

        startingPos = transform.position;

        ShowCards();
    }

    private void CardStack_cardRemoved(object sender, CardRemovedEventArgs e)
    {
        if(fetchedCard.ContainsKey(e.CardIndex))
        {
            Destroy(fetchedCard[e.CardIndex]);
            fetchedCard.Remove(e.CardIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(stackLastCount != cardStack.Size)
        {
            stackLastCount = cardStack.Size;
            ShowCards();
        }
    }

    void FixedUpdate()
    {
        int offset = -1;
        foreach (Card card in cardStack.GetCards())
        {
            offset++;

            Vector3 move = new Vector3(startingPos.x + 0.005f * offset, startingPos.y, startingPos.z - 0.0001f * offset);
            Vector3 temp = transform.rotation.eulerAngles;

            if (!cardStack.isGameStack)
            {
                move = new Vector3(startingPos.x + 0.5f * offset, startingPos.y, startingPos.z - 0.0001f * offset);
                card.isFacedUp = true;
            }
            else
            {
                card.isFacedUp = false;
            }

            // face down = 180f, face up = 0f
            if (card.isFacedUp)
            {
                temp.y = 0f;
            }
            else
            {
                temp.y = 180f;
            }
            GameObject cardCopy = transform.GetChild(offset).gameObject;
            // move the cards to their respective position
            cardCopy.transform.position = move;
            cardCopy.transform.rotation = Quaternion.Euler(temp);
        }
    }

    private void ShowCards()
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

                Vector3 move = new Vector3(startingPos.x + 0.005f * offset, startingPos.y, startingPos.z - 0.0001f * offset);
                Vector3 temp = transform.rotation.eulerAngles;

                if (!cardStack.isGameStack) {
                    card.isFacedUp = true;
                    move = new Vector3(startingPos.x + 0.5f * offset, startingPos.y, startingPos.z - 0.0001f * offset);
                }
                else
                {
                    card.isFacedUp = false;
                }

                // face down = 180f, face up = 0f
                if (card.isFacedUp)
                {
                    temp.y = 0f;
                }
                else
                {
                    temp.y = 180f;
                }

                GameObject cardCopy = (GameObject)Instantiate(cardPrefab);

                // move the cards to their respective position
                cardCopy.transform.position = move;
                cardCopy.transform.rotation = Quaternion.Euler(temp);

                // group them up
                // make them as the children to the game object with this script attached
                cardCopy.transform.parent = transform;

                cardCopy.AddComponent<CardModel>();
                cardCopy.GetComponent<CardModel>().id = card.id;
                cardCopy.GetComponent<CardModel>().rank = card.rank;
                cardCopy.GetComponent<CardModel>().suit = card.suit;

                GameObject cardFaceTexture = cardCopy.transform.Find("CardFace").gameObject;
                cardFaceTexture.GetComponent<SpriteRenderer>().sprite = cardTextures[card.id];

                fetchedCard.Add(card.id, cardCopy);
            }
        }
    }
}
