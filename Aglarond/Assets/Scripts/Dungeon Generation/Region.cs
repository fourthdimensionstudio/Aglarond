using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Dungeon {
    public class Region {
        public int regionNumber;
        public List<RegionUnit> regionUnits;

        public Region(int _regionNumber) {
            regionNumber = _regionNumber;
            regionUnits = new List<RegionUnit>();
        }

        public void AddRegionUnitToRegion(RegionUnit _regionUnit) {
            regionUnits.Add(_regionUnit);
        }

        public bool IsThisUnitOnThisRegion(RegionUnit _regionUnit) {
            return regionUnits.Contains(_regionUnit);
        }

        public void IncorporateRegion(Region _region) {
            foreach(RegionUnit regionUnit in _region.regionUnits) {
                regionUnits.Add(regionUnit);
            }

            _region.InvalidateRegion();
        }

        public void InvalidateRegion() {
            regionNumber = -1;
            regionUnits.Clear();
        }
    }
}
