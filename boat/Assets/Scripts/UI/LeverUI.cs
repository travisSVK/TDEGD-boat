using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeverUI : MonoBehaviour
{
    [SerializeField] private RectTransform m_Knob;
    [SerializeField] private RectTransform m_MinPoint;
    [SerializeField] private RectTransform m_MaxPoint;
    [SerializeField] private RotatorUI m_Rotator;

    private float m_Progress = 0.0f;

    public float progress
    {
        get
        {
            return m_Progress;
        }

        set
        {
            m_Progress = Mathf.Clamp(value, -1.0f, 1.0f);
            m_Knob.anchoredPosition = Vector3.Lerp(m_MinPoint.anchoredPosition, m_MaxPoint.anchoredPosition, (m_Progress + 1.0f) / 2.0f);
            if (m_Rotator)
            {
                m_Rotator.scale = m_Progress;
            }
        }
    }
}
