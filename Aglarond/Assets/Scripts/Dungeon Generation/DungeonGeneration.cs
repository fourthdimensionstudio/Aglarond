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
        private const int km_amountAlloweedToExceedTilemap = 2;

        private List<Room> m_rooms;
        private int m_currentRegion = -1;

        private void Start() {
            m_rooms = new List<Room>();
            AddRooms();
            AddMaze();
        }

        // 1. Generate the Rooms
        private void AddRooms() {
            for(int i = 0; i < km_attemptsToPlaceRoom; i++) {
                // 1. Pick a random room size
                Vector2 roomPosition = new Vector2(Random.Range(0, km_stageWidth), Random.Range(0, km_stageHeight));
                int roomWidth = Random.Range(km_minRoomSize, km_maxRoomSize);
                if(roomWidth % 2 == 0) {
                    roomWidth++;
                }

                int roomHeight = Random.Range(km_minRoomSize, km_maxRoomSize);
                if(roomHeight % 2 == 0) {
                    roomHeight++;
                }
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
        private void AddMaze() {
            int tilemapWidth = carvedRooms.size.x;
            int tilemapHeight = carvedRooms.size.y;

            Vector3Int randomStartingPoint;
            do {
                randomStartingPoint = new Vector3Int(Random.Range(0, tilemapWidth), Random.Range(0, tilemapHeight), 0);
            } while (carvedRooms.GetTile(randomStartingPoint) != null);

            Debug.Log($"Random Starting Point: {randomStartingPoint}");
            Stack<Vector3Int> visitedPositions = new Stack<Vector3Int>();
            Vector3Int currentPoint = randomStartingPoint;
            visitedPositions.Push(currentPoint);
            carvedRooms.SetTile(currentPoint, roomTile);

            bool reachedEnd = false;
            int currentTries = 0;

            while(!reachedEnd && currentTries < 9999) {

                List<Vector3Int> possibleMoves = new List<Vector3Int> {
                    Vector3Int.left,
                    Vector3Int.up,
                    Vector3Int.right,
                    Vector3Int.down
                };

                while (possibleMoves.Count > 0) {
                    Vector3Int chosenMove = possibleMoves.RandomOrDefault();
                    
                    if(DoesTileHasAdjacents(currentPoint + chosenMove)) {
                        possibleMoves.Remove(chosenMove);
                    } else {
                        Vector3Int futurePoint = currentPoint + chosenMove;
                        if(futurePoint.x >= (tilemapWidth + km_amountAlloweedToExceedTilemap) ||
                            futurePoint.y >= (tilemapHeight + km_amountAlloweedToExceedTilemap) || 
                            futurePoint.x < (0 - km_amountAlloweedToExceedTilemap) || 
                            futurePoint.y < (0 - km_amountAlloweedToExceedTilemap)) {
                            Debug.Log("REJECTED!!");
                            possibleMoves.Remove(chosenMove);
                        } else {
                            currentPoint += chosenMove;
                            visitedPositions.Push(currentPoint);
                            carvedRooms.SetTile(currentPoint, roomTile);
                            break;
                        }
                    }
                }

                if(possibleMoves.Count == 0) {
                    if(visitedPositions.Count == 0) {
                        Debug.Log("Left organically.");
                        reachedEnd = true;
                    } else {
                        currentTries++;
                        currentPoint = visitedPositions.Pop();
                    }
                }
            }
        }

        private bool DoesTileHasAdjacents(Vector3Int _tilePosition) {
            int totalAdjacents = 0;
            if(carvedRooms.GetTile(_tilePosition + Vector3Int.left) != null) {
                totalAdjacents++;
            }

            if (carvedRooms.GetTile(_tilePosition + Vector3Int.up) != null) {
                totalAdjacents++;
            }

            if (carvedRooms.GetTile(_tilePosition + Vector3Int.right) != null) {
                totalAdjacents++;
            }

            if (carvedRooms.GetTile(_tilePosition + Vector3Int.down) != null) {
                totalAdjacents++;
            }

            return (totalAdjacents > 1);
        }
        // 3. Make the Connections
        // 4. Cleanup dead ends
    }
}
