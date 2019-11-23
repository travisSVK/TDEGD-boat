using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterObjectPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_Props = new List<GameObject>();

    private Water m_Water = null;

    private void Awake()
    {
        m_Water = FindObjectOfType(typeof(Water)) as Water;

        List<Vector2> locations = new List<Vector2>();
        for (int x = 1; x <= 50; ++x)
        {
            for (int z = 1; z < 10; ++z)
            {
                locations.Add(new Vector2(x, z));
            }
        }

        for (int i = 0; i < 50; ++i)
        {
            int j = Random.Range(0, locations.Count);
            Vector2 location = locations[j];
            locations.RemoveAt(j);
            float height = m_Water.GetFloorDepth(location.x, location.y);
            Vector3 finalPosition = new Vector3(location.x, height, location.y);
            Quaternion rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            Vector3 scale = Vector3.one * Random.Range(0.8f, 1.2f);
            GameObject obj = Instantiate(m_Props[Random.Range(0, m_Props.Count)], finalPosition, rotation);
            obj.transform.localScale = scale;
        }
    }
}
