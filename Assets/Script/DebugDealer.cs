using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CardStack))]

public class DebugDealer : MonoBehaviour
{
    private CardStack dealer;
    private List<CardStack> player;

    void Awake ()
    {
        player = new List<CardStack>();
        dealer = GameObject.Find("Dealer").GetComponent<CardStack>();
        player[0] = GameObject.Find("Player1").GetComponent<CardStack>();
        player[1] = GameObject.Find("Player2").GetComponent<CardStack>();
        player[2] = GameObject.Find("Player3").GetComponent<CardStack>();
        player[3] = GameObject.Find("Player4").GetComponent<CardStack>();
    }

    void OnGUI()
    {
        if(GUI.Button(new Rect(10, 10, 256, 38), "Deal Card!"))
        {
            player[0].Add(dealer.RemoveAt(dealer.Size - 1));
            player[1].Add(dealer.RemoveAt(dealer.Size - 1));
            player[2].Add(dealer.RemoveAt(dealer.Size - 1));
            player[3].Add(dealer.RemoveAt(dealer.Size - 1));
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
