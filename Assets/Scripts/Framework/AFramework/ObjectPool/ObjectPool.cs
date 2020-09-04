using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AFramework
{
    public class ObjectPool : MonoBehaviour
    {
        static Dictionary<string, ObjectPool> sPoolObjs = new Dictionary<string, ObjectPool>();
        PooledObject prefab;

        List<PooledObject> availableObjects = new List<PooledObject>();

        public static void UseCustomPool(PooledObject prefab, ObjectPool custompool)
        {
            string poolName = prefab.name + " Pool";
            sPoolObjs[poolName] = custompool;
            custompool.prefab = prefab;
            prefab.Pool = custompool;
        }

        public static void CachePool(PooledObject prefab, int cacheNum)
        {
            string poolName = prefab.name + " Pool";
            if (!sPoolObjs.ContainsKey(poolName))
            {
                GameObject obj = new GameObject(poolName);
                DontDestroyOnLoad(obj);
                ObjectPool pool = obj.AddComponent<ObjectPool>();
                pool.prefab = prefab;
                sPoolObjs[poolName] = pool;
                prefab.Pool = pool;
            }

            var poolObj = sPoolObjs[poolName];
            var poolTransform = poolObj.transform;
            var poolList = poolObj.availableObjects;
            while (poolList.Count < cacheNum)
            {
                PooledObject obj = Instantiate<PooledObject>(prefab);
                obj.transform.SetParent(poolTransform, false);
                obj.Pool = poolObj;
                poolObj.AddObject(obj);
            }
        }

        public static ObjectPool GetPool(PooledObject prefab)
        {
            string poolName = prefab.name + " Pool";
            if (!sPoolObjs.ContainsKey(poolName))
            {
                GameObject obj = new GameObject(poolName);
                DontDestroyOnLoad(obj);
                ObjectPool pool = obj.AddComponent<ObjectPool>();
                pool.prefab = prefab;
                sPoolObjs[poolName] = pool;
                prefab.Pool = pool;
            }

            return sPoolObjs[poolName];
        }

        public PooledObject GetObject()
        {
            PooledObject obj;
            int lastAvailableIndex = availableObjects.Count - 1;
            if (lastAvailableIndex >= 0)
            {
                obj = availableObjects[lastAvailableIndex];
                availableObjects.RemoveAt(lastAvailableIndex);
                obj.gameObject.SetActive(true);
            }
            else
            {
                obj = Instantiate<PooledObject>(prefab);
                obj.transform.SetParent(transform, false);
                obj.Pool = this;
            }
            return obj;
        }

        public void AddObject(PooledObject obj)
        {
            obj.transform.SetParent(this.transform);
            obj.gameObject.SetActive(false);
            availableObjects.Add(obj);
        }
    }
}

