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
    public static int currentRank;
    public static int currentPlayer;
    public Text numberOfTurnText;
    public Text currentRankText;
    public Text currentPlayerText;

    public Text specialMessage;
    public Text guideMsg;

    public GameObject pauseMenu;

    void Awake()
    {
        numberOfPlayer = 4;
        currentPlayer = 0;
        currentRank = 0;

        pauseMenu.SetActive(false);
        specialMessage.gameObject.SetActive(false);
        guideMsg.gameObject.SetActive(false);

        players = new List<Player>();
        dealer = GameObject.Find("Dealer").GetComponent<CardStack>();
        for (int i = 0; i < numberOfPlayer; i++)
        {
            string playerObjectString = "Player" + (i + 1).ToString();

            Player player = GameObject.Find(playerObjectString).GetComponent<Player>();
            player.playerStack = player.GetComponent<CardStack>();
            
            player.isHuman = i == 0 ? true : false;
            player.localPlayer = i == 0 ? true : false;
            player.isPlayerTurn = false;
            player.playerTurn = player.playerIndex = i;

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
        guideMsg.gameObject.SetActive(false);
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
            Debug.Log(((Player)sender).PlayerName + " call doubt!");

            StopCoroutine(changePlayerTurn);
            StartCoroutine(DoubtCallEvent((Player)sender));
        }
    }

    // Use this for initialization
    void Start()
    {
        numberOfTurn = 0;
        numberOfTurnText.text = "Turn: " + (++numberOfTurn);
        currentRankText.text = "Current Rank: " + CardStack.ranks[currentRank];
        currentPlayerText.text = "Current Player: " + players[currentPlayer].RealName;

        changePlayerTurn = ChangePlayerTurn();
        StartCoroutine(DrawCards());
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnEnable()
    {
        //if (players != null)
        //{
        //    foreach (Player player in players)
        //    {
        //        player.playerStack.cardRemoved += CardStack_cardRemoved;
        //        player.callDoubt += Player_callDoubt;
        //    }
        //}
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

    IEnumerator DrawCards()
    {
        yield return new WaitForSeconds(2f);

        while (dealer.Size != 0)
        {
            foreach (Player player in players)
            {
                //player.playerStack.Add(dealer.RemoveAt(dealer.Size - 1));
                player.playerStack.Add(dealer.TransferCard(dealer.Size - 1, player.playerStack));
                yield return new WaitForSeconds(0.1f);
            }
        }

        foreach (Player player in players)
        {
            player.CalculateEndGameRank(currentRank + 1, numberOfPlayer);
        }

        yield return new WaitForSeconds(2f);

        players[0].playerStack.zoomCard = true;
        players[currentPlayer].isPlayerTurn = true;
        guideMsg.gameObject.SetActive(true);
    }

    IEnumerator ChangePlayerTurn()
    {
        if(currentPlayer == 0) { players[0].playerStack.zoomCard = true; }
        players[currentPlayer].isPlayerTurn = false;

        // wait for 5 seconds before changing player turn
        yield return new WaitForSeconds(1f);
        
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
            if (currentPlayer == 0) { players[0].playerStack.zoomCard = true; }
            players[currentPlayer].isPlayerTurn = false;
            //currentRank = (currentRank + 1) < 13 ? (currentRank + 1) : 0;
            //currentRankText.text = "Current Rank: " + CardStack.ranks[currentRank];

            yield return new WaitForSeconds(2f);

            CardModel card = dealer.transform.GetChild(dealer.transform.childCount - 1).GetComponent<CardModel>();
            card.flipCard = true;
            card.flipZoom = true;

            yield return new WaitForSeconds(2f);

            //Debug.Log("Played card rank: " + card.rank);
            //Debug.Log("Current rank: " + (currentRank + 1));

            if (card.rank != (currentRank + 1))
            {
                specialMessage.text = "Fake";
                specialMessage.color = new Color(218 / 255.0f, 0f, 0f);

                yield return new WaitForSeconds(0.5f);

                Debug.Log("Die, " + players[currentPlayer].RealName + "!!!");
                PlayerGetCardStack(players[currentPlayer]);

                // Messages from playerss
                players[currentPlayer].textBubble.SetActive(true);
                players[currentPlayer].message.text = players[currentPlayer].WinFailMsg();

                yield return new WaitForSeconds(1f);

                players[currentPlayer].textBubble.SetActive(false);
                ChangeTurn();
            }
            else
            {
                //Debug.Log(players[currentPlayer].RealName + " won!");
                //currentRankText.text = players[currentPlayer].RealName + " won!";

                specialMessage.gameObject.SetActive(true);
                specialMessage.text = players[currentPlayer].RealName + "\nwon!";
                specialMessage.color = new Color(218 / 255.0f, 0f, 0f);


                foreach (Player player in players)
                {
                    if (player.PlayerName.Contains("Player1")) { player.avatar.sprite = player.characterSprite[12]; }
                    if (player.PlayerName.Contains("Player2")) { player.avatar.sprite = player.characterSprite[15]; }
                    if (player.PlayerName.Contains("Player3")) { player.avatar.sprite = player.characterSprite[14]; }
                    if (player.PlayerName.Contains("Player4")) { player.avatar.sprite = player.characterSprite[13]; }
                }

                players[currentPlayer].textBubble.SetActive(true);
                players[currentPlayer].message.text = players[currentPlayer].WinMsg();
            }
        }
        else // normal doubting event
        {
            challenger.textBubble.SetActive(true);
            challenger.message.text = challenger.DoubtMsg();

            CardModel card = dealer.transform.GetChild(dealer.transform.childCount - 1).GetComponent<CardModel>();
            card.flipCard = true;
            card.flipZoom = true;

            //Debug.Log("Played card rank: " + card.rank);
            //Debug.Log("Current rank: " + (currentRank + 1));
            yield return new WaitForSeconds(0.5f);

            // Messages from players
            players[currentPlayer].textBubble.SetActive(true);
            players[currentPlayer].message.text = players[currentPlayer].DoubtResponseMsg();

            yield return new WaitForSeconds(1.5f);

            specialMessage.gameObject.SetActive(true);

            if (card.rank != (currentRank + 1))
            {
                specialMessage.text = "Doubt \nSuccess";
                specialMessage.color = new Color(0f, 128 / 255f, 0f);
                //specialMessage.color = new Color(218 / 255f, 0f, 0f);

                yield return new WaitForSeconds(0.5f);

                Debug.Log("Die, " + players[currentPlayer].RealName + "!!!");
                
                // Messages from players
                challenger.message.text = challenger.DoubtSuccessMsg();
                players[currentPlayer].message.text = players[currentPlayer].DoubtDodgeFailMsg();

                PlayerGetCardStack(players[currentPlayer]);
            }
            else
            {
                specialMessage.text = "Doubt \nFailed";
                specialMessage.color = new Color(218 / 255f, 0f, 0f);
                //specialMessage.color = new Color(0f, 128 / 255f, 0f);

                yield return new WaitForSeconds(0.5f);

                Debug.Log(challenger.RealName + " is dying...");

                // Messages from players
                challenger.message.text = challenger.DoubtFailMsg();
                players[currentPlayer].message.text = players[currentPlayer].DoubtDodgeSuccessMsg();

                PlayerGetCardStack(challenger);
            }
            yield return new WaitForSeconds(1f);

            specialMessage.gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);
            challenger.textBubble.SetActive(false);
            players[currentPlayer].textBubble.SetActive(false);

            ChangeTurn();
        }
    }

    private void PlayerGetCardStack(Player player)
    {
        while (dealer.Size != 0)
        {
            //player.playerStack.Add(dealer.RemoveAt(dealer.Size - 1));
            player.playerStack.Add(dealer.TransferCard(dealer.Size - 1, player.playerStack));
        }
        int newTurn = (currentPlayer + 1) < 4 ? (currentPlayer + 1) : 0;
        player.playerTurn = (player.playerIndex - newTurn) >= 0 ? (player.playerIndex - newTurn) : (player.playerIndex + (4 - newTurn));
        player.CalculateEndGameRank(currentRank + 2, numberOfPlayer);
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
            player.avatar.sprite = player.characterSprite[0];
        }
        currentPlayerText.text = "Current Player: " + players[currentPlayer].RealName;

        if(currentPlayer == 0) { 
            numberOfTurnText.text = "Turn: " + (++numberOfTurn);
        }

        currentRank = (currentRank + 1) < 13 ? (currentRank + 1) : 0;
        currentRankText.text = "Current Rank: " + CardStack.ranks[currentRank];
        
        if (currentPlayer == 0) {
            players[0].playerStack.zoomCard = true;
            guideMsg.gameObject.SetActive(true);
        }
        else
        {
            guideMsg.gameObject.SetActive(false);
        }
    }

    public void PauseGame()
    {
        if (Time.timeScale == 1.0f)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }
}