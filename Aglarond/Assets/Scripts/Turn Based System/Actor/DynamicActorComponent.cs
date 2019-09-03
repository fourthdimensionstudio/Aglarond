using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FourthDimension.TurnBased.Actor {
    public class DynamicActorComponent : BaseActor {
        // TODO Stats?
        // TODO Collision Handling?
        // TODO Sounds?
        // TODO Particles?
        // TODO Death?
        // TODO Events OnActorAttacked, OnActorDeath, etc...
        // TODO Movement
        private bool m_isActorCurrentlyMoving = false;

        // TODO Implement Has Taken Turn
        public override bool HasTakenTurn() {
            return true;
        }

        public void Move(Input.EMovementDirection _movementDirection, bool _canMove, bool _hasActed) {
            if (m_isActorCurrentlyMoving) {
                return;
            }

            // TODO Handle Scale Changing here
            StartCoroutine(MovementRoutine(_movementDirection, 0.1f));
        }

        private IEnumerator MovementRoutine(Input.EMovementDirection _movementDirection, float _timeToMove) {
            m_isActorCurrentlyMoving = true;
            Vector2 originalPosition = m_currentPosition;
            Vector2 movementDirection = Input.InputUtilities.GetMovementVectorFromDirection(_movementDirection);
            Vector2 destinationPosition = originalPosition + movementDirection;

            // The Actor's current position is updated before the animation is set
            m_currentPosition = destinationPosition;
            
            yield return null;
            m_isActorCurrentlyMoving = false;
            transform.position = m_currentPosition;
        }
    }
}
