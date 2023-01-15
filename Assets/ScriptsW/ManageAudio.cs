using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject audioObject;

    private AudioReverbFilter reverbFilter;
    private AudioEchoFilter echoFilter;

    // Start is called before the first frame update
    void Start()
    {
        reverbFilter = audioObject.GetComponent<AudioReverbFilter>();
        echoFilter = audioObject.GetComponent<AudioEchoFilter>();
    }


    public void ApplyReverb(){
        reverbFilter.enabled = true;

        /*
        filter.decayTime = 4f;
        filter.room = -500;
        filter.roomHF = -500;
        filter.reverbLevel = -500;
        */
    }

    public void StopReverb(){
        reverbFilter.enabled = false;
    }

    public void ApplyEcho(){
        echoFilter.enabled = true;
    }

    public void StopEcho(){
        echoFilter.enabled = false;
    }
}
