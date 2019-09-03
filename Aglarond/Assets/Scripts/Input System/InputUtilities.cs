using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Input {
    public static class InputUtilities {
        public static EMovementDirection GetMovementDirectionFromVector(Vector2 _input) {
            if (_input.x != 0) {
                if (_input.x > 0) {
                    return EMovementDirection.RIGHT;
                } else {
                    return EMovementDirection.LEFT;
                }
            } else if (_input.y != 0) {
                if (_input.y > 0) {
                    return EMovementDirection.UP;
                } else {
                    return EMovementDirection.DOWN;
                }
            }

            return EMovementDirection.NONE;
        }

        public static Vector2 GetMovementVectorFromDirection(EMovementDirection _direction) {
            switch(_direction) {
                case EMovementDirection.UP:
                    return Vector2.up;
                case EMovementDirection.RIGHT:
                    return Vector2.right;
                case EMovementDirection.DOWN:
                    return Vector2.down;    
                case EMovementDirection.LEFT:
                    return Vector2.left;
            }

            return Vector2.zero;
        }
    }
}
