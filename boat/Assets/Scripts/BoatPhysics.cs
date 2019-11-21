using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPhysics : MonoBehaviour
{
    private float m_WaterDensity = 1027.0f;
    [SerializeField] private float m_BuoyancyCoeficient = 0.1f;
    [SerializeField] private float m_DistanceToSurfaceThreshold = 0.1f;
    [SerializeField] private float normalThreshold = 0.0f;
    
    private BoatMesh m_BoatMesh;
    private Rigidbody m_RigidBody;

    private void Start()
    {
        m_RigidBody = GetComponent(typeof(Rigidbody)) as Rigidbody;
        m_BoatMesh = GetComponent(typeof(BoatMesh)) as BoatMesh;
    }

    private void FixedUpdate()
    {
        float Cf = Math.ResistanceCoefficient(m_WaterDensity, m_RigidBody.velocity.magnitude, m_BoatMesh.GetLength());
        if (m_BoatMesh.underWaterTriangles.Count > 0)
        {
            // Get all triangles
            List<Triangle> underWaterTriangles = m_BoatMesh.underWaterTriangles;

            foreach (Triangle triangle in underWaterTriangles)
            {
                Vector3 appliedForce = Math.CalculateBuoyancy(triangle, m_WaterDensity, m_DistanceToSurfaceThreshold, m_BuoyancyCoeficient);

                Vector3 resistance = Math.CalculateViscousWaterResistance(m_WaterDensity, triangle, Cf);
                appliedForce += resistance;

                // Add forces to boat
                m_RigidBody.AddForceAtPosition(appliedForce, triangle.center);
                
                // debug buoyancy
                Debug.DrawRay(triangle.center, resistance * 0.05f, Color.red);
            }
        }
    }
}
