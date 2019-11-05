/*
 * MIT License
 * Copyright (c) 2019 Fourth Dimension Studios
 * Code written by Guilherme de Oliveira
 */


using System.Collections;
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

        [Header("Particle Effects")]
        public ParticleSystem actorHitParticles;

        // TODO Death?

        // Sprite Flashing Related
        private const float km_flashDelayTime = 0.1f;
        private SpriteRenderer m_actorSpriteRenderer;

        private bool m_isActorCurrentlyMoving = false;

        // EVENTS
        public event System.Action OnActorAttacked;
        public event System.Action OnActorSufferedDamage;
        public event System.Action OnActorInteracted;

        public override void InitializeActor(EActorType _actorType) {
            base.InitializeActor(_actorType);
            m_currentHealth = actorStat.maxHealth;

            m_actorSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        #region ENTRY POINTS
        public override bool HasTakenTurn() {
            return true;
        }

        /// <summary>
        /// <para>Tries to move the Dynamic Actor on the specified direction</para>
        /// </summary>
        /// <param name="_movementDirection">Direction which agent will atempt to move</param>
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
            DynamicActorComponent actorToInteractWith = TurnBasedSystemManager.instance.WhatActorIsAtPosition(positionToAttack);

            if(actorToInteractWith && actorToInteractWith.ActorType != m_actorType) {
                TurnBasedSystemManager.instance.HandleCombat(this, actorToInteractWith);
                return true;
            }

            return false;
        }

        protected bool CanMoveOnDirection(Input.EMovementDirection _direction) {
            return CanMoveOnDirection(Input.InputUtilities.GetMovementVectorFromDirection(_direction));
        }

        protected bool CanMoveOnDirection(Vector2 _movementDirection) {
            if (_movementDirection == Vector2.zero) {
                return false;
            }

            // TODO Handle Interfaces when player move to position ?
            Collider2D blockedCollision = Physics2D.OverlapCircle(this.m_currentPosition + _movementDirection, 0.05f, movementBlockedLayers);
            Collider2D triggerCollision = Physics2D.OverlapCircle(this.m_currentPosition + _movementDirection, 0.05f, triggersLayers);

            if (blockedCollision) {
                return false;
            } else if(triggerCollision) {
                IInteractAndNotMove interactAndNotMove = triggerCollision.GetComponent<IInteractAndNotMove>();

                interactAndNotMove?.Interact();
                ActorInteracted(.1f);
                OnActorInteracted?.Invoke();
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

        private void ActorInteracted(float _interactionTime) {
            InitializeMovementAndAction(Vector2.zero);

            Vector2 midwayPoint = m_currentPosition + (Vector2.up / 2.0f);
            Sequence interactionSequence = DOTween.Sequence();
            interactionSequence.Append(transform.DOMove(midwayPoint, _interactionTime / 2.0f).SetEase(Ease.InOutQuint));
            interactionSequence.Append(transform.DOMove(m_currentPosition, _interactionTime / 2.0f).SetEase(Ease.InOutQuint));
            interactionSequence.onComplete += OnMovementRoutineFinished;
            interactionSequence.Play();
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
        /// <summary>
        /// <para>Perform necessary logic when actor dealt damage</para>
        /// </summary>
        public void ActorDealtDamage() {
            PlaySoundEffect(actorAttackedSounds.RandomOrDefault());
            OnActorAttacked?.Invoke();
        }

        /// <summary>
        /// <para>Perform necessary logic when actor suffered damage</para>
        /// </summary>
        /// <param name="_damage">Amount of damage suffered</param>
        public virtual void ActorSufferedDamage(int _damage) {
            m_currentHealth -= _damage;

            if(actorHitParticles) {
                Instantiate(actorHitParticles, m_currentPosition, Quaternion.identity).Play();
            }

            if (m_currentHealth < 0) {
                Die();
            } else {
                if(m_actorSpriteRenderer != null) {
                    StartCoroutine(FlashRoutine());
                }

                PlaySoundEffect(actorDamagedSounds.RandomOrDefault());
                OnActorSufferedDamage?.Invoke();
            }
        }

        private IEnumerator FlashRoutine() {
            for(int i = 0; i < 2; i++) {
                m_actorSpriteRenderer.material.SetFloat("_FlashAmount", 1.0f);
                yield return new WaitForSeconds(km_flashDelayTime);
                m_actorSpriteRenderer.material.SetFloat("_FlashAmount", 0.0f);
                yield return new WaitForSeconds(km_flashDelayTime);
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
