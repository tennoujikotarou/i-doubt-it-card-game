using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Sprite[] characterSprite;
    public event DoubtCallEventHandler callDoubt;

    public GameObject textBubble;
    public Text message;
    public Image avatar;

    public string PlayerName { get; set; }
    public string RealName { get; set; }
    public int playerIndex { get; set; }

    public bool isHuman { get; set; }
    public bool localPlayer { get; set; }

    public CardStack playerStack { get; set; }

    public bool isPlayerTurn { get; set; }
    public bool canCallDoubt { get; set; }

    public int playerTurn { get; set; }
    public int endGameRank { get; private set; }

    public bool isPlayDiceRolled { get; set; }
    public bool isDoubtDiceRolled { get; set; }

    private int cardStackSize
    {
        get { return GameObject.Find("Dealer").GetComponent<CardStack>().Size; }
    }

    private int honestChance
    {
        get { return (playerStack.Size <= 6 || cardStackSize >= 4) ? 90 : 30; }
    }

    private int DoubtChance
    {
        get
        {
            int currentRank = GameManager.currentRank + 1;
            int cardCount = 0;
            for (int i = 0; i < playerStack.Size; i++)
            {
                if (playerStack.transform.GetChild(i).GetComponent<CardModel>().rank == currentRank)
                {
                    cardCount++;
                }
            }

            switch (cardCount)
            {
                case 0:
                    return 10;
                case 1:
                    return 20;
                case 2:
                    return 30;
                case 3:
                    return 70;
                case 4:
                    return 100;
                default:
                    return 1;
            }
        }
    }

    void Awake()
    {
        textBubble.SetActive(false);
    }

    void Start()
    {
        string playerButton = PlayerName + "ButtonDoubt";
        if (playerButton.Contains("Player1"))
        {
            Button btnDoubt = GameObject.Find(playerButton).GetComponent<Button>();
            btnDoubt.onClick.AddListener(delegate { Doubt(); });
        }
        canCallDoubt = false;
        isPlayDiceRolled = false;
        isDoubtDiceRolled = false;

        GameObject.Find(PlayerName + "Name").GetComponent<Text>().text = RealName;

        if (PlayerName.Contains("Player1")) { characterSprite = Resources.LoadAll<Sprite>("neptune_sprite"); }
        if (PlayerName.Contains("Player2")) { characterSprite = Resources.LoadAll<Sprite>("noire_sprite"); }
        if (PlayerName.Contains("Player3")) { characterSprite = Resources.LoadAll<Sprite>("blanc_sprite"); }
        if (PlayerName.Contains("Player4")) { characterSprite = Resources.LoadAll<Sprite>("vert_sprite"); }
    }

    public void Doubt()
    {
        if (callDoubt != null && canCallDoubt)
        {
            //string playerAvatar = PlayerName + "Avatar";
            //Image characterAvatar = GameObject.Find(playerAvatar).GetComponent<Image>();
            //characterAvatar.sprite = neptuneSprite[8];
            callDoubt(this, new DoubtCallEventHandlerArgs(true));
        }
    }

    void Update()
    {
        GameObject.Find(PlayerName + "Cards").GetComponent<Text>().text = "On Hand: " + playerStack.Size;
        if (!isHuman)
        {
            if (isPlayerTurn && playerStack.Size != 0 && !isPlayDiceRolled)
            {
                AutoPlay();
                isPlayDiceRolled = true;
            }

            if (canCallDoubt && !isDoubtDiceRolled && cardStackSize >= 3)
            {
                AutoDoubt();
                isDoubtDiceRolled = true;
            }
        }
    }

    void FixedUpdate()
    {
        //string playerPanel = PlayerName + "Panel";
        //Image activeBg = GameObject.Find(playerPanel).GetComponent<Image>();
        //if (isPlayerTurn)
        //{
        //    activeBg.CrossFadeAlpha(1.5f, 1f, false);
        //}
        //else
        //{
        //    activeBg.CrossFadeAlpha(0.2f, 1f, false);
        //}
    }

    public void CalculateEndGameRank(int currentRank, int numberOfPlayer)
    {
        Debug.Log("=========================");
        Debug.Log("PlayerStack size: " + playerStack.Size);
        Debug.Log("numberOfPlayer: " + numberOfPlayer);
        Debug.Log("Current rank: " + currentRank);
        Debug.Log(PlayerName + " turn: " + playerTurn);
        endGameRank = ((((playerStack.Size - 1) * numberOfPlayer) % 13) + currentRank + playerTurn);
        endGameRank = endGameRank < 14 ? endGameRank : (endGameRank % 13 == 0 ? 13 : endGameRank - 13 * (endGameRank / 13));
        Debug.Log(PlayerName + " end rank: " + endGameRank);
        Debug.Log("=========================");
    }

    private void AutoPlay()
    {
        int randomPlay = 0;
        int diceRoll = Random.Range(0, 100);
        int currentRank = GameManager.currentRank + 1;

        bool canHonest = false;
        int matchCardCount = 0;
        for (int i = 0; i < playerStack.transform.childCount; i++)
        {
            if (playerStack.transform.GetChild(i).GetComponent<CardModel>().rank == currentRank)
            {
                canHonest = true;
                randomPlay = i;
                matchCardCount++;
            }
        }

        // Still have tons of cards in hand, should not play the card for now...
        if (playerStack.transform.GetChild(randomPlay).GetComponent<CardModel>().rank == endGameRank
            && (playerStack.Size >= 7 || playerStack.Size <= 4)
            && matchCardCount == 1)
        {
            canHonest = false;
        }
        int loopBreak = 0;
        //Debug.Log("Dice: " + diceRoll + " - chance: " + honestChance + " - can honest: " + canHonest);
        if (diceRoll < honestChance && canHonest)
        {
            //Debug.Log("==========honest==========");
            // find the card that matches with the current rank
            while (playerStack.transform.GetChild(randomPlay).GetComponent<CardModel>().rank != currentRank)
            {
                randomPlay = Random.Range(0, playerStack.Size);
                loopBreak++;
                if (loopBreak > 10000)
                {
                    break;
                }
            }
        }
        else
        {
            // find the card that doesnt match with the current rank
            while (playerStack.transform.GetChild(randomPlay).GetComponent<CardModel>().rank == currentRank
                || playerStack.transform.GetChild(randomPlay).GetComponent<CardModel>().rank == endGameRank)
            {
                randomPlay = Random.Range(0, playerStack.Size);
                loopBreak++;
                if (loopBreak > 10000)
                {
                    break;
                }
            }
        }
        //Debug.Log("====Card play: " + playerStack.transform.GetChild(randomPlay).GetComponent<CardModel>().rank + "====");
        playerStack.transform.GetChild(randomPlay).GetComponent<CardModel>().Invoke("PlayCard", 1f);
    }

    private void AutoDoubt()
    {
        int diceRoll = Random.Range(0, 100);
        //Debug.Log("Doubt chance: " + DoubtChance);
        //Debug.Log("roll: " + diceRoll);
        if (diceRoll < DoubtChance)
        {
            Invoke("Doubt", 0.5f);
        }
    }

    public string DoubtSuccessMsg()
    {
        if (PlayerName.Contains("Player1")) { avatar.sprite = characterSprite[4]; }
        if (PlayerName.Contains("Player2")) { avatar.sprite = characterSprite[7]; }
        if (PlayerName.Contains("Player3")) { avatar.sprite = characterSprite[5]; }
        if (PlayerName.Contains("Player4")) { avatar.sprite = characterSprite[7]; }
        switch (Random.Range(0, 5))
        {
            case 0: return "Mwahaha~~!!";
            case 1: return "So obvious.";
            case 2: return "You can't fool me!";
            case 3: return "Phew, close call.";
            case 4: return "Dundundun~~";
            default: return "Yay, I got it right!";
        }
    }
    public string DoubtFailMsg()
    {
        if (PlayerName.Contains("Player1")) { avatar.sprite = characterSprite[Random.Range(9, 13)]; }
        if (PlayerName.Contains("Player2")) { avatar.sprite = characterSprite[Random.Range(10, 13)]; }
        if (PlayerName.Contains("Player3")) { avatar.sprite = characterSprite[Random.Range(9, 11)]; }
        if (PlayerName.Contains("Player4")) { avatar.sprite = characterSprite[Random.Range(9, 12)]; }
        switch (Random.Range(0, 5))
        {
            case 0: return "Noooo!!!";
            case 1: return "L-Lucky shot!";
            case 2: return "Such... trick...";
            case 3: return "Ouch!";
            case 4: return "Goodbye, world...";
            default: return "...";
        }
    }

    public string DoubtDodgeSuccessMsg()
    {
        if (PlayerName.Contains("Player1")) { avatar.sprite = characterSprite[4]; }
        if (PlayerName.Contains("Player2")) { avatar.sprite = characterSprite[7]; }
        if (PlayerName.Contains("Player3")) { avatar.sprite = characterSprite[5]; }
        if (PlayerName.Contains("Player4")) { avatar.sprite = characterSprite[7]; }
        switch (Random.Range(0, 5))
        {
            case 0: return "Huehuehue~~";
            case 1: return "I got you!";
            case 2: return "Oh no you don't.";
            case 3: return "Dadada~~~";
            case 4: return "Here's your present~!!";
            default: return "Take this!";
        }
    }

    public string DoubtDodgeFailMsg()
    {
        if (PlayerName.Contains("Player1")) { avatar.sprite = characterSprite[Random.Range(9, 13)]; }
        if (PlayerName.Contains("Player2")) { avatar.sprite = characterSprite[Random.Range(10, 13)]; }
        if (PlayerName.Contains("Player3")) { avatar.sprite = characterSprite[Random.Range(9, 11)]; }
        if (PlayerName.Contains("Player4")) { avatar.sprite = characterSprite[Random.Range(9, 12)]; }
        switch (Random.Range(0, 5))
        {
            case 0: return "NOOOO!!!";
            case 1: return "Ugh...";
            case 2: return "Ooh... nooo....";
            case 3: return "Waaaah~~!!";
            case 4: return "RIP, me.";
            default: return "...";
        }
    }

    public string DoubtResponseMsg()
    {
        if (PlayerName.Contains("Player1")) { avatar.sprite = characterSprite[8]; }
        if (PlayerName.Contains("Player2")) { avatar.sprite = characterSprite[9]; }
        if (PlayerName.Contains("Player3")) { avatar.sprite = characterSprite[8]; }
        if (PlayerName.Contains("Player4")) { avatar.sprite = characterSprite[8]; }
        switch (Random.Range(0, 5))
        {
            case 0: return "!!!";
            case 1: return "Gah!";
            case 2: return "Erk!";
            case 3: return "Nyuu~?";
            case 4: return "Can't be helped...";
            default: return "...";
        }
    }

    public string WinFailMsg()
    {
        if (PlayerName.Contains("Player1")) { avatar.sprite = characterSprite[16]; }
        if (PlayerName.Contains("Player2")) { avatar.sprite = characterSprite[13]; }
        if (PlayerName.Contains("Player3")) { avatar.sprite = characterSprite[15]; }
        if (PlayerName.Contains("Player4")) { avatar.sprite = characterSprite[14]; }
        switch (Random.Range(0, 5))
        {
            case 0: return "Why can't I win...";
            case 1: return "Nuuuu!";
            case 2: return "Waaaa...";
            case 3: return "Nyauu~~!!!";
            case 4: return "N-Next time!";
            default: return "...";
        }
    }

    public string WinMsg()
    {
        if (PlayerName.Contains("Player1")) { avatar.sprite = characterSprite[3]; }
        if (PlayerName.Contains("Player2")) { avatar.sprite = characterSprite[4]; }
        if (PlayerName.Contains("Player3")) { avatar.sprite = characterSprite[4]; }
        if (PlayerName.Contains("Player4")) { avatar.sprite = characterSprite[4]; }
        switch (Random.Range(0, 1))
        {
            case 0: return "YAY!";
            default: return "...";
        }
    }

    public string DoubtMsg()
    {
        if (PlayerName.Contains("Player1")) { avatar.sprite = characterSprite[1]; }
        if (PlayerName.Contains("Player2")) { avatar.sprite = characterSprite[2]; }
        if (PlayerName.Contains("Player3")) { avatar.sprite = characterSprite[1]; }
        if (PlayerName.Contains("Player4")) { avatar.sprite = characterSprite[1]; }
        switch (Random.Range(0, 1))
        {
            case 0: return "Doubt!";
            default: return "...";
        }
    }
}