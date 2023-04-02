using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerUI : MonoBehaviour
{
    public Action<Color> OnColorUpdate;

    public Color Color { get => m_Color; }

    [SerializeField] private Slider m_RedSlider;
    [SerializeField] private Slider m_GreenSlider;
    [SerializeField] private Slider m_BlueSlider;
    [SerializeField] private TextMeshProUGUI m_RedSliderValue;
    [SerializeField] private TextMeshProUGUI m_GreenSliderValue;
    [SerializeField] private TextMeshProUGUI m_BlueSliderValue;
    [SerializeField] private Image m_ColorPreview;
    private Color m_Color;
    private float m_RedValue = 255;
    private float m_GreenValue = 255;
    private float m_BlueValue = 255;

    private const int MinValueColor = 0;
    private const int MaxValueColor = 255;

    private void Start()
    {
        m_RedSliderValue.text = m_RedSlider.value.ToString();
        m_GreenSliderValue.text = m_GreenSlider.value.ToString();
        m_BlueSliderValue.text = m_BlueSlider.value.ToString();

        m_RedSlider.onValueChanged.AddListener((float value) =>
        {
            m_RedSliderValue.text = value.ToString();
            m_RedValue = Mathf.InverseLerp(MinValueColor, MaxValueColor, value);
            UpdateColor();
        }
        );
        m_GreenSlider.onValueChanged.AddListener((float value) =>
        {
            m_GreenSliderValue.text = value.ToString();
            m_GreenValue = Mathf.InverseLerp(MinValueColor, MaxValueColor, value);
            UpdateColor();
        }
        );
        m_BlueSlider.onValueChanged.AddListener((float value) =>
        {
            m_BlueSliderValue.text = value.ToString();
            m_BlueValue = Mathf.InverseLerp(MinValueColor, MaxValueColor, value);
            UpdateColor();
        }
        );
        m_RedSlider.value = m_RedValue;
        m_GreenSlider.value = m_GreenValue;
        m_BlueSlider.value = m_BlueValue;
        UpdateColor();
    }

    public void UpdateColorNetwork(Color x)
    {
        m_ColorPreview.color = x;
        m_Color = x;
    }

    private void UpdateColor()
    {
        m_Color = new(m_RedValue, m_GreenValue, m_BlueValue);
        OnColorUpdate?.Invoke(m_Color);
    }
}
