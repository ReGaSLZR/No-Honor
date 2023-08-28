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
        private SpawnPoints spawnPoints;

        #endregion //Inspector Fields

        private int itemIndex = 0;

        #region Unity Callbacks

        private void Awake()
        {
            HideAllObjectsInPool();
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

        [NaughtyAttributes.Button] //TODO remove this later. Only for Editor quick play-test
        public GameObject GetRandomObjectToRandomPosition()
        {
            int newIndex;
            do 
            {
                newIndex = Random.Range(0, listObjectsInPool.Count);
            }
            while ((newIndex == itemIndex) 
                || (listObjectsInPool[newIndex].gameObject.activeInHierarchy) 
                || (listObjectsInPool[newIndex].gameObject.activeSelf));

            itemIndex = newIndex;
            GameObject itemFromPool = listObjectsInPool[newIndex].gameObject;

            var spawnPoint = spawnPoints.GetRandomSpawnPoint();
            itemFromPool.transform.SetPositionAndRotation(
                spawnPoint.position, spawnPoint.rotation);
            itemFromPool.SetActive(true);

            return itemFromPool;
        }

        public void HideAllObjectsInPool()
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

        #endregion //Public API

        #region Client Impl

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