using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;



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
            }

            Handles.color = Gizmos.color = Color.red;
            Vector3 to = transform.position + _tangent;
            Gizmos.DrawSphere(to, .1f);
            Gizmos.DrawLine(transform.position, to);
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

            UpdateMovement();

            UpdateTimers();
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
        Vector3 _tangent;
        float _forwardMultiplier = 1;
        Vector3? _pullTarget = null;
        float _pullStrength = 0f;
        Vector3? _pushDirection = null;

        private void CalculateOffset()
        {
            Vector3 offsetDir = _isLoopingClockwise ? transform.right : -transform.right;
            _loopOffsetPoint = transform.position + offsetDir * _playerMovData.LoopOffset;
        }

        private void UpdateMovement()
        {
            switch (_currState)
            {
                case EState.Looping:
                    Vector3 centerToPlayer = transform.position - _loopOffsetPoint;
                    Vector3 desiredPos = _loopOffsetPoint + centerToPlayer.normalized * _playerMovData.LoopOffset;
                    transform.position = desiredPos;
                    Vector3 lhs = _isLoopingClockwise ? Vector3.up : Vector3.down;
                    _tangent = Vector3.Cross(lhs, centerToPlayer).normalized;
                    _rigidBody.linearVelocity = _tangent * _playerMovData.BaseLoopSpeed;
                    _rigidBody.rotation = Quaternion.LookRotation(_tangent);
                    break;
                case EState.Moving:
                    _rigidBody.position += _tangent * _playerMovData.BaseMoveSpeed;
                    _rigidBody.rotation = Quaternion.LookRotation(_tangent);
                    break;
            }
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

        internal void ApplyPullEffect(Vector3 position, float pullStrength)
        {
            _pullTarget = position;
            _pullStrength = pullStrength;
        }

        internal void ApplyPushEffect(Vector3 direction, float pushStrength)
        {
            _pushDirection =
                pushStrength <= 0
                ? null
                : direction * pushStrength;
        }

        internal void Teleport(Transform target, float forwardOffset)
        {
            transform.position = target.position + target.forward * forwardOffset;
            transform.forward = target.forward;
            if (_currState == EState.Moving)
                _tangent = target.forward;
            CalculateOffset();
        }

        internal void Rebound()
        {
            if (_currState == EState.Looping)
                _isLoopingClockwise = !_isLoopingClockwise;
            else
            {
                _rigidBody.linearVelocity -= _rigidBody.linearVelocity;
                _tangent = -_tangent;
            }
        }

        #endregion
    }
}