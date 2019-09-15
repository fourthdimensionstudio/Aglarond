using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FourthDimension.Roguelike {
    public class ShadowLine {
        List<Shadow> m_shadows;

        public bool IsFullShadow {
            get {
                return m_shadows.Count == 1 && m_shadows[0].start == 0 && m_shadows[0].end == 1;
            }
        }

        public ShadowLine() {
            m_shadows = new List<Shadow>();
        }

        /// <summary>
        /// Add a new shadow to the Shadow Line.
        /// </summary>
        /// <param name="_shadowToAdd">Shadow to add.</param>
        public void AddShadowToLine(Shadow _shadowToAdd) {
            // First we have to figure out where to slot the new shadow in the list.
            int index = 0;
            for(; index < m_shadows.Count; index++) {
                // We stop when hit an intersetion point.
                if(m_shadows[index].start >= _shadowToAdd.start) {
                    break;
                }
            }

            // We know where the shadow is going to be, now we check if it overlaps the previous or next.
            Shadow previousOverlappedShadow = null;
            if(index > 0 && m_shadows[index - 1].end > _shadowToAdd.start) {
                previousOverlappedShadow = m_shadows[index - 1];
            }

            Shadow nextOverlappedShadow = null;
            if (index < m_shadows.Count && m_shadows[index].start < _shadowToAdd.end) {
                nextOverlappedShadow = m_shadows[index];
            }

            // Inserting the shadow and unifying with overlapping shadows.
            if(nextOverlappedShadow != null) {
                if(previousOverlappedShadow != null) {
                    // overlaps both, so unify one and delete the other
                    previousOverlappedShadow.end = nextOverlappedShadow.end;
                    m_shadows.RemoveAt(index);
                } else {
                    // overlaps the next one, so unify it with that.
                    nextOverlappedShadow.start = _shadowToAdd.start;
                }
            } else {
                if(previousOverlappedShadow != null) {
                    // Overlaps the previous one, so unify it with that
                    previousOverlappedShadow.end = _shadowToAdd.end;
                } else {
                    // Does not overlap anything, so insert.
                    m_shadows.Add(_shadowToAdd);
                }
            }
        }

        /// <summary>
        /// Verifying if any shadow in the line covers a tile.
        /// </summary>
        /// <param name="_projection">Shadow tile to be verified.</param>
        /// <returns>True if the tile is covered by this shadow, false otherwise.</returns>
        public bool IsInShadow(Shadow _projection) {
            foreach(Shadow shadow in m_shadows) {
                if(shadow.ContainsOther(_projection)) {
                    return true;
                }
            }

            return false;
        }
    }

    public class Shadow {
        public int start;
        public int end;

        public Shadow(int _start, int _end) {
            start = _start;
            end = _end;
        }

        /// <summary>
        /// Check if a shadow is completely covered by this shadow
        /// </summary>
        /// <param name="_other">Shadow tile to be checked</param>
        /// <returns>True if _other if completely covered by this shadow, false otherwise.</returns>
        public bool ContainsOther(Shadow _other) {
            return (start <= _other.start && end >= _other.end);
        }
    }

    public class FieldOfView : MonoBehaviour {
        [Header("TEMP for Debug")]
        public Tilemap fieldOfViewTilemap;
        public Tile fieldOfViewTile;

        public Tilemap dungeonTilemap;
        public Tilemap collisionTilemap;
        [Header("Field of View")]
        public int maxDistance = 5;

        public void InitializeFieldOfView(Vector2 _position) {
            RefreshVisibility(new Vector3Int((int)_position.x, (int)_position.y, 0));
        }

        private void RefreshVisibility(Vector3Int _originPosition) {
            fieldOfViewTilemap.ClearAllTiles();

            for(int octant = 0; octant < 8; octant++) {
                RefreshOctant(_originPosition, octant);
            }
        }

        private void RefreshOctant(Vector3Int _originPosition, int octant) {
            ShadowLine shadowLine = new ShadowLine();
            bool fullShadow = false;

            for(int row = 2; row < 3; row++) {
                // TODO should stop once we go out of bounds.
                Vector3Int position = _originPosition + CalculatePositionInOctant(row, 0, octant);

                /*
                if(position.y + row >= dungeonTilemap.size.y || position.y + row < 0) {
                    break;
                }
                */

                for(int col = 0; col <= row; col++) {
                    position = _originPosition + CalculatePositionInOctant(row, col, octant);
                    RoguelikeTile currentTile = dungeonTilemap.GetTile(position) as RoguelikeTile;

                    // TODO this is bad
                    if (currentTile == null) {
                        currentTile = collisionTilemap.GetTile(position) as RoguelikeTile;
                    }

                    if(currentTile == null) {
                        continue;
                    }

                    // If we went out of bounds, bail on this row
                    /*
                    if (position.x + col >= dungeonTilemap.size.x || position.x + col < 0) {
                        return;
                    }
                    */

                    if (fullShadow) {
                        // Tile is not visible
                        // currentTile.isVisible = false;
                        fieldOfViewTilemap.SetTile(position, fieldOfViewTile);
                    } else {
                        Shadow projection = ProjectTile(row, col);
                        bool isVisible = !shadowLine.IsInShadow(projection);
                        if(!isVisible) {
                            fieldOfViewTilemap.SetTile(position, fieldOfViewTile);
                        }
                        // currentTile.isVisible = isVisible;

                        if(isVisible && collisionTilemap.GetTile(position) != null) {
                            shadowLine.AddShadowToLine(projection);
                            fullShadow = shadowLine.IsFullShadow;
                        }
                    }
                    
                }
            }
        }

        private Vector3Int CalculatePositionInOctant(int _row, int _col, int _octant) {
            switch(_octant) {
                case 0:
                    return new Vector3Int(_col, -_row, 0);
                case 1:
                    return new Vector3Int(_row, -_col, 0);
                case 2:
                    return new Vector3Int(_row, _col, 0);
                case 3:
                    return new Vector3Int(_col, _row, 0);
                case 4:
                    return new Vector3Int(-_col, _row, 0);
                case 5:
                    return new Vector3Int(-_row, _col, 0);
                case 6:
                    return new Vector3Int(-_row, -_col, 0);
                case 7:
                    return new Vector3Int(-_col, -_row, 0);
            }

            return Vector3Int.zero;
        }

        private Shadow ProjectTile(int _row, int _col) {
            int topLeft = (_col / (_row + 2));
            int bottomRight = (_col + 1) / (_row + 1);
            return new Shadow(topLeft, bottomRight);
        }

    }
}
