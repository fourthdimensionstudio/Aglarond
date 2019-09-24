using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FourthDimension.UI {
    public class FeedbackText : MonoBehaviour {
        private TextMeshProUGUI m_feedbackText;
        private RectTransform m_rectTransform;
        private const float km_timeToWait = 0.3f;
        private const float km_verticalOffset = 30f;

        private bool m_isBusy;
        public bool IsBusy {
            get {
                return m_isBusy;
            }
            set {
                m_isBusy = value;
            }
        }

        private void Start() {
            m_feedbackText = GetComponentInChildren<TextMeshProUGUI>();
            m_rectTransform = GetComponent<RectTransform>();
            m_isBusy = false;
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// <para>Activate the text and updates position, text content and color.</para>
        /// </summary>
        /// <param name="_position">Text position in screen position</param>
        /// <param name="_value">Text content</param>
        /// <param name="_color">Text color</param>
        public void ActivateText(Vector3 _position, string _value, Color _color) {
            m_rectTransform.position = _position;
            this.m_feedbackText.text = _value;
            this.m_feedbackText.color = _color;
            ActivateText();
        }

        /// <summary>
        /// <para>Activate the Feedback Text</para>
        /// </summary>
        public void ActivateText() {
            m_isBusy = true;
            this.gameObject.SetActive(true);

            Vector3 midwayPoint = m_rectTransform.position + (Vector3.up * km_verticalOffset);

            m_rectTransform.localScale = Vector3.one;

            Sequence feedbackTextSequence = DOTween.Sequence();
            feedbackTextSequence.Append(m_rectTransform.DOMove(midwayPoint, km_timeToWait / 2.0f).SetEase(Ease.InOutQuint));
            feedbackTextSequence.Append(m_rectTransform.DOScale(0f, km_timeToWait / 2.0f).SetEase(Ease.InOutQuint));
            feedbackTextSequence.onComplete += CompletedSequence;
            feedbackTextSequence.Play();
        }

        private void CompletedSequence() {
            m_isBusy = false;
            this.gameObject.SetActive(false);
        }
    }
}
