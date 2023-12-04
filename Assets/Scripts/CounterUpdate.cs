using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CounterUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var s = gameObject.GetComponent<TMP_Text>();
        var  p = gameObject.GetComponentInParent<UIControler>();
        s.text = GameManager.instance.hand.Where(x=>x==p.type).Count().ToString();
    }
}
