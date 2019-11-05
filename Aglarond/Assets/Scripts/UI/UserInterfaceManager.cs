using TMPro;
using UnityEngine;

namespace FourthDimension.UI {
    public class UserInterfaceManager : MonoBehaviour {
        [Header("Heads Up Display")]
        public TextMeshProUGUI playerHealthText;

        private FeedbackText[] m_feedbackTexts;
        private Camera m_mainCameraReference;

        private void Awake() {
            m_feedbackTexts = GetComponentsInChildren<FeedbackText>();
            m_mainCameraReference = Camera.main;
            FindObjectOfType<TurnBased.Actor.Hero>().OnHeroHealthChanged += UpdateHealthTextOnScreen;
        }

        /// <summary>
        /// <para>Render a Feedback Text on Screen.</para>
        /// </summary>
        /// <param name="_worldPosition">World Position for the Feedback Text</param>
        /// <param name="_text">Actual text that will be rendered</param>
        /// <param name="_textColor">Text Color</param>
        public void RenderFeedbackText(Vector3 _worldPosition, string _text, Color _textColor) {
            foreach(FeedbackText feedbackText in m_feedbackTexts) {
                if(!feedbackText.IsBusy) {
                    feedbackText.ActivateText(m_mainCameraReference.WorldToScreenPoint(_worldPosition), _text, _textColor);
                    break;
                }
            }
        }

        /*
         * HUD
         */
        private void UpdateHealthTextOnScreen(float _currentHealth, float _maxHealth) {
            playerHealthText.text = $"HP: {_currentHealth}/{_maxHealth}";
        }

    }
}
