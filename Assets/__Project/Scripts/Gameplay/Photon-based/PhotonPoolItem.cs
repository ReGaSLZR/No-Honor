using Photon.Pun;
using UnityEngine;

namespace ReGaSLZR
{

    [RequireComponent(typeof(PhotonView))]
    public class PhotonPoolItem : MonoBehaviour
    {

        private void Awake()
        {
            var master = FindObjectOfType<PhotonGameMaster>();
            if (master == null)
            {
                return;
            }

            master.RegisterCrate(this);
        }

    }

}