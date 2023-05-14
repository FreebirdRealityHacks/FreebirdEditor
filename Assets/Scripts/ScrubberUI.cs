using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

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
        private Transform reticle;

        [SerializeField]
        private Transform indicator;

        [SerializeField]
        private Renderer debugObj;

        [SerializeField]
        public TMP_Text text;

        /*
        [SerializeField]
        private Renderer purpleDebug;
        [SerializeField]
        private Renderer yellowDebug;
        */

        private float _startAngle = 0f;
        private float _prevAngle = 0f;
        private Quaternion _startRot = Quaternion.identity;
        private float[] _prevDeltas = { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        private int _prevDeltasIndex = 0;

        /****************** PUBLIC */
        public float value // negative or positive, from -1 to 1
        {
            get { return _value; }
        }
        /****************** */

        private float _value;
        private readonly float VALUE_STEP = 0.04f;

        protected void OnEnable()
        {
            InteractableR.WhenChanged += WhenFingerChanged;
            InteractableL.WhenChanged += WhenFingerChanged;
            text.text = "";
        }

        void Update()
        {
            reticle.position = new Vector3(0f, 100f, 0f);
            //text.text =  " reticle pos y: 0";
            if (_value != 0f)
            {
                if (Mathf.Abs(_value) <= VALUE_STEP/2)
                {
                    _value = 0f;
                } else
                {
                    _value += _value > 0 ? -VALUE_STEP/2 : VALUE_STEP/2;
                }
                //text.text = "scrubber value: " + Mathf.FloorToInt(_value * 100) / 100f;
            }
        }

        protected void OnDisable()
        {
            InteractableR.WhenChanged -= WhenFingerChanged;
            InteractableL.WhenChanged -= WhenFingerChanged;
        }

        private void WhenFingerChanged(FingerTipInteractableStateChangeArgs args) => _WhenFingerChanged(args);

        private float _AngleFrom360(Vector3 v)
        {
            bool isOnRight = Vector3.Dot(v, transform.right) >= 0;
            if (isOnRight)
            {
                return Vector3.Angle(transform.forward, v);
            } else
            {
                return 360 - Vector3.Angle(transform.forward, v);
            }
        }

        protected void _WhenFingerChanged(FingerTipInteractableStateChangeArgs args)
        {
            if (args.FingertipPosition == null)
            {
                return;
            }
            debugObj.material.color = Color.green;

            Vector3 currPos = indicator.position;
            Vector3 fingerPos = (Vector3)args.FingertipPosition;
            fingerPos = new Vector3(fingerPos.x, currPos.y, fingerPos.z);
            float distance = Vector3.Distance(fingerPos, currPos);
            if (distance > 0.1) // finger is outside of the scrubber circle
            {
                debugObj.material.color = Color.red;
                return;
            }

            // Reticle for Hover State
            reticle.position = new Vector3(fingerPos.x, currPos.y + 0.002f, fingerPos.z);

            if (args.NewState == InteractableState.Normal && args.PreviousState != InteractableState.Normal)
            {
                _value = 0;
                //text.text = "scrubber value: " + Mathf.FloorToInt(_value * 100) / 100f;
            }
            else if (args.PreviousState == InteractableState.Normal
                && (args.NewState == InteractableState.Select || args.NewState == InteractableState.Hover))
            {
                Vector3 centerToFinger = fingerPos - currPos;
                _startAngle = _AngleFrom360(centerToFinger);
                _prevAngle = 360*10; // TODO Logic works for 10 rotations
                _startRot = indicator.transform.rotation;
                for (int i = 0; i < _prevDeltas.Length; i++)
                {
                    _prevDeltas[i] = 0;
                }
                _prevDeltasIndex = 0;
            }
            else if (args.NewState == InteractableState.Select || args.NewState == InteractableState.Hover)
            {
                Vector3 centerToFinger = fingerPos - currPos;
                int prevRotations = (int)Mathf.Floor(_prevAngle / 360f);

                float currentAngle = _AngleFrom360(centerToFinger);
                if (currentAngle < 30 && _prevAngle % 360 > 330)
                {
                    currentAngle = (prevRotations + 1) * 360 + currentAngle;
                } else if (currentAngle > 330 && _prevAngle % 360 < 30) { 
                    currentAngle = (prevRotations - 1) * 360 + currentAngle;
                } else {
                    currentAngle = prevRotations * 360 + currentAngle;
                }
                float delta = currentAngle - _prevAngle;
                float total = delta;
                for (int i = 0; i < _prevDeltas.Length; i++)
                {
                    total += _prevDeltas[i];
                }
                float avgVal = total / (_prevDeltas.Length - 1);
                bool isMovingFast = Mathf.Abs(avgVal) > 2f;
                if (!isMovingFast)
                {
                    _value = 0;
                } else if (Mathf.Abs(_value) < 1f)
                {
                    _value += avgVal > 0 ? VALUE_STEP : -VALUE_STEP;
                    _value = Mathf.Clamp(_value, -1, 1);
                }
                //text.text = "scrubber value: " + Mathf.FloorToInt(_value * 100) / 100f;

                _prevAngle = currentAngle;
                _prevDeltas[_prevDeltasIndex] = delta;
                _prevDeltasIndex = (_prevDeltasIndex + 1) % _prevDeltas.Length;

                // SPIN
                indicator.transform.rotation = _startRot * Quaternion.AngleAxis(currentAngle - _startAngle, transform.up);
            }
        }

    }
}