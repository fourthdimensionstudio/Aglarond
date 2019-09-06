using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.TurnBased {
    public class TurnBasedSystemManager : MonoBehaviour {
        private Actor.DynamicActorComponent[] m_dynamicActorsOnScene;
        // TODO have a differentiation between dynamicActorsOnScene and TurnList

        // Turn Based Handling
        private int m_currentActorTurn = 0;

        // TODO Initialize System Function
        private void Awake() {
            m_dynamicActorsOnScene = FindObjectsOfType<Actor.DynamicActorComponent>();
            Debug.Log($"[TURN BASED SYSTEM MANAGER] Awaking TUrnBasedSystemManager - {m_dynamicActorsOnScene.Length} Dynamic Actors on Scene");
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
                m_currentActorTurn = ((m_currentActorTurn + 1) % m_dynamicActorsOnScene.Length);
            }
        }
        #endregion

    }
}
