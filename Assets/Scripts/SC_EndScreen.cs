using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EndScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }



    public void GoToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
