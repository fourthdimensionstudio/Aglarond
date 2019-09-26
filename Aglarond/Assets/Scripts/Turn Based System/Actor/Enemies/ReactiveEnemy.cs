/*
 * MIT License
 * Copyright (c) 2019 Fourth Dimension Studios
 * Code written by Guilherme de Oliveira
 */

using FourthDimension.AI;
using FourthDimension.Input;

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

            if(m_heroReference.CurrentPosition.x < m_currentPosition.x && CanMoveOnDirection(EMovementDirection.LEFT)) {
                movement = EMovementDirection.LEFT;
            } else if(m_heroReference.CurrentPosition.x > m_currentPosition.x && CanMoveOnDirection(EMovementDirection.RIGHT)) {
                movement = EMovementDirection.RIGHT;
            } else if(m_heroReference.CurrentPosition.y < m_currentPosition.y && CanMoveOnDirection(EMovementDirection.DOWN)) {
                movement = EMovementDirection.DOWN;
            } else if(m_heroReference.CurrentPosition.y > m_currentPosition.y && CanMoveOnDirection(EMovementDirection.UP)) {
                movement = EMovementDirection.UP;
            }

            Move(movement);
            return EReturnStatus.SUCCESS;
        }
    }
}
