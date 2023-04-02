using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(AudioSource))]

    public class ClickButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {

        [SerializeField] private AudioClip m_PressSound;
        [SerializeField] private Sprite m_Pressed, m_Unpressed, m_Highlight;
        [SerializeField] private ButtonType m_ButtonType = ButtonType.Clicky;
        private bool isPressed = false;

        private enum ButtonType { Clicky, FullPush }

        private Image m_Image;


        public void OnPointerExit(PointerEventData eventData)
        {
            switch (m_ButtonType)
            {
                case ButtonType.Clicky:
                    UnHighliteButton();
                    break;
                case ButtonType.FullPush:
                    if (!isButtonPressed())
                    {
                        UnHighliteButton();
                    }
                    break;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            switch (m_ButtonType)
            {
                case ButtonType.Clicky:
                    ClickyButton();
                    break;
                case ButtonType.FullPush:
                    PressButton();
                    break;
            }
            isPressed = !isPressed;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            switch (m_ButtonType)
            {
                case ButtonType.Clicky:
                    HighliteButton();
                    break;
                case ButtonType.FullPush:
                    if (!isButtonPressed())
                    {
                        HighliteButton();
                    }
                    break;
            }
        }

        private void Start()
        {
            m_Image = GetComponent<Image>();

            if (m_Unpressed != null)
            {
                m_Image.sprite = m_Unpressed;
            }
            else
            {
                throw new Exception("Missing default sprite for button");
            }

            if (m_PressSound == null)
            {
                m_PressSound = Resources.Load<AudioClip>("UI/Sounds/ButtonClick");
            }
        }

        private void ClickyButton()
        {
            m_Image.sprite = m_Unpressed;
            SoundManager.Singletone.PlayUI(m_PressSound);
        }

        private void PressButton()
        {
            if (m_Pressed != null)
            {
                if (isButtonPressed())
                {
                    m_Image.sprite = m_Unpressed;
                    SoundManager.Singletone.PlayUI(m_PressSound);
                }
                else
                {
                    m_Image.sprite = m_Pressed;
                    SoundManager.Singletone.PlayUI(m_PressSound);
                }
            }
            else
            {
                throw new Exception("Wrong Type button. Use Clicky button");
            }
        }

        private void HighliteButton()
        {
            if (m_Highlight != null)
            {
                m_Image.sprite = m_Highlight;
            }
        }

        private void UnHighliteButton()
        {
            if (m_Highlight != null)
            {
                m_Image.sprite = m_Unpressed;
            }
        }

        private bool isButtonPressed()
        {
            return m_Image.sprite == m_Pressed;
        }
    }
}