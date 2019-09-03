using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Input {
    public enum EMovementDirection {
        NONE,
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    public abstract class BaseInput {
        protected EMovementDirection m_movementDirection;
        public abstract EMovementDirection TickInput();
    }
}
