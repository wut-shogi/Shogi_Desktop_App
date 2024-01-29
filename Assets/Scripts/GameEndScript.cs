using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEndScript : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPasser.instance.Winner != null) 
            text.text = "Game Over Winner is " + PlayerPasser.instance.Winner.name + " as " + (PlayerPasser.instance.Winner.forward==1?"White":"Black");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
