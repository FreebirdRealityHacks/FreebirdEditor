using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public AudioSource audioSource;
    public TimelineController timelineController;
    public List<CreationElement> effectList = new List<CreationElement>();
    public HashSet<CreationElement> effectsPlaying = new HashSet<CreationElement>();
    public GameObject player;

    enum GameMode {
        Record,
        Edit,
        Play
    }

    // TODO: Change to Edit
    GameMode gameMode = GameMode.Record;


    // Visual Effects
    public GameObject fireworkPrefab;
    public GameObject fireCirclePrefab;

    // Audio Effects
    public AudioReverbFilter reverbFilter;
    public AudioEchoFilter echoFilter;

    // Start is called before the first frame update
    void Start()
    {
        StopReverb();
        StopEcho();
    }


    void Update(){
        GameMode previousGameMode = gameMode;

        if (Input.GetKeyDown("t")) {
            gameMode = GameMode.Edit;
        }


        if (Input.GetKeyDown("y")) {
            gameMode = GameMode.Play;
        }


        if (Input.GetKeyDown("u")) {
            gameMode = GameMode.Record;
        }


        //readinput when press a , create object with current time and position at cursor
        if (gameMode == GameMode.Record) {
            if (Input.GetKeyDown("v")) {
                AppendVFX1();
            }

            if (Input.GetKeyDown("b")) {
                AppendVFX2();
            }

            if (Input.GetKeyDown("r")) {
                AppendReverbEffect();
            }

            if (Input.GetKeyDown("e")) {
                AppendEchoEffect();
            }
        }

        bool enteredPlayMode = previousGameMode != GameMode.Play && gameMode == GameMode.Play;
        if (enteredPlayMode) {
            timelineController.EnterPlayMode();
            audioSource.time = 0;
            audioSource.Play();
        }

        bool leftPlayMode = previousGameMode == GameMode.Play && gameMode != GameMode.Play;
        if (leftPlayMode) {
            timelineController.ExitPlayMode();
            audioSource.Pause();
        }

        // Input play/pause
        if (Input.GetKeyDown("space")) {
            if (!audioSource.isPlaying) {
                audioSource.Play();
            } else {
                audioSource.Pause();
            }
        }

        // seek
        if (!Mathf.Approximately(Input.GetAxis("Vertical"), 0)) {
            audioSource.time += Input.GetAxis("Vertical") * 20f * Time.deltaTime;
        }

        if (audioSource.isPlaying) {
            // Play VFX
            for (int i = 0; i < effectList.Count; i += 1) {
                CreationElement effect = effectList[i];
                if (effect.startTime <= audioSource.time && audioSource.time <= effect.endTime) {
                    if (effect.type == CreationElement.Type.VFX && !effectsPlaying.Contains(effect)) {
                        StartCoroutine(StartThenStopVFX(effect));
                    }
                }
            }

            // Play SFX

            if (IsReverbEffectEnabled()) {
                ApplyReverb();
            } else {
                StopReverb();
            }

            if (IsEchoEffectEnabled()) {
                ApplyEcho();
            } else {
                StopEcho();
            }
        }

    }

    bool IsReverbEffectEnabled() {
        for (int i = 0; i < effectList.Count; i += 1) {
            CreationElement effect = effectList[i];

            if (!(effect.startTime <= audioSource.time && audioSource.time <= effect.endTime)) {
                continue;
            }

            if (effect.type == CreationElement.Type.SFX && effect.effectName == CreationElement.EffectName.Reverb) {
                return true;
            }
        }
        return false;
    }

    bool IsEchoEffectEnabled() {
        for (int i = 0; i < effectList.Count; i += 1) {
            CreationElement effect = effectList[i];

            if (!(effect.startTime <= audioSource.time && audioSource.time <= effect.endTime)) {
                continue;
            }

            if (effect.type == CreationElement.Type.SFX && effect.effectName == CreationElement.EffectName.Echo) {
                return true;
            }
        }
        return false;
    }

    IEnumerator StartThenStopVFX(CreationElement effect) {
        effectsPlaying.Add(effect);

        if (effect.effectName == CreationElement.EffectName.Firework) {
            GameObject vfx = Instantiate(fireworkPrefab, effect.position, Quaternion.identity);
            float gap = 0.1f;
            yield return new WaitForSeconds(effect.endTime - effect.startTime + gap);
            Destroy(vfx);
        }

        if (effect.effectName == CreationElement.EffectName.FireCircle) {
            GameObject vfx = Instantiate(fireCirclePrefab, effect.position, Quaternion.identity);
            float gap = 0.1f;
            yield return new WaitForSeconds(effect.endTime - effect.startTime + gap);
            Destroy(vfx);
        }
        
        effectsPlaying.Remove(effect);
    }

    public void AppendVFX1() {
        CreationElement element1 = new CreationElement();
        element1.type = CreationElement.Type.VFX;
        element1.effectName = CreationElement.EffectName.Firework;
        element1.position = new Vector3(Random.Range(-1f, 1f), Random.Range(1.0f, 1.5f), player.transform.localPosition.z + 1.5f);
        element1.startTime = audioSource.time + 0.05f;
        element1.endTime = element1.startTime + 2f;

        timelineController.AddCreationElement(element1);
        effectList.Add(element1);
    }

    public void AppendVFX2() {
        CreationElement element1 = new CreationElement();
        element1.type = CreationElement.Type.VFX;
        element1.effectName = CreationElement.EffectName.FireCircle;
        element1.position = new Vector3(Random.Range(-1f, 1f), Random.Range(1.0f, 1.5f), player.transform.localPosition.z + 1.5f);
        element1.startTime = audioSource.time + 0.05f;
        element1.endTime = element1.startTime + 2f;

        timelineController.AddCreationElement(element1);
        effectList.Add(element1);
    }

    public void AppendEchoEffect() {
        CreationElement element1 = new CreationElement();
        element1.type = CreationElement.Type.SFX;
        element1.effectName = CreationElement.EffectName.Echo;
        element1.position = new Vector3(0, 0, 0);
        element1.startTime = audioSource.time + 0.05f;
        element1.endTime = element1.startTime + 1.5f;

        timelineController.AddCreationElement(element1);
        effectList.Add(element1);
    }

    public void AppendReverbEffect() {
        CreationElement element1 = new CreationElement();
        element1.type = CreationElement.Type.SFX;
        element1.effectName = CreationElement.EffectName.Reverb;
        element1.position = new Vector3(0, 0, 0);
        element1.startTime = audioSource.time + 0.05f;
        element1.endTime = element1.startTime + 1.5f;

        timelineController.AddCreationElement(element1);
        effectList.Add(element1);
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
