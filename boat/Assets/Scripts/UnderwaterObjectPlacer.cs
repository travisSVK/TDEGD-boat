using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterObjectPlacer : MonoBehaviour
{
    [SerializeField] private int m_NumberOfObjects = 10;
    [SerializeField] private List<GameObject> m_Props = new List<GameObject>();

    private Water m_Water = null;

    private void Awake()
    {
        m_Water = FindObjectOfType(typeof(Water)) as Water;

        List<Vector2> locations = new List<Vector2>();
        for (int x = 0; x < m_Water.chunkLength; ++x)
        {
            for (int z = 1; z < m_Water.width -1; ++z)
            {
                locations.Add(new Vector2(transform.position.x + x + 0.5f, z + 0.5f));
            }
        }

        for (int i = 0; i < m_NumberOfObjects; ++i)
        {
            int j = Random.Range(0, locations.Count);
            Vector2 location = locations[j];
            locations.RemoveAt(j);
            float height = m_Water.GetFloorDepth(location.x, location.y);
            Vector3 finalPosition = new Vector3(location.x, height, location.y);
            Quaternion rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            Vector3 scale = Vector3.one * Random.Range(0.8f, 1.6f);
            GameObject obj = Instantiate(m_Props[Random.Range(0, m_Props.Count)], finalPosition, rotation, transform);
            obj.transform.localScale = scale;
        }
    }
}
