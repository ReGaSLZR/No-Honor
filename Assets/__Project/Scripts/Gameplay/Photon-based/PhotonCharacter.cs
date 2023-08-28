using Photon.Pun;
using UnityEngine;

namespace ReGaSLZR
{

    public class PhotonCharacter : Character
    {

        #region Inspector Fields

        [Header("Photon Settings")]

        [SerializeField]
        private PhotonView photonView;

        #endregion //Inspector Fields

        #region Unity Callbacks

        private void Start()
        {
            if (!photonView.AmOwner)
            {
                SetUpAsNPC();
            }
        }

        #endregion //Unity Callbacks

    }

}