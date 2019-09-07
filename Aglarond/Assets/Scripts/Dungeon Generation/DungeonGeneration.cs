using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FourthDimension.Dungeon {
    public class DungeonGeneration : MonoBehaviour {
        [Header("Tilemaps")]
        public Tilemap carvedRooms;
        public Tile roomTile;
        public Tile mazeTile;

        //---------------------------------------- Configuration
        // TODO Make a ScriptableObject with the config variables
        private const int km_stageWidth = 40;
        private const int km_stageHeight = 40;

        private const int km_attemptsToPlaceRoom = 200;
        private const float km_extraConnectorChance = 20;
        private const int km_roomExtraSize = 2;
        private const int km_minRoomSize = 3;
        private const int km_maxRoomSize = 6;
        private const int km_amountAlloweedToExceedTilemap = 0;
        // -----------------------------------------------------

        private List<Room> m_rooms;
        private int m_currentRegion = -1;

        private List<Vector3Int> m_cardinalMoves = new List<Vector3Int> {
            Vector3Int.left,
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down
        };

        private List<Vector3Int> m_diagonalMoves = new List<Vector3Int> {
            Vector3Int.left + Vector3Int.up,
            Vector3Int.up + Vector3Int.right,
            Vector3Int.right + Vector3Int.down,
            Vector3Int.down + Vector3Int.left
        };

        private List<Vector3Int> m_allMovesWithDiagonals = new List<Vector3Int> {
            Vector3Int.left,
            Vector3Int.left + Vector3Int.up,
            Vector3Int.up,
            Vector3Int.up + Vector3Int.right,
            Vector3Int.right,
            Vector3Int.right + Vector3Int.down,
            Vector3Int.down,
            Vector3Int.down + Vector3Int.left
        };

        private void Start() {
            m_rooms = new List<Room>();
            AddRooms();
            AddMaze();
        }

        #region ROOM GENERATION
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
        #endregion

        // 2. Generate the Mazes
        private void AddMaze() {
            int tilemapWidth = carvedRooms.size.x;
            int tilemapHeight = carvedRooms.size.y;
            Vector3Int startingPoint;

            for(int x = 0; x < tilemapWidth; x++) {
                for (int y = 0; y < tilemapHeight; y++) {
                    startingPoint = new Vector3Int(x, y, 0);
                    if (carvedRooms.GetTile(startingPoint) == null && !DoesSingleTileHasAdjacents(startingPoint)) {
                        StartMazeOnPoint(startingPoint);
                    }
                }
            }
        }

        private void StartMazeOnPoint(Vector3Int _point) {
            Debug.Log("Starting Maze");

            int tilemapWidth = carvedRooms.size.x;
            int tilemapHeight = carvedRooms.size.y;

            Stack<Vector3Int> visitedPositions = new Stack<Vector3Int>();
            Vector3Int currentPoint = _point;
            visitedPositions.Push(currentPoint);
            carvedRooms.SetTile(currentPoint, mazeTile);

            bool reachedEnd = false;
            int currentTries = 0;

            while (!reachedEnd && currentTries < 1999) {
                currentTries++;

                List<Vector3Int> possibleMoves = new List<Vector3Int> {
                    Vector3Int.left,
                    Vector3Int.up,
                    Vector3Int.right,
                    Vector3Int.down
                };

                while (possibleMoves.Count > 0) {
                    Vector3Int chosenMove = possibleMoves.RandomOrDefault();

                    if (DoesTileHasRoomAdjacents(currentPoint, currentPoint + chosenMove)) {
                        possibleMoves.Remove(chosenMove);
                    } else {
                        Vector3Int futurePoint = currentPoint + chosenMove;
                        if (futurePoint.x >= (tilemapWidth + km_amountAlloweedToExceedTilemap) ||
                            futurePoint.y >= (tilemapHeight + km_amountAlloweedToExceedTilemap) ||
                            futurePoint.x < (0 - km_amountAlloweedToExceedTilemap) ||
                            futurePoint.y < (0 - km_amountAlloweedToExceedTilemap)) {
                            // Debug.Log("REJECTED!!");
                            possibleMoves.Remove(chosenMove);
                        } else {
                            currentPoint += chosenMove;
                            visitedPositions.Push(currentPoint);
                            carvedRooms.SetTile(currentPoint, mazeTile);
                            break;
                        }
                    }
                }

                if (possibleMoves.Count == 0) {
                    if (visitedPositions.Count == 0) {
                        Debug.Log($"Left Organically");
                        reachedEnd = true;
                    } else {
                        currentPoint = visitedPositions.Pop();
                    }
                }
            }
        }

        /// <summary>
        /// Check all tiles around a tile for adjacents.
        /// </summary>
        /// <param name="_tilePosition">Point to check for adjacents</param>
        /// <returns>True if the tile has 1 or more neighbours</returns>
        private bool DoesSingleTileHasAdjacents(Vector3Int _tilePosition) {
            int totalAdjacents = 0;

            foreach (Vector3Int movement in m_allMovesWithDiagonals) {
                if (carvedRooms.GetTile(_tilePosition + movement) != null) {
                    totalAdjacents++;
                }
            }

            // Debug.Log($"[Does Single Tile Has Adjacents] {totalAdjacents} total adjacents");
            return (totalAdjacents > 0);
        }

        /// <summary>
        /// Check all room tiles around a tile for adjacents, ignoring the origin point
        /// </summary>
        /// <param name="_currentPoint">Current point that the algorithm is on</param>
        /// <param name="_futurePoint">Point that the algorithm is trying to go</param>
        /// <returns>Returns true if the tile has 1 or more room neighbours</returns>
        private bool DoesTileHasRoomAdjacents(Vector3Int _currentPoint, Vector3Int _futurePoint) {
            int totalAdjacents = 0;

            // Checking all adjacents for the tile
            foreach (Vector3Int movement in m_allMovesWithDiagonals) {
                if(carvedRooms.GetTile(_futurePoint + movement) != null &&
                    _futurePoint + movement != _currentPoint) {
                    totalAdjacents++;
                }
            }

            // Then we check on diagonals for mazeTiles and subtract them from the adjacents
            foreach(Vector3Int diagonalMove in m_diagonalMoves) {
                if(carvedRooms.GetTile(_futurePoint + diagonalMove) == mazeTile) {
                    totalAdjacents--;
                }
            }

            // Debug.Log($"[Does Tile Has Room Adjacents] {totalAdjacents} total adjacents");
            return (totalAdjacents > 0);
        }

        // 3. Make the Connections
        // 4. Cleanup dead ends
    }
}
