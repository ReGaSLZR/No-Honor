using Photon.Pun;
using UnityEngine;

namespace ReGaSLZR
{

    [RequireComponent(typeof(PhotonView))]
    public class PhotonPoolItem : MonoBehaviour
    {

        private PhotonView photonView;

        public PhotonView ViewPhoton => photonView;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            var master = FindObjectOfType<PhotonGameMaster>();
            if (master == null)
            {
                return;
            }

            master.RegisterCrate(this);
        }

    }

}