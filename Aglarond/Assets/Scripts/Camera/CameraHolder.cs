using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.CameraUtilities {
    public enum EShakeStyle {
        HORIZONTAL,
        VERTICAL,
        DIRECTIONAL,
        ANGULAR,
        ALL,
    }

    public class CameraHolder : MonoBehaviour {
        // SINGLETON
        public static CameraHolder instance;

        [Header("Cameras on Scene")]
        public Camera mainCamera;
        public Camera screenshakeCamera;

        // Screenshake Configuration Constants
        private const float km_maxAngleShake = .5f;
        private const float km_maxOffsetShake = .5f;

        private float m_cameraTrauma;
        private EShakeStyle m_shakeStyle;
        private Coroutine m_shakeRoutine;

        private void Awake() {
            if(instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        private void Start() {
            m_shakeRoutine = null;
            m_cameraTrauma = 0f;
        }

        public void AddTraumaToCamera(float _amount, EShakeStyle _style) {
            m_shakeStyle = _style;
            AddTraumaToCamera(_amount);
        }

        private void AddTraumaToCamera(float _amount) {
            if(m_cameraTrauma <= 0) {
                // screenshakeCamera.transform.position = mainCamera.transform.position;
                // TODO ensure both cameras ortographic sizes are the same.
                screenshakeCamera.enabled = true;
                mainCamera.enabled = false;
                m_cameraTrauma = Mathf.Clamp01(_amount);
            } else {
                m_cameraTrauma = Mathf.Clamp01(m_cameraTrauma + _amount);
            }

            // If m_shakeRoutine is not null, Screenshake is already happening.
            // This way, adding to the camera trauma will make the screen shake more and for a longer time, that means we don't need to reinitialize the Coroutine.
            // Reinitializing the Coroutine would cause inumerous bad behaviors.
            if (m_shakeRoutine == null) {
                m_shakeRoutine = StartCoroutine(ShakeRoutine());
            }
        }

        private IEnumerator ShakeRoutine() {
            while(m_cameraTrauma > 0) {
                m_cameraTrauma -= Time.deltaTime;
                screenshakeCamera.transform.position = mainCamera.transform.position;

                float shakeAngle = km_maxAngleShake * (m_cameraTrauma * m_cameraTrauma) * Random.Range(-1f, 1f);
                float shakeOffsetX = km_maxOffsetShake * (m_cameraTrauma * m_cameraTrauma) * Random.Range(-1f, 1f);
                float shakeOffsetY = km_maxOffsetShake * (m_cameraTrauma * m_cameraTrauma) * Random.Range(-1f, 1f);
                ShakeWithStyle(new Vector2(shakeOffsetX, shakeOffsetY), shakeAngle);
                yield return null;
            }

            screenshakeCamera.enabled = false;
            mainCamera.enabled = true;
            m_shakeRoutine = null;
        }

        private void ShakeWithStyle(Vector2 _directionalOffset, float _angularOffset) {
            switch(m_shakeStyle) {
                case EShakeStyle.HORIZONTAL:
                    ShakeDirectional(new Vector2(_directionalOffset.x, 0));
                    break;
                case EShakeStyle.VERTICAL:
                    ShakeDirectional(new Vector2(0, _directionalOffset.y));
                    break;
                case EShakeStyle.ANGULAR:
                    ShakeRotational(_angularOffset);
                    break;
                case EShakeStyle.DIRECTIONAL:
                    ShakeDirectional(_directionalOffset);
                    break;
                case EShakeStyle.ALL:
                    ShakeDirectional(_directionalOffset);
                    ShakeRotational(_angularOffset);
                    break;
            }
        }

        private void ShakeDirectional(Vector2 _offset) {
            Vector3 newPosition = screenshakeCamera.transform.position;
            newPosition.x += _offset.x;
            newPosition.y += _offset.y;
            screenshakeCamera.transform.position = newPosition;
        }

        private void ShakeRotational(float _offset) {
            Vector3 newRotation = screenshakeCamera.transform.localEulerAngles;
            newRotation.z += _offset;
            screenshakeCamera.transform.localEulerAngles = newRotation;
        }
    }
}
