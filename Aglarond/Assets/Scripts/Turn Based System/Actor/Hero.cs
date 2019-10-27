/*
 * MIT License
 * Copyright (c) 2019 Fourth Dimension Studios
 * Code written by Guilherme de Oliveira
 */

namespace FourthDimension.TurnBased.Actor {
    public class Hero : DynamicActorComponent {
        // TODO movement denied clip
        private Input.BaseInput m_playerInputSystem;
        private Input.EMovementDirection m_currentMovementDirection;
        private Roguelike.FieldOfView m_fieldOfView;

        private void Awake() {
            // DEBUGGING
            // InitializeHero();
        }

        public void InitializeHero() {
            // TODO Mobile Input
            // TODO Controller Input
            m_playerInputSystem = new Input.AxisInput();
            m_fieldOfView = GetComponent<Roguelike.FieldOfView>();
            InitializeActor(EActorType.Player);

            OnActorAttacked += (() => {
                ShakeScreen(.5f);
            });

            OnActorInteracted += (() => {
                ShakeScreen(.33f);
            });

            m_fieldOfView.InitializeFieldOfView(m_currentPosition);
        }

        private void Update() {
            m_currentMovementDirection = m_playerInputSystem.TickInput();
        }

        public override bool HasTakenTurn() {
            if(m_currentMovementDirection == Input.EMovementDirection.NONE) {
                return false;
            }

            Move(m_currentMovementDirection);
            m_fieldOfView.RefreshVisibility(m_currentPosition);
            return true;
        }
    }
}
