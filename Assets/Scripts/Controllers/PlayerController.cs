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
        [SerializeField] PlayerData _playerData;
        [SerializeField] Rigidbody _rigidBody;

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
            _wantsToMove = Input.GetKey(KeyCode.Mouse0);

            _wantsToFlipLooping = Input.GetKeyDown(KeyCode.Mouse1);
        }

        #endregion

        #region State Change

        EState _currState = (EState)(-1);

        private void UpdateState()
        {
            bool calculateOffset = false;

            // State Changing
            EState targetState = _currState;
            if (_wantsToMove)
            {
                if (targetState == EState.Moving) // Still moving
                    _moveTimer = _playerData.MoveCooldown;
                else if (TryResetTimer(ref _moveTimer, _playerData.MoveCooldown))
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

                if (TryResetTimer(ref _flipLoopTimer, _playerData.LoopDirFlipCooldown))
                {
                    _isLoopingClockwise = !_isLoopingClockwise;

                    calculateOffset = true;
                }
            }

            if (calculateOffset)
            {
                Vector3 offsetDir = _isLoopingClockwise ? transform.right : -transform.right;
                _loopOffsetPoint = transform.position + offsetDir * _playerData.LoopOffset;
            }
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

        private void UpdateMovement()
        {
            switch (_currState)
            {
                case EState.Looping:
                    Vector3 centerToPlayer = transform.position - _loopOffsetPoint;
                    Vector3 desiredPos = _loopOffsetPoint + centerToPlayer.normalized * _playerData.LoopOffset;
                    transform.position = desiredPos;
                    _tangent = Vector3.Cross(_isLoopingClockwise ? Vector3.up : Vector3.down, centerToPlayer).normalized;
                    _rigidBody.linearVelocity = _tangent * _playerData.BaseLoopSpeed;
                    _rigidBody.rotation = Quaternion.LookRotation(_tangent);
                    break;
                case EState.Moving:
                    _rigidBody.position += _tangent * _playerData.BaseMoveSpeed;
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
    }
}