using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class ObjectPool : MonoBehaviour
    {
        private static Dictionary<string, ObjectPool> objectPoolList = new();
        [SerializeField]
        private string poolName;
        private Queue<GameObject> poolingObjectQueue = new();

        private void Awake()
        {
            if (!objectPoolList.ContainsKey(poolName))
            {
                objectPoolList.Add(poolName, this);
            }
            else
            {
                objectPoolList[poolName] = this;
            }
        }


        public static GameObject GetObject(string poolName)
        {
            ObjectPool pool = objectPoolList[poolName];

            if (pool.poolingObjectQueue.Count <= 0) return null;

            GameObject poolingObject = pool.poolingObjectQueue.Dequeue();
            poolingObject.SetActive(true);
            poolingObject.transform.SetParent(pool.transform.parent);

            return poolingObject;
        }
        public static GameObject GetObject(string poolName, GameObject prefab)
        {
            ObjectPool pool = objectPoolList[poolName];

            if (pool.poolingObjectQueue.Count > 0)
            {
                return GetObject(poolName);
            }
            else
            {
                return Instantiate(prefab, pool.transform.parent);
            }
        }

        public static void ReturnObject(string poolName, GameObject poolingObject)
        {
            ObjectPool pool = objectPoolList[poolName];

            poolingObject.transform.SetParent(pool.transform);
            poolingObject.SetActive(false);
            pool.poolingObjectQueue.Enqueue(poolingObject);
        }
    }
}
