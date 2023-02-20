using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Oculus.Interaction.Input;

namespace Oculus.Interaction
{
    public class ScrubberUI : MonoBehaviour
    {
        [SerializeField]
        private Transform surfaceTransform;

        [SerializeField]
        private Transform reticle;

        [SerializeField]
        private Transform indicator;

        [SerializeField]
        private Renderer debugObj;

        public bool hasUpdate;

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

        public bool isScrubbing
        {
            get {
                return _isSelected && Mathf.Abs(_value) > 10f;
            }
        }
        /****************** */

        private float _value;

        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _leftHand;
        public IHand leftHand => _leftHand as IHand;

        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _rightHand;
        public IHand rightHand => _rightHand as IHand;

        private IInteractableView _interactableView;
        bool _wasSelected = false;
        bool _isSelected = false;

        void Start() {
            _interactableView = GetComponent<IInteractableView>();
            _interactableView.WhenStateChanged += WhenInteractableStateChanged;
        }

        private void WhenInteractableStateChanged(InteractableStateChangeArgs args) {
            _wasSelected = args.PreviousState == InteractableState.Select || args.PreviousState == InteractableState.Hover;
            _isSelected = args.NewState == InteractableState.Select || args.NewState == InteractableState.Hover;
            Debug.Log($"Calling WhenInteractableStateChanged wasSelected: {_wasSelected}, isSelected: {_isSelected}");
        }

        private Vector3? GetFingerTipPosition()
        {
            HandJointId jointId = HandJointUtils.GetHandFingerTip(HandFinger.Index);
            Pose jointPose;
            // if (leftHand.GetJointPose(jointId, out jointPose))
            // {
            //     return jointPose.position;
            // }

            if (rightHand.GetJointPose(jointId, out jointPose))
            {
                return jointPose.position;
            }
            return null;
        }

        void Update()
        {
            if (!_isSelected) {
                reticle.localPosition = new Vector3(0, reticle.localPosition.y, 0);
                return;
            }

            Vector3? nullableFingerPos = GetFingerTipPosition();
            if (nullableFingerPos == null) {
                return;
            }

            Vector3 currPos = indicator.position;
            Vector3 fingerPos = (Vector3)nullableFingerPos;
            fingerPos = new Vector3(fingerPos.x, currPos.y, fingerPos.z);
            float distance = Vector3.Distance(fingerPos, currPos);
            if (distance > 0.2)
            {
                reticle.localPosition = new Vector3(0, reticle.localPosition.y, 0);
                return;
            }

            // Reticle for Hover State
            reticle.position = new Vector3(fingerPos.x, reticle.position.y, fingerPos.z);

            if (!_wasSelected) {
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
            else {
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

                _prevAngle = currentAngle;
                _prevDeltas[_prevDeltasIndex] = delta;
                _prevDeltasIndex = (_prevDeltasIndex + 1) % _prevDeltas.Length;

                // SPIN
                indicator.transform.rotation = _startRot * Quaternion.AngleAxis(currentAngle - _startAngle, transform.up);
            }
        }

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

    }
}