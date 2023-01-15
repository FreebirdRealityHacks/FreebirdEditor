using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Oculus.Interaction
{
    public class ScrubberUI : MonoBehaviour
    {
        [SerializeField]
         FingerTipInteractable InteractableR;
        [SerializeField]
        FingerTipInteractable InteractableL;

        [SerializeField]
        private Transform surfaceTransform;

        [SerializeField]
        private Renderer reticle;

        [SerializeField]
        private GameObject indicator;

        private bool _started = false;

        void Start()
        {
            this.BeginStart(ref _started);


            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
               InteractableR.WhenChanged += UpdateVisualState;
               InteractableL.WhenChanged += UpdateVisualState;
            }
        }

        void Update()
        {

        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                InteractableR.WhenChanged -= UpdateVisualState;
                InteractableL.WhenChanged -= UpdateVisualState;
            }
        }

        private void UpdateVisualState(FingerTipInteractableStateChangeArgs args) => UpdateVisuals(args);

        protected virtual void UpdateVisuals(FingerTipInteractableStateChangeArgs args)
        {
            if (args.FingertipPosition != null) {
                Vector3 pos = (Vector3) args.FingertipPosition;
                pos.y = surfaceTransform.position.y;
                reticle.transform.position = pos;

                if (args.NewState == InteractableState.Select)
                {
                    Vector3 posAtSameY = pos;
                    posAtSameY.y = indicator.transform.position.y;
                    indicator.transform.LookAt(posAtSameY, Vector3.up);
                }
            }
        }

    }
}