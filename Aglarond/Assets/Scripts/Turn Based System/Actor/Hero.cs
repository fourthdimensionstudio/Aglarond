using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.TurnBased.Actor {
    public class Hero : DynamicActorComponent {
        // TODO movement denied clip

        private Input.BaseInput m_playerInputSystem;
        private Input.EMovementDirection m_currentMovementDirection;

        private void Awake() {
            // DEBUGGING
            // InitializeHero();
        }

        public void InitializeHero() {
            // TODO Mobile Input
            m_playerInputSystem = new Input.AxisInput();
            InitializeActor(EActorType.Player);

            onActorAttacked += (() => {
                ShakeScreen(.5f);
            });
        }

        private void Update() {
            m_currentMovementDirection = m_playerInputSystem.TickInput();
        }

        public override bool HasTakenTurn() {
            if(m_currentMovementDirection == Input.EMovementDirection.NONE) {
                return false;
            }

            Move(m_currentMovementDirection);
            return true;
        }
    }
}
