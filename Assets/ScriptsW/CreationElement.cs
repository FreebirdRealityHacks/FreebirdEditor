using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreationElement
{
    public enum Type {
        VFX,
        SFX,
        Skybox
    }

    public enum EffectName {
        Firework,
        FireCircle,
        Reverb,
        Echo,
        BlueSkybox,
        PinkSkybox,
        NeutralSkybox,
    }

    public Type type;
    public EffectName effectName;
    public Vector3 position;
    public float startTime;
    public float endTime;
}
