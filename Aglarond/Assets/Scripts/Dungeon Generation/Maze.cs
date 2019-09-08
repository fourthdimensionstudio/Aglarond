using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Dungeon {
    public class Maze : RegionUnit  {
        public Vector3Int startingPoint;
        public List<Vector3Int> mazePoints;

        public Maze(Vector3Int _startingPoint) : base(ERegionType.Maze) {
            startingPoint = _startingPoint;
            mazePoints = new List<Vector3Int>();
            mazePoints.Add(_startingPoint);
        }

        public override bool IsPositionWithinRegionUnit(Vector3Int _position) {
            return mazePoints.Contains(_position);
        }
    }
}
