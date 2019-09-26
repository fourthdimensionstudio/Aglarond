/*
 * MIT License
 * Copyright (c) 2019 Fourth Dimension Studios
 * Code written by Guilherme de Oliveira
 */

namespace FourthDimension.AI {
    public enum EReturnStatus {
        SUCCESS,
        FAILURE,
        RUNNING
    }

    public delegate EReturnStatus BehaviorTreeAction();

    public abstract class Behavior {
        private string m_nodeName;

        public Behavior(string _nodeName) {
            m_nodeName = _nodeName;
        }

        public abstract EReturnStatus Tick();
    }
}
