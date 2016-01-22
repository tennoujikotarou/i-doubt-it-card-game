using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private CardStack dealer;
    private List<Player> players;
    private IEnumerator changePlayerTurn;
    private int numberOfPlayer;

    private int numberOfTurn;
    private Text numberOfTurnText;
    public static int currentRank;
    private Text currentRankText;
    private int currentPlayer;
    private Text currentPlayerText;

    void Awake()
    {
        numberOfPlayer = 4;
        currentPlayer = 0;
        currentRank = 12;
        currentRankText = GameObject.Find("CurrentRank").GetComponent<Text>();
        currentPlayerText = GameObject.Find("CurrentPlayer").GetComponent<Text>();
        numberOfTurnText = GameObject.Find("Turn").GetComponent<Text>();

        players = new List<Player>();
        dealer = GameObject.Find("Dealer").GetComponent<CardStack>();
        for (int i = 0; i < numberOfPlayer; i++)
        {
            string playerObjectString = "Player" + (i + 1).ToString();

            Player player = GameObject.Find(playerObjectString).GetComponent<Player>();
            player.playerStack = GameObject.Find(playerObjectString).GetComponent<CardStack>();
            
            player.isHuman = i == 0 ? true : false;
            player.isPlayerTurn = false;
            player.playerTurn = i;

            player.PlayerName = playerObjectString;
            player.playerStack.cardRemoved += CardStack_cardRemoved;
            player.callDoubt += Player_callDoubt;

            if (i == 0) { player.RealName = "Neptune"; }
            if (i == 1) { player.RealName = "Noire"; }
            if (i == 2) { player.RealName = "Blanc"; }
            if (i == 3) { player.RealName = "Vert"; }
            players.Add(player);
        }
    }

    private void CardStack_cardRemoved(object sender, CardRemovedEventArgs e)
    {
        changePlayerTurn = ChangePlayerTurn();
        // allow other players to call doubt
        foreach (Player player in players)
        {
            if (!player.isPlayerTurn)
            {
                player.canCallDoubt = true;
            }
        }

        //Debug.Log(sender + " played a card");
        if ((((CardStack)sender).Size - 1) == 0)
        {
            // last card check
            StartCoroutine(DoubtCallEvent(null));
        }
        else
        {
            StartCoroutine(changePlayerTurn);
        }
    }

    private void Player_callDoubt(object sender, DoubtCallEventHandlerArgs e)
    {
        if (e.CallDoubt)
        {
            //Debug.Log(((Player)sender).PlayerName + " call doubt!");

            StopCoroutine(changePlayerTurn);
            StartCoroutine(DoubtCallEvent((Player)sender));
        }
    }

    // Use this for initialization
    void Start()
    {
        players[currentPlayer].isPlayerTurn = true;

        numberOfTurn = 0;
        currentRankText.text = "Current Rank: ";
        currentPlayerText.text = "Current Player: " + players[currentPlayer].RealName;
        while (dealer.Size != 0)
        {
            foreach (Player player in players)
            {
                player.playerStack.Add(dealer.RemoveAt(dealer.Size - 1));
            }
        }

        foreach (Player player in players)
        {
            player.CalculateEndGameRank(currentRank + 1, numberOfPlayer);
        }

        changePlayerTurn = ChangePlayerTurn();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDisable()
    {
        if(players != null)
        {
            foreach(Player player in players)
            {
                player.playerStack.cardRemoved -= CardStack_cardRemoved;
                player.callDoubt -= Player_callDoubt;
            }
        }
    }

    IEnumerator ChangePlayerTurn()
    {
        players[currentPlayer].isPlayerTurn = false;
        currentRank = (currentRank + 1) < 13 ? (currentRank + 1) : 0;
        currentRankText.text = "Current Rank: " + CardStack.ranks[currentRank];

        // wait for 5 seconds before changing player turn
        yield return new WaitForSeconds(1f);

        //players[currentPlayer].isPlayDiceRolled = false;
        ChangeTurn();
    }
    IEnumerator DoubtCallEvent(Player challenger)
    {
        foreach (Player player in players)
        {
            player.canCallDoubt = false;
        }

        if (challenger == null) // last card check
        {
            players[currentPlayer].isPlayerTurn = false;
            currentRank = (currentRank + 1) < 13 ? (currentRank + 1) : 0;
            currentRankText.text = "Current Rank: " + CardStack.ranks[currentRank];

            yield return new WaitForSeconds(2f);

            CardModel card = dealer.transform.GetChild(dealer.transform.childCount - 1).GetComponent<CardModel>();
            card.flipCard = true;

            yield return new WaitForSeconds(2f);

            //Debug.Log("Played card rank: " + card.rank);
            //Debug.Log("Current rank: " + (currentRank + 1));

            if (card.rank != (currentRank + 1))
            {
                Debug.Log("Die, " + players[currentPlayer].RealName + "!!!");
                PlayerGetCardStack(players[currentPlayer]);
                
                ChangeTurn();
            }
            else
            {
                //Debug.Log(players[currentPlayer].RealName + " won!");
                currentRankText.text = players[currentPlayer].RealName + " won!";
            }
        }
        else // normal doubting event
        {
            CardModel card = dealer.transform.GetChild(dealer.transform.childCount - 1).GetComponent<CardModel>();
            card.flipCard = true;

            //Debug.Log("Played card rank: " + card.rank);
            //Debug.Log("Current rank: " + (currentRank + 1));

            yield return new WaitForSeconds(2f);

            if (card.rank != (currentRank + 1))
            {
                Debug.Log("Die, " + players[currentPlayer].RealName + "!!!");
                PlayerGetCardStack(players[currentPlayer]);
            }
            else
            {
                Debug.Log(challenger.RealName + " is dying...");
                PlayerGetCardStack(challenger);
            }

            ChangeTurn();
        }
    }

    private void PlayerGetCardStack(Player player)
    {
        while (dealer.Size != 0)
        {
            player.playerStack.Add(dealer.RemoveAt(dealer.Size - 1));
        }
        int newTurn = (currentPlayer + 1) < 4 ? (currentPlayer + 1) : 0;
        player.playerTurn = (player.playerTurn - newTurn) >= 0 ? (player.playerTurn - newTurn) : (player.playerTurn + (4 - newTurn));
        player.CalculateEndGameRank(currentRank + 1, numberOfPlayer);
    }

    private void ChangeTurn()
    {
        currentPlayer = (currentPlayer + 1) < 4 ? (currentPlayer + 1) : 0;
        players[currentPlayer].isPlayerTurn = true;

        foreach (Player player in players)
        {
            player.canCallDoubt = false;
            player.isDoubtDiceRolled = false;
            player.isPlayDiceRolled = false;
        }
        currentPlayerText.text = "Current Player: " + players[currentPlayer].RealName;
        numberOfTurnText.text = "Turn: " + (++numberOfTurn);
    }
}