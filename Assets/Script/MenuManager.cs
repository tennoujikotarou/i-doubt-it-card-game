using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{

    public Menu currentMenu;

    // Use this for initialization
    void Start()
    {
        ShowMenu(currentMenu);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowMenu(Menu menu)
    {
        if (currentMenu != null)
        {
            currentMenu.IsOpen = false;
        }

        currentMenu = menu;
        currentMenu.IsOpen = true;
    }

    public void LoadPlayGame()
    {
        //Application.LoadLevel(1);
        Application.LoadLevel("Game Scene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
