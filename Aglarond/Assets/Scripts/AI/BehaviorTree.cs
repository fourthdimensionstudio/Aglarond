/*
 * MIT License
 * Copyright (c) 2019 Fourth Dimension Studios
 * Code written by Guilherme de Oliveira
 */

namespace FourthDimension.AI {
    public class BehaviorTree {
        protected Behavior m_treeRoot;

        public BehaviorTree(Behavior _treeRoot) {
            m_treeRoot = _treeRoot;
        }

        public void Tick() {
            if(m_treeRoot == null) {
                return;
            }

            m_treeRoot.Tick();
        }
    }
}
