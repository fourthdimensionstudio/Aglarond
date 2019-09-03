using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Input {
    public class AxisInput : BaseInput {
        private const float km_holdingTimeToAct = 0.2f;
        private float m_holdingInputTime;

        public AxisInput() {
            m_movementDirection = EMovementDirection.NONE;
            m_holdingInputTime = 0f;
        }

        public override EMovementDirection TickInput() {
            int horizontalAxis = Mathf.CeilToInt(UnityEngine.Input.GetAxisRaw("Horizontal"));
            int verticalAxis = Mathf.CeilToInt(UnityEngine.Input.GetAxisRaw("Vertical"));

            // Calculating how many time the player has been holding the axis
            // This way the player can move by holding a key instead of having to press it every time
            if(horizontalAxis != 0 || verticalAxis != 0) {
                m_holdingInputTime += Time.deltaTime;
            }

            // Effectively Processing which movement has to be made
            if(
                // Player pressed something
                (UnityEngine.Input.GetButtonDown("Horizontal") || UnityEngine.Input.GetButtonDown("Vertical"))
                || // or has been holding for enough time
                m_holdingInputTime > km_holdingTimeToAct
                ) {
                m_holdingInputTime = 0f;

                // Horizontal Axis have movement priority over Vertical Axis
                // This cannot be handled in one line because it is needed to ensure player cannot move diagonally
                if(horizontalAxis != 0) {
                    m_movementDirection = InputUtilities.GetMovementDirectionFromVector(new Vector2(horizontalAxis, 0));
                } else {
                    m_movementDirection = InputUtilities.GetMovementDirectionFromVector(new Vector2(0, verticalAxis));
                }

            } else {
                m_movementDirection = EMovementDirection.NONE;
            }

            return m_movementDirection;
        }
    }
}
