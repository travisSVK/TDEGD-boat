using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPhysics : MonoBehaviour
{
    private float m_WaterDensity = 1027.0f;
    [SerializeField] private float m_BuoyancyCoeficient = 0.1f;
    [SerializeField] private float m_DistanceToSurfaceThreshold = 0.1f;
    [SerializeField] private float m_NormalThreshold = 0.0f;
    [SerializeField] private float m_Cpd1 = 1.0f;
    [SerializeField] private float m_Cpd2 = 1.0f;
    [SerializeField] private float m_Cps1 = 1.0f;
    [SerializeField] private float m_Cps2 = 1.0f;
    [SerializeField] private float m_Fp = 1.0f;
    [SerializeField] private float m_Fs = 1.0f;

    private BuoyancyMesh m_BuoyancyMesh;
    private Rigidbody m_RigidBody;

    private void Start()
    {
        m_RigidBody = GetComponent(typeof(Rigidbody)) as Rigidbody;
        m_BuoyancyMesh = GetComponent(typeof(BuoyancyMesh)) as BuoyancyMesh;
    }

    private void FixedUpdate()
    {
        float Cf = WaterMath.ResistanceCoefficient(m_WaterDensity, m_RigidBody.velocity.magnitude, m_BuoyancyMesh.GetLength());
        if (m_BuoyancyMesh.underwaterTriangles.Count > 0)
        {
            // Get all triangles
            List<Triangle> underWaterTriangles = m_BuoyancyMesh.underwaterTriangles;

            foreach (Triangle triangle in underWaterTriangles)
            {
                Vector3 appliedForce = WaterMath.CalculateBuoyancy(triangle, m_WaterDensity, m_DistanceToSurfaceThreshold, m_NormalThreshold, m_BuoyancyCoeficient);
                appliedForce += WaterMath.CalculateViscousWaterResistance(m_WaterDensity, triangle, Cf);
                Vector3 dragForce = WaterMath.CalculatePressureDrag(triangle, m_Cpd1, m_Cpd1, m_Cps1, m_Cps2, m_Fp, m_Fs);
                appliedForce += dragForce;

                // Add forces to boat
                m_RigidBody.AddForceAtPosition(appliedForce, triangle.center);
                
                // debug buoyancy
                Debug.DrawRay(triangle.center, dragForce * 2.0f, Color.red);
            }
        }
    }
}
