/*
 * MIT License
 * Copyright (c) 2019 Fourth Dimension Studios
 * Code written by Guilherme de Oliveira
 */

using FourthDimension.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.TurnBased.Actor {
    public class ReactiveEnemy : DynamicActorComponent {
        private Hero m_heroReference;
        private BehaviorTree m_enemyBehavior;

        private void Awake() {
            InitializeActor(EActorType.Enemy);
            m_heroReference = FindObjectOfType<Hero>();

            m_enemyBehavior = new BehaviorTree(
                new BehaviorTreeBuilder()
                .Selector("Enemy Selector")
                    .Action("Move Towards Player", MoveTowardsPlayer)
                .End()
                .Build()
                );
        }

        public override bool HasTakenTurn() {
            m_enemyBehavior.Tick();
            return true;
        }

        private EReturnStatus MoveTowardsPlayer() {
            Input.EMovementDirection movement = Input.EMovementDirection.NONE;

            if(m_heroReference.CurrentPosition.x < m_currentPosition.x) {
                movement = Input.EMovementDirection.LEFT;
            } else if(m_heroReference.CurrentPosition.x > m_currentPosition.x) {
                movement = Input.EMovementDirection.RIGHT;
            } else if(m_heroReference.CurrentPosition.y < m_currentPosition.y) {
                movement = Input.EMovementDirection.DOWN;
            } else if(m_heroReference.CurrentPosition.y > m_currentPosition.y) {
                movement = Input.EMovementDirection.UP;
            }

            Move(movement);
            return EReturnStatus.SUCCESS;
        }
    }
}
