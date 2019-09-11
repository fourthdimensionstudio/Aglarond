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

    public class Room : RegionUnit {
        /// <summary>
        /// Room Position is the bottom left position of the room
        /// </summary>
        public Vector2 roomPosition;
        public int width;
        public int height;
        public RoomBoundaries roomBoundaries;

        public Room(Vector2 _roomPosition, int _width, int _height) : base(ERegionType.Room) {
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
            // this room is on the left
            if(roomBoundaries.bottomLeft.x + width < _other.roomBoundaries.bottomLeft.x) {
                return false;
            }

            // this room is on the right
            if(_other.roomBoundaries.bottomLeft.x + _other.width < roomBoundaries.bottomLeft.x) {
                return false;
            }

            // this room is above
            if(roomBoundaries.bottomLeft.y > _other.roomBoundaries.topLeft.y) {
                return false;
            }

            // this room is below
            if(roomBoundaries.topLeft.y < _other.roomBoundaries.bottomLeft.y) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if a determined position is within this room
        /// </summary>
        /// <param name="_position">position to verified</param>
        /// <returns>whether or not the position is within the room</returns>
        public override bool IsPositionWithinRegionUnit(Vector3Int _position) {
            return (_position.x >= roomBoundaries.bottomLeft.x && _position.x <= roomBoundaries.bottomRight.x && _position.y <= roomBoundaries.topLeft.y && _position.y >= roomBoundaries.bottomLeft.y);
        }

        public Vector2 GetRoomCenter() {
            return new Vector2(roomPosition.x + width/2, roomPosition.y + height/2);
        }
    }
}
