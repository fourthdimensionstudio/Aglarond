using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FourthDimension.TurnBased.Actor {
    public class DynamicActorComponent : BaseActor {
        [Header("Actor Stats")]
        public DynamicActorStats actorStat;
        protected int m_currentHealth;

        [Header("Collision Detection")]
        public LayerMask movementBlockedLayers;
        public LayerMask triggersLayers;

        [Header("Sound Effects")]
        public AudioClip[] actorAttackedSounds;
        public AudioClip[] actorDamagedSounds;
        public AudioClip[] actorDiedSounds;

        // TODO Particles?
        // TODO Death?
        // TODO Events OnActorAttacked, OnActorDeath, etc...
        private bool m_isActorCurrentlyMoving = false;

        // EVENTS
        public event System.Action onActorAttacked;
        public event System.Action onActorSufferedDamage;

        public override void InitializeActor(EActorType _actorType) {
            base.InitializeActor(_actorType);
            m_currentHealth = actorStat.maxHealth;
        }

        #region ENTRY POINTS
        public override bool HasTakenTurn() {
            return true;
        }

        public void Move(Input.EMovementDirection _movementDirection) {
            Vector2 movementDirection = Input.InputUtilities.GetMovementVectorFromDirection(_movementDirection);
            // Handling Combat
            bool willEngageInCombat = WillEngageOnCombatOnMovement(movementDirection);
            bool canMoveOnDirection = false;

            // Handling Movement
            if (!willEngageInCombat) {
                canMoveOnDirection = CanMoveOnDirection(movementDirection);
            }

            Move(movementDirection, canMoveOnDirection, willEngageInCombat);
        }
        #endregion

        private bool WillEngageOnCombatOnMovement(Vector2 _movementDirection) {
            Vector2 positionToAttack = m_currentPosition + _movementDirection;

            // SINGLETON REFERENCE
            if(TurnBasedSystemManager.instance.IsThereAnActorAt(positionToAttack)) {
                TurnBasedSystemManager.instance.HandleCombat(this, TurnBasedSystemManager.instance.WhatActorIsAtPosition(positionToAttack));
                return true;
            }

            return false;
        }

        protected bool CanMoveOnDirection(Vector2 _movementDirection) {
            if (_movementDirection == Vector2.zero) {
                return false;
            }

            // TODO Handle Interfaces when player move to position ?
            Collider2D blockedCollision = Physics2D.OverlapCircle(this.m_currentPosition + _movementDirection, 0.05f, movementBlockedLayers);

            if (blockedCollision) {
                return false;
            }

            return true;
        }

        #region MOVEMENT HANDLING

        private void Move(Vector2 _movementDirection, bool _canMove, bool _hasActed) {
            if (m_isActorCurrentlyMoving) {
                return;
            }

            if(_hasActed) {
                ActorActed(_movementDirection, 0.1f);
            } else if(_canMove) {
                ActorMoved(_movementDirection, 0.15f);
            }
        }

        private void ActorActed(Vector2 _directionWhichActed, float _actionTime) {
            InitializeMovementAndAction(_directionWhichActed);

            Sequence actionSequence = DOTween.Sequence();
            actionSequence.Append(transform.DOMove(m_currentPosition + _directionWhichActed, _actionTime / 2.0f).SetEase(Ease.InOutExpo));
            actionSequence.AppendInterval(0.1f);
            actionSequence.Append(transform.DOMove(m_currentPosition, _actionTime / 2.0f).SetEase(Ease.InOutExpo));
            actionSequence.onComplete += OnMovementRoutineFinished;
            actionSequence.Play();
        }

        private void ActorMoved(Vector2 _movementDirection, float _timeToMove) {
            InitializeMovementAndAction(_movementDirection);

            Vector2 midwayPoint = m_currentPosition + new Vector2(_movementDirection.x / 2, _movementDirection.y / 2 + 0.25f);
            Vector2 destinationPosition = m_currentPosition + _movementDirection;
            // The Actor's current position is updated before the animation is set
            m_currentPosition = destinationPosition;

            // Beautifully Handling the movement
            Sequence movementSequence = DOTween.Sequence();
            movementSequence.Append(transform.DOMove(midwayPoint, _timeToMove / 2.0f).SetEase(Ease.InOutQuint));
            movementSequence.Append(transform.DOMove(destinationPosition, _timeToMove / 2.0f).SetEase(Ease.OutBack));
            movementSequence.onComplete += OnMovementRoutineFinished;
            movementSequence.Play();
        }

        private void InitializeMovementAndAction(Vector2 _direction) {
            m_isActorCurrentlyMoving = true;

            // Handling Scale Change here
            if (_direction.x != 0) {
                transform.localScale = new Vector3(Mathf.Sign(_direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        private void OnMovementRoutineFinished() {
            m_isActorCurrentlyMoving = false;
            transform.position = m_currentPosition;
        }
        #endregion

        #region COMBAT HANDLING
        public void ActorDealtDamage() {
            PlaySoundEffect(actorAttackedSounds.RandomOrDefault());
            onActorAttacked?.Invoke();
        }

        public void ActorSufferedDamage(int _damage) {
            // TODO Particle Effects?
            m_currentHealth -= _damage;

            if (m_currentHealth < 0) {
                Die();
            } else {
                PlaySoundEffect(actorDamagedSounds.RandomOrDefault());
                onActorSufferedDamage?.Invoke();
            }
        }

        private void Die() {
            // TODO Particle Effects ?
            // TODO Instantiate a dead body ?
            PlaySoundEffect(actorDiedSounds.RandomOrDefault());
            TurnBasedSystemManager.instance.RemoveDynamicActorFromScene(this);
            Destroy(gameObject);
        }
        #endregion
    }
}
