using UnityEngine;

namespace FourthDimension.Dungeon {
    public class RoomBoundaries {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;

        public RoomBoundaries(Vector2 _topLeft, Vector2 _topRight, Vector2 _bottomLeft, Vector2 _bottomRight) {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomLeft = _bottomLeft;
            bottomRight = _bottomRight;
        }
    }

    public class Room {
        /// <summary>
        /// Room Position is the bottom left position of the room
        /// </summary>
        public Vector2 roomPosition;
        public int width;
        public int height;
        public RoomBoundaries roomBoundaries;

        public Room(Vector2 _roomPosition, int _width, int _height) {
            roomPosition = _roomPosition;
            width = _width;
            height = _height;

            roomBoundaries = new RoomBoundaries(
                new Vector2(_roomPosition.x, _roomPosition.y + height),
                new Vector2(_roomPosition.x + _width, _roomPosition.y + _height),
                _roomPosition,
                new Vector2(_roomPosition.x + _width, _roomPosition.y)
                );
        }

        public bool CollidesWith(Room _other) {
            return (
                Mathf.Abs(roomPosition.x - _other.roomPosition.x) * 2 < (width + _other.width) &&
                Mathf.Abs(roomPosition.y - _other.roomPosition.y) * 2 < (height + _other.height)
                );
        }
    }
}
