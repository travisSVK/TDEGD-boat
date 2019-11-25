using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoamGenerator : MonoBehaviour
{
    static private List<FoamGenerator> m_FoamGenerators = new List<FoamGenerator>();

    static public List<FoamGenerator> foamGenerators
    {
        get
        {
            return m_FoamGenerators;
        }
    }

    private void OnEnable()
    {
        m_FoamGenerators.Add(this);
    }

    private void OnDisable()
    {
        m_FoamGenerators.Remove(this);
    }
}
