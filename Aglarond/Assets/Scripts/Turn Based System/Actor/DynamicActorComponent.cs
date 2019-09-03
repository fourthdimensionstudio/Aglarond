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

            StartCoroutine(MovementRoutine(_movementDirection, 0.1f));
        }

        private IEnumerator MovementRoutine(Input.EMovementDirection _movementDirection, float _timeToMove) {
            m_isActorCurrentlyMoving = true;
            Vector2 originalPosition = m_currentPosition;
            Vector2 movementDirection = Input.InputUtilities.GetMovementVectorFromDirection(_movementDirection);

            // Handling Scale Change here
            if(movementDirection.x != 0) {
                transform.localScale = new Vector3(Mathf.Sign(movementDirection.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            Vector2 midwayPoint = originalPosition + new Vector2(movementDirection.x / 2, movementDirection.y / 2 + 0.25f);
            Vector2 destinationPosition = originalPosition + movementDirection;
            // The Actor's current position is updated before the animation is set
            m_currentPosition = destinationPosition;

            // Beautifully Handling the movement
            Sequence movementSequence = DOTween.Sequence();
            movementSequence.Append(transform.DOMove(midwayPoint, 0.1f).SetEase(Ease.InOutQuint));
            movementSequence.Append(transform.DOMove(destinationPosition, 0.1f).SetEase(Ease.OutBack));
            movementSequence.onComplete += OnMovementRoutineFinished;
            movementSequence.Play();

            yield return null;
        }

        private void OnMovementRoutineFinished() {
            m_isActorCurrentlyMoving = false;
            transform.position = m_currentPosition;
        }
    }
}
