using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Dungeon {
    public class Connector {
        public Vector3Int connectorPosition;
        public List<Maze> connectedMazes;
        public List<Room> connectedRooms;

        public Connector(Vector3Int _connectorPosition) {
            connectorPosition = _connectorPosition;
            connectedMazes = new List<Maze>();
            connectedRooms = new List<Room>();
        }

        public void AddConnectedRegion(Maze _connectedMaze) {
            connectedMazes.Add(_connectedMaze);
        }

        public void AddConnectedRegion(Room _connectedRoom) {
            connectedRooms.Add(_connectedRoom);
        }

        public void ConnectAllRegions() {
            int lowestRegion = int.MaxValue;

            // Getting Lowest Region Value
            foreach(Maze maze in connectedMazes) {
                if(maze.region < lowestRegion) {
                    lowestRegion = maze.region;
                }
            }

            foreach(Room room in connectedRooms) {
                if(room.region < lowestRegion) {
                    lowestRegion = room.region;
                }
            }

            // Assigning Them
            foreach(Maze maze in connectedMazes) {
                maze.region = lowestRegion;
            }

            foreach(Room room in connectedRooms) {
                room.region = lowestRegion;
            }
        }

        public bool AreRegionsConnected() {
            List<int> regions = new List<int>();

            foreach(Maze maze in connectedMazes) {
                if(!regions.Contains(maze.region)) {
                    regions.Add(maze.region);
                }
            }

            foreach(Room room in connectedRooms) {
                if(!regions.Contains(room.region)) {
                    regions.Add(room.region);
                }
            }

            return regions.Count == 1;
        }
    }
}
