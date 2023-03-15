using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI { 

public class SliderHandler : MonoBehaviour
{
    [SerializeField]
    private Slider m_Slider;
    [SerializeField]
    private TextMeshProUGUI m_TextMeshPro;

    private float m_CurrentNumber;

    private void Start()
    {
        m_Slider = GetComponent<Slider>();
        m_Slider.onValueChanged.AddListener(ChangeCurrentNumber);
        ChangeCurrentNumber(m_Slider.value);
    }

    private void ChangeCurrentNumber(float arg0)
    {
        if (m_Slider.wholeNumbers)
        {
            m_CurrentNumber = arg0;
        }
        else
        {
            m_CurrentNumber = arg0;
            m_CurrentNumber = (float)Math.Round(m_CurrentNumber, 2);
        }
        UpdateDislpay();
    }

    private void UpdateDislpay()
    {
        m_TextMeshPro.SetText(m_CurrentNumber.ToString());
    }
}
}