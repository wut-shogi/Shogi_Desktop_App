using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using TMPro;
using System.Diagnostics;
using System;

[RequireComponent(typeof(Button))]
public class CanvasSampleOpenFileText : MonoBehaviour, IPointerDownHandler {
    public TextMeshProUGUI output;
    public bool player1 = true;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", ".txt", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(OutputRoutine(url));
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }


    public void AddBot()
    {
        Player p = new BotPlayer("Engine", player1);
        if (player1)
            PlayerPasser.instance.player1 = p;
        else
            PlayerPasser.instance.player2 = p;
    }

    void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "exe", false);
        if (paths.Length > 0)
        {
            output.text = paths[0];
            
           Player p =  new UsiPlayer("Engine", player1, paths[0]);
            if (player1)
               PlayerPasser.instance.player1 = p;
            else
               PlayerPasser.instance.player2 = p;
        }
    }
#endif

}