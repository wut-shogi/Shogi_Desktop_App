using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    // Start is called before the first frame update
    private Rect windowRect = new Rect((Screen.width - 200) / 2, (Screen.height - 300) / 2, 200, 300);
    // Only show it if needed.
    private bool show = false;

    void OnGUI()
    {
        if (show)
            windowRect = GUI.Window(0, windowRect, DialogWindow, "Game Over");
    }

    // This is the actual window.
    void DialogWindow(int windowID)
    {
        float y = 20;
        GUI.Label(new Rect(5, y, windowRect.width, 20), "Again?");

        if (GUI.Button(new Rect(5, y, windowRect.width - 10, 20), "Restart"))
        {
            Application.LoadLevel(0);
            show = false;
        }

        if (GUI.Button(new Rect(5, y, windowRect.width - 10, 20), "Exit"))
        {
            Application.Quit();
            show = false;
        }
    }

    // To open the dialogue from outside of the script.
    public void Open()
    {
        show = true;
    }
}
