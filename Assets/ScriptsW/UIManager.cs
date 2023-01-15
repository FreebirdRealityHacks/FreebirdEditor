using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject buttonPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonSelected(){
        buttonPanel.GetComponent<Renderer>().material.color = Color.magenta;
        //.SetColor("_Color", Color.magenta);
    }

    public void ButtonDeselected(){
        buttonPanel.GetComponent<Renderer>().material.color = Color.white;
        //SetColor("_Color", Color.white);
    }
}
