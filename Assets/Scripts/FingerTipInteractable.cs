using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using UnityEngine.Events;

namespace Oculus.Interaction
{
    public class FingerTipInteractable : MonoBehaviour
    {
        // Update only:
        // - When state changes
        // - On Update for Hover or Select State
        public event Action<FingerTipInteractableStateChangeArgs> WhenChanged;

        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;
        public IHand Hand => _hand as IHand;

        [SerializeField, Interface(typeof(IInteractableView))]
        private MonoBehaviour _interactableView;
        private IInteractableView InteractableView;

        //[SerializeField]
        //private Renderer debugSphere;

        private Vector3? _lastFingertipPos;
        InteractableState _previousState;
        InteractableState _newState;

        protected void Awake()
        {
            InteractableView = _interactableView as IInteractableView;
        }

        protected bool _started = false;
        private bool _isVisible = true;
        public bool IsVisible => _isVisible;
        protected bool _didStateChange = false;

        void Start()
        {
            this.BeginStart(ref _started);

            Assert.IsNotNull(Hand);
            Assert.IsNotNull(InteractableView);

            UpdateDebugVisual();
            this.EndStart(ref _started);
        }

        void Update()
        {
            if (_shouldUpdateDelegates())
            {
                WhenChanged(new FingerTipInteractableStateChangeArgs(_previousState, _newState, _lastFingertipPos));
                _didStateChange = false;
            }
        }

        protected void LateUpdate()
        {
            if (Hand == null || !IsVisible)
            {
                _lastFingertipPos = null;
                return;
            }

            if (_shouldUpdateDelegates()) {
                _lastFingertipPos = _fingerTipPosition();
            }

            //if (debugSphere != null)
            //{
            //    debugSphere.transform.position = _lastFingertipPos ?? Vector3.zero;
            //}
        }


        protected void OnEnable()
        {
            if (_started)
            {
                UpdateDebugVisual();
                InteractableView.WhenStateChanged += WhenStateChanged;
            }
        }

        protected void OnDisable()
        {
            if (_started)
            {
                InteractableView.WhenStateChanged -= WhenStateChanged;
            }
        }

        protected void UpdateDebugVisual()
        {
            /*
            if (debugSphere != null)
            {
                switch (InteractableView.State)
                {
                    case InteractableState.Select:
                        debugSphere.material.color = Color.green;
                        break;
                    case InteractableState.Hover:
                        debugSphere.material.color = Color.yellow;
                        break;
                    default:
                        debugSphere.material.color = Color.grey;
                        break;
                }
            }*/
        }
        protected void _WhenStateChanged(InteractableStateChangeArgs args)
        {
            _previousState = args.PreviousState;
            _newState = args.NewState;
            _didStateChange = true;
            UpdateDebugVisual();
        }

        private void WhenStateChanged(InteractableStateChangeArgs args) => _WhenStateChanged(args);
        private bool _shouldUpdateDelegates()
        {
            return _didStateChange || (_newState == InteractableState.Hover || _newState == InteractableState.Select);
        }

        private Vector3? _fingerTipPosition()
        {
            HandJointId jointId = HandJointUtils.GetHandFingerTip(HandFinger.Index);
            if (Hand.GetJointPose(jointId, out Pose jointPose))
            {
                return jointPose.position;
            }
            return null;
        }
    }

    public struct FingerTipInteractableStateChangeArgs
    {
        public InteractableState PreviousState { get; }
        public InteractableState NewState { get; }
        public Vector3? FingertipPosition { get; } // null if hand DNE

        public FingerTipInteractableStateChangeArgs(
            InteractableState previousState,
            InteractableState newState,
            Vector3? fingertipPosition)
        {
            PreviousState = previousState;
            NewState = newState;
            FingertipPosition = fingertipPosition;
        }
    }
}