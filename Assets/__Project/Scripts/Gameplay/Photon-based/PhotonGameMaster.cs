using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReGaSLZR
{

    public class PhotonGameMaster : GameMaster
    {

        #region Inspector Fields

        [Header("Photon settings")]

        [SerializeField]
        private PhotonView viewPhoton;

        [SerializeField]
        private SpawnPoints spawnPoints;

        [SerializeField]
        private uint cratesInPool = 25;

        [SerializeField]
        private PhotonPoolItem[] prefabCrates;

        #endregion //Inspector Fields

        private readonly List<PhotonPoolItem> listCrates = new List<PhotonPoolItem>();
        private WaitForSeconds oneSec;

        #region Unity Callbacks

        protected override void Awake()
        {
            CheckPhotonReadiness();
            base.Awake();

            oneSec = new WaitForSeconds(1);
            itemSpawnDelay = new WaitForSeconds(itemSpawnInterval - 1);
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
            while (characters.Count > 0)
            {
                yield return oneSec;
                if (PhotonNetwork.IsMasterClient)
                {
                    DoSpawnPoolItems();
                }
                yield return itemSpawnDelay;
            }
        }

        protected override IEnumerator C_ClearItems()
        {
            yield return null;
        }

        #endregion //Protected Virtuals

        #region Client Impl

        [PunRPC]
        private void RPC_ShowPoolItems(int[] ids)
        {
            Debug.Log($"RPC_ShowPoolItems");
            var list = new List<int>();
            list.AddRange(ids);
            foreach (var item in listCrates)
            {
                item.gameObject.SetActive(list.Contains(item.ViewPhoton.ViewID));
            }
        }

        private void CheckPhotonReadiness()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                viewLoading.SetActive(true);
                SceneManager.LoadScene(sceneModel.SceneLobby);
            }
        }

        private void DoSpawnPoolItems()
        {
            var itemCount = 0;
            var ids = new List<int>();

            while (itemCount < itemSpawnCount)
            {
                var index = Random.Range(0, listCrates.Count);

                if (!ids.Contains(listCrates[index].ViewPhoton.ViewID))
                {
                    ids.Add(listCrates[index].ViewPhoton.ViewID);

                    listCrates[index].transform.position =
                        spawnPoints.GetRandomSpawnPoint().position;
                    itemCount++;
                }
            }

            viewPhoton.RPC("RPC_ShowPoolItems", RpcTarget.All, ids.ToArray());
        }

        private void SpawnPooledItems()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            uint spawned = 0;

            while (spawned < cratesInPool)
            {
                var index = 0;

                while (index < prefabCrates.Length)
                {
                    var item = PhotonNetwork.InstantiateRoomObject(
                        prefabCrates[index].name, Vector3.zero, Quaternion.identity);
                    item.gameObject.SetActive(false);
                    listCrates.Add(item.GetComponent<PhotonPoolItem>());
                    index++;
                }

                spawned++;
            }
        }

        #endregion //Client Impl

        public void RegisterCrate(PhotonPoolItem item) {
            listCrates.Add(item);
            item.gameObject.SetActive(false);
        }

    }

}