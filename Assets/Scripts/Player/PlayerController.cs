using UnityEngine;
using System;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GJ_GMTK_Jul_2025
{
    enum EState
    {
        Looping,
        Moving
    }

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] PlayerMovementData _playerMovData;

        bool _isTakingInput = true;
        Rigidbody _rigidBody;

#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            if (_currState == EState.Looping)
            {
                Handles.color = Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(_loopOffsetPoint, .1f);
                Gizmos.DrawLine(transform.position, _loopOffsetPoint);
                Vector3 diff = transform.position - _loopOffsetPoint;
                Vector3 mid = diff / 2;
                Handles.Label(_loopOffsetPoint + Vector3.forward * 0.2f, diff.magnitude.ToString("f3"));
            }

            Handles.color = Gizmos.color = Color.red;
            Vector3 to = transform.position + _rigidBody.linearVelocity;
            Gizmos.DrawSphere(to, .1f);
            Gizmos.DrawLine(transform.position, to);

            Handles.Label(transform.position + Vector3.right * 0.75f, _rigidBody.linearVelocity.magnitude.ToString("f3"));
        }

#endif

        private void Start()
        {
            _playerMovData ??= Player.PlayerMovData;
            _rigidBody ??= Player.PlayerRigidbody;
            _rigidBody ??= GetComponent<Rigidbody>();
        }

        void Update()
        {
            UpdateInput();

            UpdateState();

            UpdateTimers();
        }

        void FixedUpdate()
        {
            UpdateMovement();
        }

        #region Input

        bool _wantsToMove;
        bool _wantsToFlipLooping;

        private void UpdateInput()
        {
            if (!_isTakingInput)
            {
                _wantsToMove = true;
                _moveTimer = 0;
                return;
            }

            _wantsToMove = Input.GetKey(KeyCode.Mouse0);

            _wantsToFlipLooping = Input.GetKeyDown(KeyCode.Mouse1);
        }

        #endregion

        #region State Change

        EState _currState = (EState)(-1);

        internal void StopInputs()
        {
            _isTakingInput = false;
        }

        private void UpdateState()
        {
            bool calculateOffset = false;

            // State Changing
            EState targetState = _currState;
            if (_wantsToMove)
            {
                if (targetState == EState.Moving) // Still moving
                    _moveTimer = _playerMovData.MoveCooldown;
                else if (TryResetTimer(ref _moveTimer, _playerMovData.MoveCooldown))
                    targetState = EState.Moving;
            }
            else
                targetState = EState.Looping;

            if (ChangeState(targetState))
            {
                if (_currState == EState.Looping)
                    calculateOffset = true;
            }

            // Rotation flipping flag set
            if (_wantsToFlipLooping)
            {
                _wantsToFlipLooping = false;

                if (TryResetTimer(ref _flipLoopTimer, _playerMovData.LoopDirFlipCooldown))
                {
                    _isLoopingClockwise = !_isLoopingClockwise;

                    calculateOffset = true;
                }
            }

            if (calculateOffset)
                CalculateOffset();
        }

        private bool ChangeState(EState state)
        {
            if (_currState == state) return false;
            _currState = state;
            return true;
        }

        #endregion

        #region Movement

        bool _isLoopingClockwise = true;
        Vector3 _loopOffsetPoint;
        float _loopCorrectionMultiplier;

        private void CalculateOffset()
        {
            Vector3 offsetDir = _isLoopingClockwise ? transform.right : -transform.right;
            _loopOffsetPoint = transform.position + offsetDir * _playerMovData.LoopOffset;
        }

        private void UpdateMovement()
        {
            float baseSpeedMultiplier = 1;
            Vector3 linearVelocity = _rigidBody.linearVelocity;

            switch (_currState)
            {
                case EState.Looping:
                    baseSpeedMultiplier = _playerMovData.BaseLoopSpeed;
                    Vector3 centerToPlayer = _rigidBody.position - _loopOffsetPoint;
                    float radius = _playerMovData.LoopOffset;
                    ApplyLoopVelocity(ref linearVelocity, centerToPlayer);
                    UpdateLoopCorrectionMultiplier();
                    ApplyLoopCorrectionForce(ref linearVelocity, centerToPlayer, radius);
                    break;
                case EState.Moving:
                    baseSpeedMultiplier = _playerMovData.BaseMoveSpeed;
                    ApplyForwardVelocity(ref linearVelocity);
                    break;
            }

            if (_applyReboundVelocity)
                ApplyReboundVelocity(ref linearVelocity);
            if (_pullTarget.HasValue)
                ApplyPointPullVelocity(ref linearVelocity);
            if (_pushDirection.HasValue)
                ApplyDirectionalPushVelocity(ref linearVelocity);

            if (linearVelocity.magnitude > baseSpeedMultiplier)
                linearVelocity = linearVelocity.normalized * baseSpeedMultiplier;

            _rigidBody.linearVelocity = linearVelocity;
            _rigidBody.rotation = Quaternion.LookRotation(linearVelocity.normalized);
        }

        public void ApplyLoopVelocity(ref Vector3 linearVelocity, Vector3 centerToPlayer)
        {
            Vector3 tangent = Vector3.Cross(_isLoopingClockwise ? Vector3.up : Vector3.down, centerToPlayer).normalized;
            linearVelocity += tangent * _playerMovData.BaseLoopSpeed * Time.deltaTime;
        }

        private void UpdateLoopCorrectionMultiplier()
        {
            float loopCorrectionMultiplierTarget = _playerMovData.LoopCorrectionMultiplier;
            if (_pullTarget.HasValue
                || _pushDirection.HasValue)
                loopCorrectionMultiplierTarget = 0;
            _loopCorrectionMultiplier = Mathf.MoveTowards(_loopCorrectionMultiplier,
                                                          loopCorrectionMultiplierTarget,
                                                          _playerMovData.LoopCorrectionMultiplierFadeSpeed);
        }

        public void ApplyLoopCorrectionForce(ref Vector3 linearVelocity, Vector3 centerToPlayer, float radius)
        {
            float radiusError = centerToPlayer.magnitude - radius;
            if (Mathf.Abs(radiusError) <= Mathf.Epsilon)
                return;
            linearVelocity -= centerToPlayer * radiusError / Time.deltaTime * _loopCorrectionMultiplier;
        }

        public void ApplyForwardVelocity(ref Vector3 linearVelocity)
        {
            linearVelocity += transform.forward * _playerMovData.BaseMoveSpeed * Time.deltaTime;
        }

        private void ApplyReboundVelocity(ref Vector3 linearVelocity)
        {
            linearVelocity -= linearVelocity * 2;
            _applyReboundVelocity = false;
        }

        private void ApplyPointPullVelocity(ref Vector3 linearVelocity)
        {
            Vector3 pullPos = _pullTarget.Value;
            Vector3 toCenter = (pullPos - _rigidBody.position).normalized;
            Vector3 pullVelocity = _pullStrength * Time.deltaTime * toCenter;
            linearVelocity += pullVelocity;
        }

        private void ApplyDirectionalPushVelocity(ref Vector3 linearVelocity)
        {
            Vector3 pushVelocity = _pushStrength * Time.deltaTime * _pushDirection.Value;
            linearVelocity += pushVelocity;
        }

        #endregion

        #region Timers

        float _flipLoopTimer = 0;
        float _moveTimer = 0;

        private bool TryResetTimer(ref float timer, float duration)
        {
            if (timer > 0)
                return false;

            timer = duration;
            return true;
        }

        private void UpdateTimers()
        {
            _flipLoopTimer -= Time.deltaTime;
            _moveTimer -= Time.deltaTime;
        }

        #endregion

        #region World Interactions

        bool _applyReboundVelocity = false;
        Vector3? _pullTarget = null;
        float _pullStrength = 0f;
        Vector3? _pushDirection = null;
        float _pushStrength = 0f;

        internal void ApplyPullEffect(Vector3 position, float pullStrength)
        {
            _pullTarget = pullStrength <= 0 ? null : position;
            _pullStrength = pullStrength;
        }

        internal void ApplyPushEffect(Vector3 direction, float pushStrength)
        {
            _pushDirection = pushStrength <= 0 ? null : direction;
            _pushStrength = pushStrength;
        }

        internal void Teleport(Transform target, float forwardOffset)
        {
            _rigidBody.position = target.position + target.forward * forwardOffset;
            _rigidBody.rotation = Quaternion.Euler(target.forward);
            Physics.Simulate(1);
            CalculateOffset();
        }

        internal void Rebound()
        {
            if (_currState == EState.Looping)
                _isLoopingClockwise = !_isLoopingClockwise;
            _applyReboundVelocity = true;
        }

        #endregion
    }
}