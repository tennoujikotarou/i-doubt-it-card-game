using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    private CanvasGroup canvasGroup;
    private Animator animator;

    void Awake ()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            canvasGroup.blocksRaycasts = canvasGroup.interactable = false;
        }
        else
        {
            canvasGroup.blocksRaycasts = canvasGroup.interactable = true;
        }
	}

    public bool IsOpen
    {
        get { return animator.GetBool("IsOpen"); }
        set { animator.SetBool("IsOpen", value); }
    }
}
