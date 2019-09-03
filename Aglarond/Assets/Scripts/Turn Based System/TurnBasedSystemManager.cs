using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.TurnBased {
    public class TurnBasedSystemManager : MonoBehaviour {
        private Actor.Hero m_heroReference;

        // TODO Initialize System Function
        private void Awake() {
            m_heroReference = FindObjectOfType<Actor.Hero>();
        }

        private void Update() {
            ProcessCurrentActorTurn();
        }

        private void ProcessCurrentActorTurn() {
            // only have the hero for now...
            m_heroReference.HasTakenTurn();
        }

    }
}
