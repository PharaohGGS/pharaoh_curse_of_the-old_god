using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pharaoh.Tools
{
    public class HoverButton : Button
    {
        [Serializable]
        /// <summary>
        /// Function definition for a button click event.
        /// </summary>
        public class ButtonHoveredEvent : UnityEvent { }

        // Event delegates triggered on click.
        [FormerlySerializedAs("onHover")]
        [SerializeField]
        private ButtonHoveredEvent m_OnHover = new ButtonHoveredEvent();

        protected HoverButton()
        { }

        public ButtonHoveredEvent onHover
        {
            get { return m_OnHover; }
            set { m_OnHover = value; }
        }

        private void Hover()
        {
            if (!IsActive() || !IsInteractable()) return;

            UISystemProfilerApi.AddMarker("HoverButton.onHover", this);
            m_OnHover.Invoke();
        }

        public virtual void OnPointerHover(PointerEventData eventData)
        {
            if (eventData.hovered.Count <= 0) return;
            Hover();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            Hover();

            // if we get set disabled during the hover
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable()) return;

            DoStateTransition(SelectionState.Highlighted, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}