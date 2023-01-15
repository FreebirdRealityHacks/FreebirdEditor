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
        private Renderer reticle;

        [SerializeField]
        private GameObject indicator;

        public TMP_Text text;

        [SerializeField]
        private Renderer purpleDebug;
        [SerializeField]
        private Renderer yellowDebug;


        //private float[] _deltaAngles = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //private int _currAngleIndex = 0;

        //private bool _started = false;
        //private Vector3 _prevForward = Vector3.zero;

        //private bool _shouldSpin = false;
        private string _debugString = "";

        // Returns the percentage that was changed since last time update was called.
        // Between -1 and 1
        //public event System.Action<float> WhenPercentageChanged;

        //void Start()
        //{
        //    this.BeginStart(ref _started);


        //    this.EndStart(ref _started);
        //}



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
            InteractableR.WhenChanged += UpdateVisualState;
            InteractableL.WhenChanged += UpdateVisualState;
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
            InteractableR.WhenChanged -= UpdateVisualState;
            InteractableL.WhenChanged -= UpdateVisualState;
            //}
        }

        private void UpdateVisualState(FingerTipInteractableStateChangeArgs args) => _UpdateVisualState(args);

        protected void _UpdateVisualState(FingerTipInteractableStateChangeArgs args)
        {
            if (args.FingertipPosition == null)
            {
                //_shouldSpin = false;
                return;
            }

            //_shouldSpin = args.NewState == InteractableState.Select;

            if (args.NewState == InteractableState.Hover || args.NewState == InteractableState.Select)
            {
                // Reticle for Hover State
                Vector3 fingerPos = (Vector3)args.FingertipPosition;
                fingerPos.y = surfaceTransform.position.y + 0.01f;
                reticle.transform.position = fingerPos;

                if (args.NewState == InteractableState.Select)
                {
                    Vector3 currPos = indicator.transform.position;

                    Vector3 fingerPosAtSameY = fingerPos;
                    fingerPosAtSameY.y = currPos.y;

                    Vector3 currentForward = fingerPosAtSameY - currPos;
                    float projOnRightSide = Vector3.Dot(currentForward, transform.right);
                    float multiplier = projOnRightSide >= 0 ? 1 : -1;
                    float distance = Vector3.Distance(currPos, fingerPosAtSameY);
                    _value = multiplier * distance * valueSensitivity;


                    // SPINNNING
                    //indicator.transform.rotation = Quaternion.AngleAxis(_value * Time.deltaTime, transform.up) * indicator.transform.rotation;

                    if (_value > 15 || value < -15)
                    {
                        _debugString = "Value!: " + _value;
                        Quaternion quaternion = new Quaternion(0, 1 / _value, 0, 1);
                        indicator.transform.SetPositionAndRotation(indicator.transform.position, quaternion * indicator.transform.rotation);
                    }

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
                //indicator.transform.LookAt(posAtSameY, Vector3.up);


                _deltaAngles[_currAngleIndex] = angle;
                _currAngleIndex = (_currAngleIndex + 1) % _deltaAngles.Length;
                _prevForward = new Vector3(currentForward.x, currentForward.y, currentForward.z);


                //indicator.transform.rotation = Quaternion.LookRotation(_prevForward, indicator.transform.up);


                //WhenPercentageChanged(averageAngle / 360);

                    */
                }
            }

        }

    }
}