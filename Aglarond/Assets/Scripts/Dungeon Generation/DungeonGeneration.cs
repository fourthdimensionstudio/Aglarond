using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FourthDimension.Dungeon {
    public class DungeonGeneration : MonoBehaviour {
        [Header("Tilemaps")]
        public Tilemap carvedRooms;
        public Tile roomTile;

        // Configuration
        private const int km_stageWidth = 40;
        private const int km_stageHeight = 40;

        private const int km_attemptsToPlaceRoom = 200;
        private const float km_extraConnectorChance = 20;
        private const int km_roomExtraSize = 2;
        private const int km_minRoomSize = 3;
        private const int km_maxRoomSize = 6;

        private List<Room> m_rooms;
        private int m_currentRegion = -1;

        private void Start() {
            m_rooms = new List<Room>();
            AddRooms();
        }

        // 1. Generate the Rooms
        private void AddRooms() {
            for(int i = 0; i < km_attemptsToPlaceRoom; i++) {
                // 1. Pick a random room size
                Vector2 roomPosition = new Vector2(Random.Range(0, km_stageWidth), Random.Range(0, km_stageHeight));
                int roomWidth = Random.Range(km_minRoomSize, km_maxRoomSize);
                int roomHeight = Random.Range(km_minRoomSize, km_maxRoomSize);
                // TODO Avoid rooms that are even-sized
                // TODO Avoid rooms too rectangular
                Room roomToAdd = new Room(roomPosition, roomWidth, roomHeight);
                bool overlaps = false;

                foreach (Room room in m_rooms) {
                    if(roomToAdd.CollidesWith(room)) {
                        overlaps = true;
                        break;
                    }
                }

                if(overlaps) {
                    continue;
                }

                m_rooms.Add(roomToAdd);

                // Starting a new region
                m_currentRegion++;
                CarveRoom(roomToAdd);
            }

            Debug.Log($"Room Generation finished, {m_currentRegion} regions and {m_rooms.Count} rooms");
        }

        private void CarveRoom(Room _room) {
            for(int x = (int)_room.roomBoundaries.bottomLeft.x; x < _room.roomBoundaries.bottomRight.x; x++) {
                for(int y = (int)_room.roomBoundaries.bottomLeft.y; y < _room.roomBoundaries.topLeft.y; y++) {
                    carvedRooms.SetTile(new Vector3Int(x, y, 0), roomTile);
                }
            }
        }
        // 2. Generate the Mazes
        // 3. Make the Connections
        // 4. Cleanup dead ends
    }
}
