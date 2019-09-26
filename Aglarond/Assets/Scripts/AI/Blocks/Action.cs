/*
 * MIT License
 * Copyright (c) 2019 Fourth Dimension Studios
 * Code written by Guilherme de Oliveira
 */

namespace FourthDimension.AI {
    public class Action : Behavior {
        private readonly BehaviorTreeAction m_nodeBehavior;

        public Action(string _nodeName, BehaviorTreeAction _behaviorTreeAction) : base(_nodeName) {
            m_nodeBehavior = _behaviorTreeAction;
        }

        public override EReturnStatus Tick() {
            return m_nodeBehavior();
        }
    }
}

