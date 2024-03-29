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
        [SerializeField]
        private GameObject prefab;
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


        public static GameObject GetObject(string poolName, Transform parent = null)
        {
            ObjectPool pool = objectPoolList[poolName];

            if (pool.poolingObjectQueue.Count > 0)
            {
                GameObject poolingObject = pool.poolingObjectQueue.Dequeue();
                poolingObject.transform.SetParent(parent == null ? pool.transform.parent : parent);
                poolingObject.SetActive(true);

                return poolingObject;
            }
            else
            {
                return Instantiate(pool.prefab, parent == null ? pool.transform.parent : parent);
            }
        }

        public static void ReturnObject(string poolName, GameObject poolingObject)
        {
            ObjectPool pool = objectPoolList[poolName];

            poolingObject.SetActive(false);
            poolingObject.transform.SetParent(pool.transform);
            pool.poolingObjectQueue.Enqueue(poolingObject);
        }
    }
}
