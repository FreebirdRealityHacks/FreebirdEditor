using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction
{
    public class FingertipInteractable : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;
        public IHand Hand => _hand as IHand;

        [SerializeField, Interface(typeof(IInteractableView))]
        private MonoBehaviour _interactableView;
        private IInteractableView InteractableView;

        private Renderer _renderer;

        protected virtual void Awake()
        {
            InteractableView = _interactableView as IInteractableView;

        }

        protected bool _started = false;
        private bool _isVisible = true;
        public bool IsVisible => _isVisible;

        void Start()
        {
            this.BeginStart(ref _started);

            Assert.IsNotNull(Hand);
            Assert.IsNotNull(InteractableView);
            _renderer = GetComponent<Renderer>();

            UpdateVisual();
            this.EndStart(ref _started);
        }

        void Update()
        {

        }

        protected virtual void LateUpdate()
        {
            if (Hand == null || !IsVisible)
            {
                return;
            }

            _DrawFingerTip();
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                UpdateVisual();
                InteractableView.WhenStateChanged += UpdateVisualState;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                InteractableView.WhenStateChanged -= UpdateVisualState;
            }
        }

        protected virtual void UpdateVisual()
        {
            switch (InteractableView.State)
            {
                case InteractableState.Select:
                    _renderer.material.color = Color.green;
                    break;
                case InteractableState.Hover:
                    _renderer.material.color = Color.yellow;
                    break;
                default:
                    _renderer.material.color = Color.grey;

                    break;
            }
        }

        private void UpdateVisualState(InteractableStateChangeArgs args) => UpdateVisual();


        private void _DrawFingerTip()
        {
            HandJointId jointId = HandJointUtils.GetHandFingerTip(HandFinger.Index);
            if (Hand.GetJointPose(jointId, out Pose jointPose))
            {
                transform.position = jointPose.position;
            }
        }
    }
}