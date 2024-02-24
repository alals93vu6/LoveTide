using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public class GameObjectPools : MonoBehaviour
{
    private Queue<GameObject> pools = new Queue<GameObject>();

    public GameObject prefab;

    public int objectCount = 10;

    public virtual void Start()
    {
        for (int i = 0; i < objectCount; i++)
        {
            var g = Instantiate(prefab , transform);
            
            g.SetActive(false);
            pools.Enqueue(g);
        }
    }
    
    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        pools.Enqueue(obj);
    }
    
    public GameObject GetGameObject(Transform _transform , Quaternion quaternion)
    {
        if (pools.Count != 0)
        {
            var g = pools.Dequeue();
            
            g.SetActive(true);
            g.transform.position = _transform.position;
            g.transform.rotation = quaternion;
            
            return g;
        }
        else
        {
            var g = Instantiate(prefab, transform);
            
            g.transform.position = _transform.position;
            g.transform.rotation = quaternion;
            pools.Enqueue(g);

            return pools.Dequeue();
        }
    }
    public GameObject GetGameObject(Transform _transform)
    {
        if (pools.Count != 0)
        {
            var g = pools.Dequeue();
            
            g.SetActive(true);
            g.transform.position = _transform.position;
            
            return g;
        }
        else
        {
            var g = Instantiate(prefab, transform);
            
            g.transform.position = _transform.position;
            pools.Enqueue(g);

            return pools.Dequeue();
        }
    }
    public GameObject GetGameObject()
    {
        if (pools.Count != 0)
        {
            var g = pools.Dequeue();
            g.SetActive(true);
            
            return g;
        }
        else
        {
            var g = Instantiate(prefab, transform);
            pools.Enqueue(g);

            return pools.Dequeue();
        }
    }
    
    
    
}
