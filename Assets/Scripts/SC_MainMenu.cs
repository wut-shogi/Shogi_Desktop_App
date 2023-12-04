using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SC_MainMenu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject CreditsMenu;
    public GameObject PlayMenu;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuButton();
    }

    public void Play()
    {
        MainMenu.SetActive(false);
        CreditsMenu.SetActive(false);
        PlayMenu.SetActive(true);
    }
    public void Sandbox()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Sandbox");
    }
    public void Singleplayer()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Sandbox");
    }
    public void Multiplayer()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Sandbox");
    }

    public void CreditsButton()
    {
        // Show Credits Menu
        MainMenu.SetActive(false);
        PlayMenu.SetActive(false);
        CreditsMenu.SetActive(true);
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        MainMenu.SetActive(true);
        CreditsMenu.SetActive(false);
        PlayMenu.SetActive(false);
    }

    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
    }
}