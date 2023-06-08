using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatefulButton : MonoBehaviour {
    public string defaultState;
    public string otherState;
    public GameObject defaultStateIcon;
    public GameObject otherStateIcon;

    public UnityEvent OnDefaultStatePress;
    public UnityEvent OnOtherStatePress;

    private bool shouldChangeState = false;
    private string currentState;

    public void OnPress() {
        shouldChangeState = true;
    }

    void Start() {
        Reset();
    }

    void Update() {
        if (!shouldChangeState) {
            return;
        }

        shouldChangeState = false;

        if (currentState == defaultState) {
            OnDefaultStatePress.Invoke();
            defaultStateIcon.SetActive(false);
            otherStateIcon.SetActive(true);
            currentState = otherState;
        } else if (currentState == otherState) {
            OnOtherStatePress.Invoke();
            otherStateIcon.SetActive(false);
            defaultStateIcon.SetActive(true);
            currentState = defaultState;
        }
    }

    public void Reset() {
        otherStateIcon.SetActive(false);
        currentState = defaultState;
        defaultStateIcon.SetActive(true);
    }
}
