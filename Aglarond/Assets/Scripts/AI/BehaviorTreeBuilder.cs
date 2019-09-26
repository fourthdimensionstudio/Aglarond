using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.AI {
    public class BehaviorTreeBuilder {
        private Behavior m_currentNode = null;
        private readonly Stack<Composite> m_parentNodes = new Stack<Composite>();

        public BehaviorTreeBuilder Sequence(string _nodeName) {
            Sequence sequence = new Sequence(_nodeName);

            if(m_parentNodes.Count > 0) {
                m_parentNodes.Peek().AddChildBehavior(sequence);
            }

            m_parentNodes.Push(sequence);
            return this;
        }

        public BehaviorTreeBuilder Selector(string _nodeName) {
            Selector selector = new Selector(_nodeName);

            if(m_parentNodes.Count > 0) {
                m_parentNodes.Peek().AddChildBehavior(selector);
            }

            m_parentNodes.Push(selector);
            return this;
        }

        public BehaviorTreeBuilder Action(string _nodeName, BehaviorTreeAction _action) {
            if(m_parentNodes.Count == 0) {
                return this;
            }

            Action action = new Action(_nodeName, _action);
            m_parentNodes.Peek().AddChildBehavior(action);
            return this;
        }

        public BehaviorTreeBuilder Condition(string _nodeName, BehaviorTreeAction _condition) {
            // Conditions are just synthatic sugar for actions that just check something...
            return Action(_nodeName, _condition);
        }

        public BehaviorTreeBuilder End() {
            m_currentNode = m_parentNodes.Pop();
            return this;
        }

        public Behavior Build() {
            if(m_currentNode == null) {
                throw new MissingComponentException("Cannot create a tree with zero nodes!");
            }
            return m_currentNode;
        }
    }
}
