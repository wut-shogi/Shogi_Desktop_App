using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBotMatch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void StartMatch()
    {
        if(PlayerPasser.instance.player1 is not HumanPlayer&& PlayerPasser.instance.player2 is not HumanPlayer)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Sandbox");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
