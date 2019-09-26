/*
 * MIT License
 * Copyright (c) 2019 Fourth Dimension Studios
 * Code written by Guilherme de Oliveira
 */

namespace FourthDimension.AI {
    public class Sequence : Composite {
        public Sequence(string _nodeName) : base(_nodeName) { }

        public override EReturnStatus Tick() {
            foreach(Behavior behavior in m_childrenBehaviors) {
                EReturnStatus childStatus = behavior.Tick();

                if(childStatus != EReturnStatus.SUCCESS) {
                    return childStatus;
                }
            }

            return EReturnStatus.SUCCESS;
        }
    }
}