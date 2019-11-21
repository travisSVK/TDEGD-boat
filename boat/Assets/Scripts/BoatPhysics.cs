using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPhysics : MonoBehaviour
{
    private float m_WaterDensity = 1027.0f;
    [SerializeField] private float m_BuoyancyCoeficient = 0.1f;
    [SerializeField] private float m_DistanceToSurfaceThreshold = 0.1f;
    [SerializeField] private float normalThreshold = 0.0f;
    
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
                Vector3 appliedForce = WaterMath.CalculateBuoyancy(triangle, m_WaterDensity, m_DistanceToSurfaceThreshold, m_BuoyancyCoeficient);

                Vector3 resistance = WaterMath.CalculateViscousWaterResistance(m_WaterDensity, triangle, Cf);
                appliedForce += resistance;

                // Add forces to boat
                m_RigidBody.AddForceAtPosition(appliedForce, triangle.center);
                
                // debug buoyancy
                Debug.DrawRay(triangle.center, resistance * 0.05f, Color.red);
            }
        }
    }
}
