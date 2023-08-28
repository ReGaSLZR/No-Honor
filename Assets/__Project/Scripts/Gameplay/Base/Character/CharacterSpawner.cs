using UnityEngine;

namespace ReGaSLZR
{

    public class CharacterSpawner : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        protected GameMaster master;

        [SerializeField]
        protected SpawnPoints spawnPoints;

        [Space]

        [SerializeField]
        protected Character prefabCharBrain;

        [Header("Settings")]

        [SerializeField]
        protected uint botCount = 0;

        #endregion //Inspector Fields

        #region Unity Callbacks

        protected virtual void Start()
        {
            SpawnNPCs();
            SpawnCharacter(false);
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private void SpawnNPCs()
        {
            uint botCount = 0;
            while(botCount < this.botCount)
            {
                SpawnCharacter(true);
                botCount++;
            }
        }

        private void SpawnCharacter(bool isBot)
        {
            var chara = InstantiateCharacter(spawnPoints.GetRandomSpawnPoint());
            master.RegisterCharacter(chara, isBot);
        }

        #endregion //Client Impl

        #region Protected Virtuals

        protected virtual Character InstantiateCharacter(Transform spawnPoint) =>
            Instantiate(prefabCharBrain, spawnPoint.position, spawnPoint.rotation);

        #endregion //Protected Virtuals

    }

}