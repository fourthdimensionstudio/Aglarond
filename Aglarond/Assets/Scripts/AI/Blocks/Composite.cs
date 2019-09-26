/*
 * MIT License
 * Copyright (c) 2019 Fourth Dimension Studios
 * Code written by Guilherme de Oliveira
 */

using System.Collections.Generic;

namespace FourthDimension.AI {
    public abstract class Composite : Behavior {
        protected List<Behavior> m_childrenBehaviors = new List<Behavior>();

        public Composite(string _nodeName) : base(_nodeName) { }

        public void AddChildBehavior(Behavior _behavior) {
            m_childrenBehaviors.Add(_behavior);
        }
    }
}
