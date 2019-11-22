using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorUI : MonoBehaviour
{
    [SerializeField] private Vector3 m_Axis = Vector3.forward;
    [SerializeField] private float m_Rotation = 180.0f;
    [SerializeField] private float m_Scale = 1.0f;

    private RectTransform m_RectTransform = null;

    public float scale
    {
        get
        {
            return m_Scale;
        }

        set
        {
            m_Scale = value;
        }
    }

    void Awake()
    {
        m_RectTransform = GetComponent(typeof(RectTransform)) as RectTransform;
    }

    void Update()
    {
        m_RectTransform.Rotate(m_Axis, m_Rotation * m_Scale * Time.deltaTime);
    }
}
