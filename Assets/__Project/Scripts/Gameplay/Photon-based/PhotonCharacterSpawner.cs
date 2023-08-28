using Photon.Pun;
using UnityEngine;

namespace ReGaSLZR
{

    public class PhotonCharacterSpawner : CharacterSpawner
    {

        #region Unity Callbacks

        

        #endregion //Unity Callbacks

        #region Protected Virtuals

        protected override Character InstantiateCharacter(Transform spawnPoint)
        {
            var obj = PhotonNetwork.Instantiate(prefabCharBrain.name, 
                spawnPoint.position, spawnPoint.rotation);

            return obj.GetComponent<Character>();
        }

        #endregion //Protected Virtuals

    }

}