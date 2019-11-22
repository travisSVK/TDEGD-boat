using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatInput : MonoBehaviour
{
    [SerializeField] private string m_AccelerationAxisName = "";
    [SerializeField] private float m_ThrustZeroClampThreshold = 0.1f;

    private LeverUI m_LevelUI = null;
    private BoatEngine m_BoatEngine = null;
    private float m_Thrust = 0.0f;
    private float m_ClampedThrust = 0.0f;

    private void Awake()
    {
        m_BoatEngine = GetComponent(typeof(BoatEngine)) as BoatEngine;
        m_LevelUI = FindObjectOfType(typeof(LeverUI)) as LeverUI;
    }

    private void Update()
    {
        float input = Input.GetAxis(m_AccelerationAxisName);
        m_Thrust += input * Time.deltaTime;
        m_Thrust = Mathf.Clamp(m_Thrust, -1.0f, 1.0f);

        if (m_Thrust < m_ThrustZeroClampThreshold && m_Thrust > -m_ThrustZeroClampThreshold)
        {
            m_ClampedThrust = 0.0f;
        }
        else
        {
            m_ClampedThrust = m_Thrust;
        }

        if (m_LevelUI)
        {
            m_LevelUI.progress = m_ClampedThrust;
        }

        m_BoatEngine.Accelerate(input);
    }
}
