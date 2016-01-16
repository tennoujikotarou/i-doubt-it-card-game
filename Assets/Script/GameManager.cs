using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private CardStack dealer;
    private List<Player> players;
    private int currentPlayer;
    private int currentRank;
    private Text currentRankText;
    private IEnumerator changePlayerTurn;

    void Awake()
    {
        players = new List<Player>();
        dealer = GameObject.Find("Dealer").GetComponent<CardStack>();
        for (int i = 0; i < 4; i++)
        {
            string playerObjectString = "Player" + (i+1).ToString();
            
            Player player = GameObject.Find(playerObjectString).AddComponent<Player>();
            player.playerStack = GameObject.Find(playerObjectString).GetComponent<CardStack>();

            player.isPlayerTurn = false;
            player.PlayerName = playerObjectString;
            player.playerStack.cardRemoved += CardStack_cardRemoved;
            player.callDoubt += Player_callDoubt;

            players.Add(player);
        }
        currentPlayer = 0;
        currentRank = 12;
        currentRankText = GameObject.Find("PlayingRank").GetComponent<Text>();
    }

    private void Player_callDoubt(object sender, DoubtCallEventHandlerArgs e)
    {
        if(e.CallDoubt)
        {
            Debug.Log(((Player)sender).PlayerName + "call doubt!");

            StopCoroutine(changePlayerTurn);

            StartCoroutine(DoubtCallEvent((Player)sender));
        }
    }

    private void CardStack_cardRemoved(object sender, CardRemovedEventArgs e)
    {
        changePlayerTurn = ChangePlayerTurn();
        // allow other players to call doubt
        foreach (Player player in players)
        {
            if(!player.isPlayerTurn)
            {
                player.canCallDoubt = true;
            }
        }
        StartCoroutine(changePlayerTurn);
    }

    void OnGUI()
    {
        //if (GUI.Button(new Rect(10, 10, 256, 38), "Deal Card!"))
        //{
        //    foreach (Player player in players)
        //    {
        //        player.playerStack.Add(dealer.RemoveAt(dealer.Size - 1));
        //    }
        //}
    }

    // Use this for initialization
    void Start()
    {
        players[currentPlayer].isPlayerTurn = true;
        currentRankText.text = "Current Rank: \n";
        while (dealer.Size != 0)
        {
            foreach (Player player in players)
            {
                player.playerStack.Add(dealer.RemoveAt(dealer.Size - 1));
            }
        }

        changePlayerTurn = ChangePlayerTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ChangePlayerTurn()
    {
        players[currentPlayer].isPlayerTurn = false;
        currentRank = (currentRank + 1) < 13 ? (currentRank + 1) : 0;
        currentRankText.text = "Current Rank: \n" + CardStack.ranks[currentRank];

        // wait for 5 seconds before changing player turn
        yield return new WaitForSeconds(5f);

        currentPlayer = (currentPlayer + 1) < 4 ? (currentPlayer + 1) : 0;
        players[currentPlayer].isPlayerTurn = true;

        foreach (Player player in players)
        {
            player.canCallDoubt = false;
        }
    }
    IEnumerator DoubtCallEvent(Player challenger)
    {
        foreach (Player player in players)
        {
            player.canCallDoubt = false;
        }
        CardModel card = dealer.transform.GetChild(dealer.transform.childCount - 1).GetComponent<CardModel>();
        card.flipCard = true;

        Debug.Log("Played card rank: " + card.rank);
        Debug.Log("Current rank: " + (currentRank + 1));

        yield return new WaitForSeconds(2f);

        if (card.rank != (currentRank + 1))
        {
            Debug.Log("Die!");
            while (dealer.Size != 0)
            {
                players[currentPlayer].playerStack.Add(dealer.RemoveAt(dealer.Size - 1));
            }
        }
        else
        {
            Debug.Log("I'm dead...");
            while (dealer.Size != 0)
            {
                challenger.playerStack.Add(dealer.RemoveAt(dealer.Size - 1));
            }
        }

        currentPlayer = (currentPlayer + 1) < 4 ? (currentPlayer + 1) : 0;
        players[currentPlayer].isPlayerTurn = true;

        foreach (Player player in players)
        {
            player.canCallDoubt = false;
        }
    }
}