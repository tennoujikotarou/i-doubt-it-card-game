using System.Collections.Generic;
using UnityEngine;

public class CardStack : MonoBehaviour
{
    public static string[] suits = { "Spade", "Club", "Diamond", "Heart" };
    public static string[] ranks = { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };
    private List<Card> cards;
    public bool isGameStack;

    void Awake()
    {
        cards = new List<Card>();
        if (isGameStack)
        {
            NewStack();
        }
    }

    public event CardRemovedEventHandler cardRemoved;

    public bool HasCard
    {
        get { return cards != null && cards.Count > 0; }
    }

    public IEnumerable<Card> GetCards()
    {
        foreach (Card card in cards)
        {
            yield return card;
        }
    }

    public int Size
    {
        get { return cards == null ? 0 : cards.Count; }
    }

    public Card RemoveAt(int position)
    {
        if (HasCard)
        {
            Card result = cards[position];

            if(cardRemoved != null)
            {
                cardRemoved(this, new CardRemovedEventArgs(result.id));
            }

            cards.RemoveAt(position);
            return result;
        }
        return null;
    }

    public Card Remove(Card card)
    {
        if (HasCard)
        {
            int position = -1;
            for (int i = 0; i < cards.Count; i++) {
                if(card.id == cards[i].id)
                {
                    position = i;
                    break;
                }
            }
            Card result = cards[position];

            if (cardRemoved != null)
            {
                cardRemoved(this, new CardRemovedEventArgs(result.id));
            }

            cards.RemoveAt(position);
            return result;
        }
        return null;
    }

    public void Add(Card value)
    {
        if (value != null)
        {
            cards.Add(value);
        }
    }

    public void NewStack()
    {
        if (cards == null)
        {
            cards = new List<Card>();
        }
        else
        {
            cards.Clear();
        }

        for (int i = 0; i < 52; i++)
        {

            Card card = new Card();

            card.id = i;
            card.rank = (i % 13) + 1;
            card.suit = suits[i / 13];

            cards.Add(card);
        }

        Shuffle();
    }

    public void Shuffle()
    {
        if (cards != null)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                int index = Random.Range(0, cards.Count);

                Card cardTemp = cards[index];
                cards[index] = cards[i];
                cards[i] = cardTemp;
            }
        }
    }
}
