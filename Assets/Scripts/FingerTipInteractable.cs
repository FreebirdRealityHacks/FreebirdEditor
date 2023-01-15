using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Assertions;
using System;

namespace Oculus.Interaction
{
    public class FingerTipInteractable : MonoBehaviour
    {
        public event Action<FingerTipInteractableStateChangeArgs> WhenChanged;

        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;
        public IHand Hand => _hand as IHand;

        [SerializeField, Interface(typeof(IInteractableView))]
        private MonoBehaviour _interactableView;
        private IInteractableView InteractableView;

        [SerializeField]
        private Renderer debugSphere;

        private Vector3 _lastFingertipPos;
        InteractableState _previousState;
        InteractableState _newState;

        protected void Awake()
        {
            InteractableView = _interactableView as IInteractableView;
        }

        protected bool _started = false;
        private bool _isVisible = true;
        public bool IsVisible => _isVisible;
        protected bool _needsUpdate = false;

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
            if (_needsUpdate)
            {
                WhenChanged(new FingerTipInteractableStateChangeArgs(_previousState, _newState, _lastFingertipPos));
            }
        }

        protected void LateUpdate()
        {
            if (Hand == null || !IsVisible)
            {
                _needsUpdate = false;
                return;
            }
            _needsUpdate = true;
            _lastFingertipPos = _fingerTipPosition() ?? Vector3.zero;
            if (debugSphere != null)
            {
                debugSphere.transform.position = _lastFingertipPos;
            }
        }

        protected void OnEnable()
        {
            if (_started)
            {
                UpdateDebugVisual();
                InteractableView.WhenStateChanged += UpdateVisualState;
            }
        }

        protected void OnDisable()
        {
            if (_started)
            {
                InteractableView.WhenStateChanged -= UpdateVisualState;
            }
        }

        protected void UpdateDebugVisual()
        {
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
            }
            _needsUpdate = true;
        }

        protected void UpdateVisuals(InteractableStateChangeArgs args)
        {
            _previousState = args.PreviousState;
            _newState = args.NewState;
            UpdateDebugVisual();
        }

        private void UpdateVisualState(InteractableStateChangeArgs args) => UpdateVisuals(args);

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
        public Vector3? FingertipPosition { get; }

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