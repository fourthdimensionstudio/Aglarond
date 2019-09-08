using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Dungeon {
    public enum ERegionType {
        Room,
        Maze
    }

    public abstract class RegionUnit {
        public ERegionType regionType;

        public RegionUnit(ERegionType _regionType) {
            regionType = _regionType;
        }

        public abstract bool IsPositionWithinRegionUnit(Vector3Int _position);
    }
}
