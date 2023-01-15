using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationElement
{
    public enum Type {
        VFX,
        SFX,
    }

    public enum EffectName {
        Firework,
        FireCircle,
        Reverb,
        Echo,
    }

    public Type type;
    public EffectName effectName;
    public Vector3 position;
    public float startTime;
    public float endTime;
}
