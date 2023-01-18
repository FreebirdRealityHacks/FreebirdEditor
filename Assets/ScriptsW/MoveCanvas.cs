using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCanvas : MonoBehaviour
{
    public Animator anim;
    public GameObject buttons;
    public GameObject backdrop;


    // Start is called before the first frame update
    void Start()
    {
        //anim.Play("RetractCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RetractCanvas(){
        //anim.Play("RetractCanvas");
        buttons.SetActive(false);
        backdrop.SetActive(false);
    }

    public void ExpandCanvas(){
        anim.Play("ExpandCanvas");
        buttons.SetActive(true);
        backdrop.SetActive(true);
    }
}
