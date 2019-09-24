using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FourthDimension.TurnBased {
    public class TurnBasedSystemManager : MonoBehaviour {
        // SINGLETON
        public static TurnBasedSystemManager instance;

        [Header("RNG Configuration")]
        public float damageRandomInterval = 0.2f;

        private List<Actor.DynamicActorComponent> m_dynamicActorsOnScene;
        // TODO have a differentiation between dynamicActorsOnScene and TurnList

        // Turn Based Handling
        private int m_currentActorTurn = 0;

        // TODO Initialize System Function
        private void Awake() {
            if(instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
            }

            m_dynamicActorsOnScene = FindObjectsOfType<Actor.DynamicActorComponent>().ToList();
            Debug.Log($"[TURN BASED SYSTEM MANAGER] Awaking TUrnBasedSystemManager - {m_dynamicActorsOnScene.Count} Dynamic Actors on Scene");
        }

        private void Update() {
            ProcessCurrentActorTurn();
        }

        // ------------------------------------------------------------------
        // ------------------------------------------------------------------
        //                  TURN BASED SYSTEM MANIPULATION
        // ------------------------------------------------------------------
        // ------------------------------------------------------------------
        #region MANIPULATION
        public void AddDynamicActorToScene(Actor.DynamicActorComponent _dynamicActor) {
            m_dynamicActorsOnScene.Add(_dynamicActor);
        }

        public void RemoveDynamicActorFromScene(Actor.DynamicActorComponent _dynamicActor) {
            m_dynamicActorsOnScene.Remove(_dynamicActor);

            m_currentActorTurn = (m_currentActorTurn % m_dynamicActorsOnScene.Count);
        }

        public bool IsThereAnActorAt(Vector2 _positionToCheck) {
            foreach(Actor.DynamicActorComponent dynamicActor in m_dynamicActorsOnScene) {
                if(dynamicActor.CurrentPosition == _positionToCheck) {
                    return true;
                }
            }

            return false;
        }

        public Actor.DynamicActorComponent WhatActorIsAtPosition(Vector2 _positionToCheck) {
            foreach(Actor.DynamicActorComponent dynamicActor in m_dynamicActorsOnScene) {
                if(dynamicActor.CurrentPosition == _positionToCheck) {
                    return dynamicActor;
                }
            }

            return null;
        }
        #endregion


        // ------------------------------------------------------------------
        // ------------------------------------------------------------------
        //                              UPDATE
        // ------------------------------------------------------------------
        // ------------------------------------------------------------------
        #region TURN BASED SYSTEM
        private void ProcessCurrentActorTurn() {
            if(m_dynamicActorsOnScene[m_currentActorTurn].HasTakenTurn()) {
                m_currentActorTurn = ((m_currentActorTurn + 1) % m_dynamicActorsOnScene.Count);
            }
        }
        #endregion

        #region COMBAT SYSTEM
        public void HandleCombat(Actor.DynamicActorComponent _actorDealingDamage, Actor.DynamicActorComponent _actorTakingDamage) {
            _actorDealingDamage.ActorDealtDamage();

            int damageDealt = CalculateDamage(_actorDealingDamage.actorStat.baseDamage, damageRandomInterval);
            _actorTakingDamage.ActorSufferedDamage(damageDealt);
            GameController.instance.RenderFeedbackText(_actorTakingDamage.CurrentPosition, damageDealt.ToString(), Color.red);
        }

        private int CalculateDamage(int _baseDamage, float _randomModifier) {
            return Random.Range(Mathf.RoundToInt(_baseDamage * (1 - _randomModifier)), Mathf.RoundToInt(_baseDamage * (1 + _randomModifier)) + 1);
        }
        #endregion

    }
}
