using System.Collections;
using UnityEngine;

namespace ReGaSLZR
{

    public class CharacterSpawner : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private GameMaster master;

        [SerializeField]
        private SpawnPoints spawnPoints;

        [Space]

        [SerializeField]
        private Character prefabCharBrain;

        [Header("Settings")]

        [SerializeField]
        private uint botCount = 0;

        #endregion //Inspector Fields

        #region Unity Callbacks

        private void Start()
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
            var spawnPoint = spawnPoints.GetRandomSpawnPoint();
            var chara = Instantiate(prefabCharBrain, 
                spawnPoint.position, spawnPoint.rotation);
            master.RegisterCharacter(chara, isBot);
        }

        #endregion //Client Impl

    }

}