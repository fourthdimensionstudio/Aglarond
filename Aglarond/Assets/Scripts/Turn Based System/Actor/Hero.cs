using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.TurnBased.Actor {
    public class Hero : DynamicActorComponent {
        // TODO movement denied clip
        // TODO input buffering time
        // TODO remember input time
        // TODO last input movement

        private Input.BaseInput m_playerInputSystem;
        private Input.EMovementDirection m_currentMovementDirection;

        private void Awake() {
            // TODO Mobile Input
            m_playerInputSystem = new Input.AxisInput();

            // TODO Subscribe to OnActorAttack the screenshake function
            // TODO Subscribe to OnActorDeath the player die function

            InitializeActor(EActorType.Player);
        }

        private void Update() {
            m_currentMovementDirection = m_playerInputSystem.TickInput();
        }

        public override bool HasTakenTurn() {
            if(m_currentMovementDirection != Input.EMovementDirection.NONE) {
                Move(m_currentMovementDirection, true, false);
                return true;
            } else {
                return false;
            }
        }
    }
}
