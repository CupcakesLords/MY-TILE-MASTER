using UnityEngine;
using System.Collections;

namespace AFramework
{
    public class PooledObject : MonoBehaviour
    {
        [System.NonSerialized]
        ObjectPool poolInstanceForPrefab;

        public T GetPooledInstance<T>() where T : PooledObject
        {
            if (!poolInstanceForPrefab)
            {
                poolInstanceForPrefab = ObjectPool.GetPool(this);
            }
            return (T)poolInstanceForPrefab.GetObject();
        }

        public ObjectPool Pool { get; set; }

        public virtual void ReturnToPool()
        {
            if (Pool)
            {
                Pool.AddObject(this);
            }
            else {
                Debug.Log("I die!");
                Destroy(gameObject);
            }
        }
    }
}