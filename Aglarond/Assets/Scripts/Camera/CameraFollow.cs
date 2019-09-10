using FourthDimension.TurnBased.Actor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.CameraUtilities {
    public class CameraFollow : MonoBehaviour {
        private Hero m_playerReference;

        private void Start() {
            m_playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
        }

        private void Update() {
            transform.position = new Vector3(m_playerReference.CurrentPosition.x, m_playerReference.CurrentPosition.y, transform.position.z);
        }
    }
}
