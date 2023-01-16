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

        public bool hasUpdate;

        //public TMP_Text text;
        /*
        [SerializeField]
        private Renderer purpleDebug;
        [SerializeField]
        private Renderer yellowDebug;
        */

        //private float[] _deltaAngles = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //private int _currAngleIndex = 0;

        //private bool _started = false;
        //private Vector3 _prevForward = Vector3.zero;

        //private bool _shouldSpin = false;
        //private string _debugString = "";

        // Returns the percentage that was changed since last time update was called.
        // Between -1 and 1
        //public event System.Action<float> WhenPercentageChanged;

        //void Start()
        //{
        //    this.BeginStart(ref _started);


        //    this.EndStart(ref _started);
        //}

        private float _startAngle = 0f;
        private float _prevAngle = 0f;
        private Quaternion _startRot = Quaternion.identity;
        private float[] _prevDeltas = { 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        private int _prevDeltasIndex = 0;

        /****************** PUBLIC */
        private float valueSensitivity = 500f;
        public float value // negative or positive
        {
            get { return _value; }
        }
        /****************** */

        private float _value;

        protected void OnEnable()
        {
            //if (_started)
            //{
            InteractableR.WhenChanged += WhenFingerChanged;
            InteractableL.WhenChanged += WhenFingerChanged;
            //}
        }

        void Update()
        {
            //indicator.transform.rotation = Quaternion.LookRotation(transform.right, indicator.transform.up);
            //purpleDebug.transform.position = indicator.transform.position;

            // text.text = _debugString;

            //if (_shouldSpin) {


            //}
        }

        protected void OnDisable()
        {
            //if (_started)
            //{
            InteractableR.WhenChanged -= WhenFingerChanged;
            InteractableL.WhenChanged -= WhenFingerChanged;
            //}
            hasUpdate = false;
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


            //_shouldSpin = args.NewState == InteractableState.Select;

            Vector3 currPos = indicator.position;
            Vector3 fingerPos = (Vector3)args.FingertipPosition;
            fingerPos = new Vector3(fingerPos.x, fingerPos.y, fingerPos.z);
            fingerPos.y = currPos.y;
            float distance = Vector3.Distance(fingerPos, currPos);
            if (distance > 0.1)
            {
                debugObj.material.color = Color.red;
                reticle.position = Vector3.zero;
                hasUpdate = false;  
                return;
            }

            // Reticle for Hover State
            reticle.position = new Vector3(fingerPos.x, fingerPos.y + 0.002f, fingerPos.z);
            if (args.PreviousState == InteractableState.Normal
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
                _value = total;
                Debug.Log("**** average : " + total + " current " + currentAngle);

                //debugObj.transform.position = currPos + (transform.right * 0.1f);

                _prevAngle = currentAngle;
                _prevDeltas[_prevDeltasIndex] = delta;
                _prevDeltasIndex = (_prevDeltasIndex + 1) % _prevDeltas.Length;

                // SPIN
                indicator.transform.rotation = _startRot * Quaternion.AngleAxis(currentAngle - _startAngle, transform.up);
                
                //  indicator.transform.rotation = Quaternion.LookRotation(currentForward, indicator.transform.up);


                //float projOnRightSide = Vector3.Dot(currentForward, indicator.transform.right);
                //float multiplier = projOnRightSide >= 0 ? 1 : -1;
                //_value = multiplier * distance * valueSensitivity;


                // SPINNNING
                //indicator.transform.rotation = Quaternion.AngleAxis(_value * Time.deltaTime, transform.up) * indicator.transform.rotation;

                //if (_value > 15 || value < -15)
                //{
                //    _debugString = "Value!: " + _value;
                //    Quaternion quaternion = new Quaternion(0, 1 / _value, 0, 1);
                //    indicator.transform.SetPositionAndRotation(indicator.transform.position, quaternion * indicator.transform.rotation);
                //    hasUpdate = true;
                //    return;
                //}














                //                    indicator.transform.SetPositionAndRotation(indicator.transform.position, indicator.transform.rotation * quaternion);

                //indicator.transform.Rotate(0, _value * Time.deltaTime, 0);

                // indicator.transform.rotation = Quaternion.LookRotation(currentForward, indicator.transform.up);

                /*
            float angle = Vector3.Angle(currentForward, _prevForward); // 0 to 180

            bool isClockwise = Vector3.Dot(Vector3.Cross(_prevForward, currentForward), indicator.transform.up) > 0;
            angle *= (isClockwise ? 1 : -1);

            float averageAngle = angle;
            for (int i = 0; i < _deltaAngles.Length; i++)
            {
                averageAngle += _deltaAngles[i];
            }
            averageAngle /= (_deltaAngles.Length + 1);

            text.text = "averageAngle: " + averageAngle;

            indicator.transform.RotateAround(currPos, indicator.transform.up, angle);


            _deltaAngles[_currAngleIndex] = angle;
            _currAngleIndex = (_currAngleIndex + 1) % _deltaAngles.Length;
            _prevForward = new Vector3(currentForward.x, currentForward.y, currentForward.z);


            //indicator.transform.rotation = Quaternion.LookRotation(_prevForward, indicator.transform.up);


            //WhenPercentageChanged(averageAngle / 360);

                */
            }

            hasUpdate = false;
        }

    }
}