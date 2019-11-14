using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEdgeMesh : MonoBehaviour
{
    private Mesh m_Mesh = null;

    void Start()
    {
        gameObject.AddComponent(typeof(MeshRenderer));
    }

    void Update()
    {
        
    }
}
