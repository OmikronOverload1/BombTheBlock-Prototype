using UnityEngine;

public class MainMenuController : MonoBehaviour
{

    public void Options()
    {

    }

    public void Desktop()
    {
        Application.Quit();
        Debug.Log("Game is quitting...");
    }
}