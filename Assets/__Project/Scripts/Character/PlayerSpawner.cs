using UnityEngine;

namespace ReGaSLZR
{

    public class PlayerSpawner : MonoBehaviour
    {

        [SerializeField]
        private PlayerHUD hudView;

        [SerializeField]
        private Transform[] spawnPoints;

        [Space]

        [SerializeField]
        private CharacterBrain prefabCharBrain;

        #region Unity Callbacks

        private void Start()
        {
            SpawnPlayer();
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private void SpawnPlayer()
        {
            var index = Random.Range(0, spawnPoints.Length);
            var player = Instantiate(prefabCharBrain, 
                spawnPoints[index].position, spawnPoints[index].rotation);
            player.SetUpAsPlayer(hudView);
        }

        #endregion //Client Impl

    }

}