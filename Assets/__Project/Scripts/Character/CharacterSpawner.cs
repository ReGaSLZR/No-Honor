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
        private Transform[] spawnPoints;

        [Space]

        [SerializeField]
        private Character prefabCharBrain;

        [Header("Settings")]

        [SerializeField]
        private uint botCount = 0;

        #endregion //Inspector Fields

        #region Unity Callbacks

        private IEnumerator Start()
        {
            yield return null;
            yield return null;
            yield return null;

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
            var index = Random.Range(0, spawnPoints.Length);
            var chara = Instantiate(prefabCharBrain, 
                spawnPoints[index].position, spawnPoints[index].rotation);
            master.RegisterCharacter(chara, isBot);
        }

        #endregion //Client Impl

    }

}