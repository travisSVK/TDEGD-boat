using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatEngine : MonoBehaviour
{
    [SerializeField] private Transform m_ForcePoint = null;
    [SerializeField] private Transform m_propeller = null;
    [SerializeField] private AudioSource m_EngineSoundSource = null;
    [SerializeField] private float m_MaxPower = 10.0f;

    private float m_Thrust = 0.0f;
    private Rigidbody m_Rigidbody = null;
    private Water m_Water = null;

    public float thrust
    {
        get
        {
            return m_Thrust;
        }

        set
        {
            m_Thrust = value;
            m_EngineSoundSource.pitch = 1.0f + Mathf.Abs(m_Thrust);
        }
    }

    private void Awake()
    {
        m_Rigidbody = GetComponent(typeof(Rigidbody)) as Rigidbody;
        m_Water = FindObjectOfType(typeof(Water)) as Water;
    }

    private void FixedUpdate()
    {
        if (m_Water.IsUnderwater(m_ForcePoint.position))
        {
            Vector3 force = m_ForcePoint.right * m_MaxPower * m_Thrust;
            m_Rigidbody.AddForceAtPosition(force, m_ForcePoint.position);
        }
    }
}
