using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public GameObject firework1;
    public GameObject firework2;
    private Vector3 vfxPos;
    private GameObject instantiatedFirework;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Vector3 clapPos
    public void DisplayFirework(){
        
        vfxPos = new Vector3(0, 3, 1);

        //instantiatedFirework = Instantiate(firework, vfxPos, Quaternion.identity);
        //StartCoroutine(ExampleCoroutine());

        StartCoroutine(ExampleCoroutine(Instantiate(firework1, vfxPos, Quaternion.identity)));
    }


    public void DisplayFirework2(){
        vfxPos = new Vector3(0, 3, 1);

        //instantiatedFirework = Instantiate(firework, vfxPos, Quaternion.identity);

        StartCoroutine(ExampleCoroutine(Instantiate(firework2, vfxPos, Quaternion.identity)));
    }

    IEnumerator ExampleCoroutine(GameObject vfx)
    {
        yield return new WaitForSeconds(3);
        
        Destroy(vfx);

        //Destroy(instantiatedFirework);

    }
}
