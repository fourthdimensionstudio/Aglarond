/*
 * MIT License
 * Copyright (c) 2019 Fourth Dimension Studios
 * Code written by Guilherme de Oliveira
 */

namespace FourthDimension.AI {
    public class Selector : Composite {
        public Selector(string _nodeName) : base(_nodeName) { }

        public override EReturnStatus Tick() {
            foreach(Behavior behavior in m_childrenBehaviors) {
                EReturnStatus childStatus = behavior.Tick();

                if(childStatus != EReturnStatus.FAILURE) {
                    return childStatus;
                }
            }

            return EReturnStatus.FAILURE;
        }
    }
}