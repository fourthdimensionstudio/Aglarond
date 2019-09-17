using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Dungeon {
    [RequireComponent(typeof(SpriteRenderer))]
    public class DungeonTile : MonoBehaviour {
        [Header("Sprites for the Tile")]
        public Sprite totallyVisibleSprite;
        public Sprite partiallyVisibleSprite;
        public Sprite nonVisibleSprite;

        [Header("Colors for Sprite")]
        public Color totallyVisibleColor;
        public Color partiallyVisibleColor;
        public Color nonVisibleColor;

        [Header("Sprite Masks")]
        public string visibleSpriteMask = "Default";
        public string partiallyVisibleSpriteMask = "Default";
        public string nonVisibleSpriteMask = "Default";

        private bool m_isTileVisible;
        public bool IsVisible {
            get {
                return m_isTileVisible;
            }
            set {
                m_isTileVisible = value;
            }
        }

        private bool m_wasTileDiscovered;
        public bool WasTileDiscovered {
            get {
                return m_wasTileDiscovered;
            }
            set {
                m_wasTileDiscovered = value;
            }
        }

        private SpriteRenderer m_spriteRenderer;
        private bool m_isWall;
        public bool IsWall {
            get {
                return m_isWall;
            }
        }

        public void InitializeTile(bool _isWall = false) {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_isWall = _isWall;
            m_isTileVisible = false;
            m_wasTileDiscovered = false;
            UpdateSpriteRenderer(nonVisibleSprite, nonVisibleColor, nonVisibleSpriteMask);
        }

        public void UpdateTile() {
            if(m_isTileVisible) {
                UpdateSpriteRenderer(totallyVisibleSprite, totallyVisibleColor, visibleSpriteMask);
            } else if(m_wasTileDiscovered) {
                UpdateSpriteRenderer(partiallyVisibleSprite, partiallyVisibleColor, partiallyVisibleSpriteMask);
            } else {
                UpdateSpriteRenderer(nonVisibleSprite, nonVisibleColor, nonVisibleSpriteMask);
            }
        }

        private void UpdateSpriteRenderer(Sprite _sprite, Color _color, string _sortingLayer) {
            m_spriteRenderer.sprite = _sprite;
            m_spriteRenderer.color = _color;
            m_spriteRenderer.sortingLayerName = _sortingLayer;
        }
    }
}
