using UnityEngine;

namespace ReGaSLZR
{

    public class SpawnPoints : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private Transform[] spawnPoints;

        #endregion //Inspector Fields

        private int pastIndex = -1;

        #region Public API

        public Transform GetRandomSpawnPoint()
        {
            int newIndex;

            do
            {
                newIndex = Random.Range(0, spawnPoints.Length);
            } 
            while (newIndex == pastIndex);

            pastIndex = newIndex;
            return spawnPoints[newIndex];
        }

        #endregion //Public API

    }

}