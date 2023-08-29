using Photon.Pun;
using UnityEngine;
using UniRx;

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
            if (!photonView.Owner.IsLocal)
            {
                SetUpAsNPC();
            }

            UpdatePhotonModelToLocal();

            stats.Model
                .Where(_ => photonView.Owner.IsLocal)
                .Subscribe(UpdatePhotonModelToCopies)
                .AddTo(this);

            SelfRegisterToGameMaster();
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private void SelfRegisterToGameMaster()
        {
            if (photonView.Owner.IsLocal)
            {
                return;
            }

            var master = FindObjectOfType<GameMaster>();
            if (master == null)
            {
                return;
            }

            master.RegisterCharacter(this, true);
        }

        private void UpdatePhotonModelToLocal(bool shouldAnimateDamageFX = false) 
            => stats.UpdateModel(photonView.Owner.GetPlayerModel(), shouldAnimateDamageFX);

        private void UpdatePhotonModelToCopies(PlayerModel model)
        {
            PhotonNetwork.SetPlayerCustomProperties(model.GetProperties());
            photonView.RPC("RPC_PlayerCopyPropertiesUpdate", 
                RpcTarget.Others, photonView.Owner.UserId);
        }

        [PunRPC]
        private void RPC_PlayerCopyPropertiesUpdate(string playerId)
        {
            Debug.Log($"RPC_PlayerCopyPropertiesUpdate()", gameObject);

            if (!playerId.Equals(photonView.Owner.UserId))
            {
                return;
            }

            UpdatePhotonModelToLocal(true);
        }

        #endregion //Client Impl

    }

}