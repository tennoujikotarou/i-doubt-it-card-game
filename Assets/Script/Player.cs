using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string PlayerName { get; set; }
    public CardStack playerStack { get; set; }

    public bool isPlayerTurn { get; set; }

    public bool canCallDoubt { get; set; }

    public event DoubtCallEventHandler callDoubt;

    void Start()
    {
        string playerButton = PlayerName + "ButtonDoubt";
        Button btnDoubt = GameObject.Find(playerButton).GetComponent<Button>();
        btnDoubt.onClick.AddListener(delegate { Doubt(); } );
        canCallDoubt = false;
    }

    public void Doubt()
    {
        if (callDoubt != null && canCallDoubt)
        {
            callDoubt(this, new DoubtCallEventHandlerArgs(true));
        }
    }

    void FixedUpdate()
    {
        string playerPanel = PlayerName + "Panel";
        Image activeBg = GameObject.Find(playerPanel).GetComponent<Image>();
        if(isPlayerTurn)
        {
            activeBg.CrossFadeAlpha(1.5f, 1f, false);
        }
        else
        {
            activeBg.CrossFadeAlpha(0.2f, 1f, false);
        }
    }
}
