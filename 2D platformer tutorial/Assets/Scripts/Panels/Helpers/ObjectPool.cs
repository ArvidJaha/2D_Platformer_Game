using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    //List of the objects to be pooled
    public List<GameObject> PrefabsForPool;

    //List of pooled objects
    private List<GameObject> _pooledObjects = new List<GameObject>();

    public GameObject GetObjectFromPool(string objectName)
    {
        var instance = _pooledObjects.FirstOrDefault(obj =>  obj.name == objectName);
        if (instance != null)
        {
            _pooledObjects.Remove(instance);
            instance.SetActive(true);
            return instance;
        }

        var prefab = PrefabsForPool.FirstOrDefault(obj => obj.name == objectName);
        if (prefab != null)
        {
            //create a new instance
            var newInstance = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            newInstance.name = objectName;
            return newInstance;
        }

        Debug.LogWarning("Object pool doesnt have a prefab for the object with name " +  objectName);
        return null;
    }

    public void poolObject(GameObject obj)
    {
        obj.SetActive(false);
        _pooledObjects.Add(obj);

    }
}
