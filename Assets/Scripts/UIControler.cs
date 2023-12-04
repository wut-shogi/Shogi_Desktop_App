using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIControler : MonoBehaviour, IPointerClickHandler
{
    public char type;
    public void OnPointerClick(PointerEventData eventData)
    {
        if ((char.IsLower(type) && GameManager.instance.currentPlayer == GameManager.instance.player2)|| (!char.IsLower(type) && GameManager.instance.currentPlayer == GameManager.instance.player1)) 
        {
            return;
        }
        if (!GameManager.instance.hand.Contains(type)) return;
        HandDropSelector selector = HandDropSelector.instance;
        selector.EnterState(type);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
