using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectPositionUI : MonoBehaviour
{
    [SerializeField] private RectTransform m_Traget = null;

    private RectTransform m_RectTransform = null;

    private void Awake()
    {
        m_RectTransform = GetComponent(typeof(RectTransform)) as RectTransform;
    }

    private void LateUpdate()
    {
        if (m_Traget)
        {
            m_Traget.position = m_RectTransform.position;
        }
    }
}
