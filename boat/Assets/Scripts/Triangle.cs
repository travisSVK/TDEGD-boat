using UnityEngine;
using System.Collections;

public class Triangle
{
    private Vector3 m_Point1;
    private Vector3 m_Point2;
    private Vector3 m_Point3;
    private Vector3 m_Center;
    private Vector3 m_Normal;
    private float m_DistanceToSurface;
    private float m_Area;

    public Vector3 normal
    {
        get 
        {
            return m_Normal;
        }
    }

    public Vector3 center
    {
        get
        {
            return m_Center;
        }
    }

    public float distanceToSurface
    {
        get 
        {
            return m_DistanceToSurface;
        }
    }

    public float area
    {
        get
        { 
            return m_Area; 
        }
    }

    public Triangle(Vector3 p1, Vector3 p2, Vector3 p3, Water water)
    {
        m_Point1 = p1;
        m_Point2 = p2;
        m_Point3 = p3;
        
        m_Center = (p1 + p2 + p3) / 3f;

        //Distance to the surface from the center of the triangle
        float waterHeight = water.GetHeight(m_Center.x, m_Center.z);
        m_DistanceToSurface = Mathf.Abs(waterHeight - m_Center.y);

        //Normal to the triangle
        m_Normal = Vector3.Cross(p2 - p1, p3 - p1).normalized;

        //Area of the triangle
        float a = Vector3.Distance(p1, p2);
        float c = Vector3.Distance(p3, p1);
        m_Area = (a * c * Mathf.Sin(Vector3.Angle(p2 - p1, p3 - p1) * Mathf.Deg2Rad)) / 2f;
    }
}
