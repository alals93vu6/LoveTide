using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    // Singleton instance
    public static ObjectPoolManager Instance;

    // Default Prefab Object
    public List<GameObject> prefab = new List<GameObject>();

    // Dictionary to hold object pools
    private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();
    
    private void Start()
    {
        prefab.ForEach(delegate(GameObject o)
        {
            CreateObjectPool(o , 10);
        });
    }

    // Create object pool for a specific prefab
    public void CreateObjectPool(GameObject prefab, int initialSize)
    {
        if (!objectPools.ContainsKey(prefab))
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            objectPools.Add(prefab, objectPool);
        }
    }

    // Get object from object pool
    public GameObject GetObjectFromPool(GameObject prefab)
    {
        if (objectPools.ContainsKey(prefab))
        {
            Queue<GameObject> objectPool = objectPools[prefab];
            if (objectPool.Count > 0)
            {
                GameObject obj = objectPool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject obj = Instantiate(prefab, transform);
                return obj;
            }
        }
        else
        {
            Debug.LogWarning("Object pool for prefab " + prefab.name + " not found.");
            return null;
        }
    }

    // Return object to object pool
    public void ReturnObjectToPool(GameObject prefab, GameObject obj)
    {
        if (objectPools.ContainsKey(prefab))
        {
            obj.SetActive(false);
            objectPools[prefab].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning("Object pool for prefab " + prefab.name + " not found.");
        }
    }
}
