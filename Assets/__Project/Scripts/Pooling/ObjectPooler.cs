using System.Collections.Generic;
using UnityEngine;

namespace ReGaSLZR
{

    public class ObjectPooler : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField] 
        private List<ObjectInPool> listObjectsInPool;

        [SerializeField]
        private Transform[] spawnPoints;

        #endregion //Inspector Fields

        private int itemIndex = 0;

        #region Unity Callbacks

        private void Awake()
        {
            DeactivateAllInPool();
        }

        private void OnDestroy()
        {
            listObjectsInPool.Clear();
        }

        #endregion //Unity Callbacks

        #region Public API

        public void RecycleInPool(int index)
        {
            GameObject poolItem = listObjectsInPool[index].gameObject;
            poolItem.transform.localPosition = Vector3.zero;
            poolItem.SetActive(false);
        }

        public GameObject GetObjectFromPool(Vector3 objectPosition,
            Quaternion objectRotation)
        {
            int checkCount = 0;

            while (listObjectsInPool[itemIndex].gameObject.activeInHierarchy)
            {
                MoveToNextIndex();
                checkCount++;

                if (checkCount >= listObjectsInPool.Count)
                {
                    Debug.LogWarning("GetObjectFromPool():" +
                        " No available pooled item. Returning null.");
                    return null;
                }
            }

            GameObject itemFromPool = listObjectsInPool[itemIndex].gameObject;
            itemFromPool.transform.SetPositionAndRotation(objectPosition, objectRotation);
            itemFromPool.SetActive(true);

            MoveToNextIndex();

            return itemFromPool;
        }

        [NaughtyAttributes.Button]
        public GameObject GetRandomObjectToRandomPosition()
        {
            var itemIndex = Random.Range(0, listObjectsInPool.Count);
            GameObject itemFromPool = listObjectsInPool[itemIndex].gameObject;

            var spawnIndex = Random.Range(0, spawnPoints.Length);
            itemFromPool.transform.SetPositionAndRotation(
                spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
            itemFromPool.SetActive(true);

            return itemFromPool;
        }

        #endregion //Public API

        #region Client Impl

        private void DeactivateAllInPool()
        {
            for (int x = (listObjectsInPool.Count - 1); x >= 0; x--)
            {
                var inPool = listObjectsInPool[x];

                if (inPool == null)
                {
                    listObjectsInPool.RemoveAt(x);
                }
                else
                {
                    inPool.SetUp(x, this);
                    inPool.gameObject.SetActive(false);
                }
            }
        }

        private void MoveToNextIndex()
        {
            itemIndex++;

            if (itemIndex >= listObjectsInPool.Count)
            {
                itemIndex = 0;
            }
        }

        #endregion //Client Impl

    }

}