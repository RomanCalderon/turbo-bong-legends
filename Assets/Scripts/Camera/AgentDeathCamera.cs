using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TPSBR
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(Health))]
    public class AgentDeathCamera : MonoBehaviour
    {
        [SerializeField] private float _lookSpeed = 10f;
        [SerializeField] private Vector3 _pushbackOffset = Vector3.up;
        [SerializeField] private float _pushBackSpeed = 10f;

        private Character _character = null;
        private Health _health = null;

        private Vector3 _defaultLocalRotation;
        private Vector3 _defaultLocalPosition;
        private Vector3 _targetLocalPosition;

        #region Initialization
        private void Awake()
        {
            _health = GetComponent<Health>();
            _character = GetComponent<Character>();
            Debug.Assert(_character != null);
            Debug.Assert(_health != null);
            _defaultLocalRotation = _character.ThirdPersonView.CameraTransformHead.localEulerAngles;
            _defaultLocalPosition = _character.ThirdPersonView.DefaultCameraTransform.localPosition;
        }
        private void OnEnable()
        {
            _health.onSpawned += _health_onSpawned;
            _health.onDeath += _health_onDeath;
        }
        private void OnDisable()
        {
            _health.onSpawned -= _health_onSpawned;
            _health.onDeath -= _health_onDeath;
        }
        private void OnDestroy()
        {
            _health.onSpawned -= _health_onSpawned;
            _health.onDeath -= _health_onDeath;
        }
        #endregion

        #region Event listeners
        private void _health_onSpawned()
        {
            // Reset orientation
            _character.ThirdPersonView.CameraTransformHead.localEulerAngles = _defaultLocalRotation;
            _character.ThirdPersonView.DefaultCameraTransform.localPosition = _defaultLocalPosition;
        }
        private void _health_onDeath()
        {
            _targetLocalPosition = _pushbackOffset;
        }
        #endregion

        // Update is called once per frame
        private void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateCameraPosition(deltaTime);
            UpdateLook(deltaTime);
        }
        private void UpdateCameraPosition(float deltaTime)
        {
            if (!_health.IsAlive)
            {
                _character.ThirdPersonView.DefaultCameraTransform.localPosition = Vector3.MoveTowards(
                    current: _character.ThirdPersonView.DefaultCameraTransform.localPosition,
                    target: _targetLocalPosition,
                    maxDistanceDelta: _pushBackSpeed * deltaTime);
            }
        }
        private void UpdateLook(float deltaTime)
        {
            if (!_health.IsAlive)
            {
                Vector3 mouseDelta = Mouse.current.delta.ReadValue();
                mouseDelta = new Vector3(-mouseDelta.y, mouseDelta.x, 0f);
                _character.ThirdPersonView.CameraTransformHead.localEulerAngles += _lookSpeed * deltaTime * mouseDelta;
            }
        }
    }
}
