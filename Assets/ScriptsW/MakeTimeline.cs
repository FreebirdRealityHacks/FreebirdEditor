using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

//[ExecuteAlways]
[ExecuteInEditMode]
public class MakeTimeline : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject cube;
    public GameObject track;

    public GameObject canvas;
    public GameObject cubeParent;
    public GameObject trackParent;

    private TextMeshProUGUI instantiatedText;
    private GameObject instantiatedCube;
    private GameObject instantiatedTrack;

    public AudioSource audioSource;

    private int audioLength;
    private float halfLength;

    private void Destroy(){
        GameObject[] objects = GameObject.FindGameObjectsWithTag("text");
        foreach (GameObject obj in objects) {
            Debug.Log("hi");
            GameObject.DestroyImmediate(obj);
        }
    }

    void OnValidate()
    {
        audioLength = Mathf.CeilToInt(audioSource.clip.length);

        GameObject[] textObjects = GameObject.FindGameObjectsWithTag("text");
        foreach (GameObject obj in textObjects) {
            UnityEditor.EditorApplication.delayCall+=()=>
            {
                DestroyImmediate(obj);
            };
        }


        for(int z=0; z<audioLength; z++){
            instantiatedText = Instantiate(text, new Vector3(-1.5f, 0, z), Quaternion.Euler(90, 0, 0));
            instantiatedText.GetComponent<TextMeshProUGUI>().text = z.ToString();
            instantiatedText.transform.SetParent(canvas.transform, false);

            for(float z2=z+.25f; z2<z+1;z2+=.25f){
                instantiatedText = Instantiate(text, new Vector3(-1.5f, 0, z2), Quaternion.Euler(90, 0, 0));
                instantiatedText.GetComponent<TextMeshProUGUI>().text = "-";
                instantiatedText.transform.SetParent(canvas.transform, false);
            }
        }



        GameObject[] cubeObjects = GameObject.FindGameObjectsWithTag("cube");
        foreach (GameObject obj in cubeObjects) {
            UnityEditor.EditorApplication.delayCall+=()=>
            {
                DestroyImmediate(obj);
            };
        }


        for(int z=0; z<audioLength; z++){
            instantiatedCube = Instantiate(cube, new Vector3(0, 0.05f, z), Quaternion.identity);
            instantiatedCube.transform.SetParent(cubeParent.transform, false);
        }


        
        GameObject[] trackObjects = GameObject.FindGameObjectsWithTag("track");
        foreach (GameObject obj in trackObjects) {
            UnityEditor.EditorApplication.delayCall+=()=>
            {
                DestroyImmediate(obj);
            };
        }

        for(float x=-1.4f; x<=0.7f; x+=0.7f){
            halfLength = (audioLength -1) / 2;
            instantiatedTrack = Instantiate(track, new Vector3(x, 0, halfLength), Quaternion.identity);

            instantiatedTrack.transform.localScale = new Vector3(0.6f, 0.1f, audioLength-1);
            instantiatedTrack.transform.SetParent(trackParent.transform, false);
        }
        
        
    }

    void Update() {
            if (Application.IsPlaying(gameObject)) {

                // Play logic

            } else {
                //Debug.Log("hello");
                // - Update is only called when something in the Scene changed.
                /*if(isFirst){
                    for(int z=0; z<20; z++){
                        Instantiate(cube, new Vector3(0, 0, z), Quaternion.identity);
                    }
                    isFirst = false;
                }*/
                // Editor logic

                //Debug.LogWarning("Editor Update()");
                
                
                
            }
        }
}
