using System;
using Oculus.Interaction.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Oculus.Interaction
{
    public class HandClapInteractor : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _handR;
        public IHand HandR => _handR as IHand;

        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _handL;
        public IHand HandL => _handL as IHand;

        [SerializeField]
        private UnityEvent _whenClapped;

        // [SerializeField]
        // private Renderer debugSphereL;
        // [SerializeField]
        // private Renderer debugSphereR;
        // [SerializeField]
        // private Renderer debugSphereClap;

        private Vector3 _lastClappedPos;
        private Vector3 _lastRPos;
        private Vector3 _lastLPos;

        protected bool _started = false;
        protected bool _needsUpdate = false;

        private bool _shouldTrigger = true;

        void Start()
        {
            this.BeginStart(ref _started);

            Assert.IsNotNull(HandR);
            Assert.IsNotNull(HandL);

            this.EndStart(ref _started);
        }

        void Update()
        {
            if (_needsUpdate)
            {
                // debugSphereClap.transform.position = _lastClappedPos;
                // debugSphereR.transform.position = _lastRPos;
                // debugSphereL.transform.position = _lastLPos;

                // debugSphereR.GetComponent<Renderer>().material.color = _shouldTrigger ? Color.green : Color.red;
            }
        }

        protected void LateUpdate()
        {
            if (HandR == null || HandL == null)
            {
                _needsUpdate = false;
                return;
            }
            _needsUpdate = true;

            _lastRPos = _middleFingerBase(HandR);
            _lastLPos = _middleFingerBase(HandL);
            float distance = Vector3.Distance(_lastRPos, _lastLPos);
            if (_shouldTrigger && distance < 0.10)
            {
                _lastClappedPos = Vector3.Lerp(_lastRPos, _lastLPos, 0.5f);
                _shouldTrigger = false;
                _whenClapped.Invoke();
            } else if (distance > 0.20)
            {
                _shouldTrigger = true;
            }
        }

        public Vector3 GetLastClappedPosition() {
            return _lastClappedPos;
        }

        private Vector3 _middleFingerBase(IHand hand)
        {
            HandJointId jointId = HandJointUtils.GetHandFingerProximal(HandFinger.Middle);
            if (hand.GetJointPose(jointId, out Pose jointPose))
            {
                return jointPose.position;
            }
            return Vector3.zero;
        }
    }
}