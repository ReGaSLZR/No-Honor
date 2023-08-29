using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReGaSLZR
{

    public class PhotonGameMaster : GameMaster
    {

        #region Inspector Fields

        [Header("Photon settings")]

        [SerializeField]
        private SpawnPoints spawnPoints;

        [SerializeField]
        private Vector3 poolItemHiddenPosition;

        [SerializeField]
        private uint cratesInPool = 25;

        [SerializeField]
        private PhotonPoolItem[] prefabCrates;

        #endregion //Inspector Fields

        private readonly List<PhotonPoolItem> listCrates = new List<PhotonPoolItem>();
        private int lastIndexInPool = 1;

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();
            SpawnPooledItems();
        }

        protected override void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            base.Start();
        }

        #endregion //Unity Callbacks

        #region Protected Virtuals

        protected override void DoExitGame()
        {
            PhotonNetwork.Disconnect();
            base.DoExitGame();
        }

        protected override IEnumerator C_SpawnItems()
        {
            //return base.C_SpawnItems();

            while (characters.Count > 0)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    DoSpawnPoolItems();
                }
                yield return itemSpawnDelay;
            }
        }

        protected override IEnumerator C_ClearItems()
        {
            //return base.C_ClearItems();

            while (characters.Count > 0)
            {
                yield return itemClearDelay;

                if (PhotonNetwork.IsMasterClient)
                {
                    DoClearItems();
                }
            }
        }

        #endregion //Protected Virtuals

        #region Client Impl

        private void DoClearItems()
        {
            foreach (var item in listCrates)
            {
                item.transform.position = poolItemHiddenPosition;
            }
        }

        private void DoSpawnPoolItems()
        {
            var itemCount = 0;
            while (itemCount < itemSpawnCount)
            {
                var index = Random.Range(0, listCrates.Count);

                if (index == lastIndexInPool)
                {
                    index = ((index + 1) >= listCrates.Count) ? 0 : (index + 1);
                }

                listCrates[index].transform.position =
                    spawnPoints.GetRandomSpawnPoint().position;
                lastIndexInPool = index;
                itemCount++;
            }
        }

        private void SpawnPooledItems()
        {
            uint spawned = 0;

            while (spawned < cratesInPool)
            {
                var index = 0;

                while(index < prefabCrates.Length)
                {
                    var item = PhotonNetwork.InstantiateRoomObject(
                        prefabCrates[index].name, poolItemHiddenPosition, Quaternion.identity);
                    listCrates.Add(item.GetComponent<PhotonPoolItem>());
                    index++;
                }
                
                spawned++;
            }
        }

        #endregion //Client Impl

        public void RegisterCrate(PhotonPoolItem item) => listCrates.Add(item);

    }

}