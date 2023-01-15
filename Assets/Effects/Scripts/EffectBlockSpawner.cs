using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBlockSpawner : MonoBehaviour
{
    public GameObject EffectBlockPrefab;
    

    // Update is called once per frame
    void Update()
    {
        // if currently selected track is AFX (audio effects)
            //move spawner to the AFX track
        // if currently selected track is VFX etc

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(EffectBlockPrefab, transform.position, Quaternion.identity);
        }

        
    }
}
