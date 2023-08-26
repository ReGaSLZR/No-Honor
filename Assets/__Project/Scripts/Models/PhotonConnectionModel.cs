using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReGaSLZR
{

    public class PhotonConnectionModel : MonoBehaviour,
        IConnectionModel.IGetter, IConnectionModel.ISetter

    {

        #region Inspector Fields

        [SerializeField]
        private string gameVersion;

        #endregion //Inspector Fields

        #region Unity Callbacks

        //private void Awake()
        //{
        //    PhotonNetwork.autoJoinLobby = false;
        //    PhotonNetwork.automaticallySyncScene = true;
        //    PhotonNetwork.ConnectUsingSettings(gameVersion);
        //}

        //#endregion //Unity Callbacks

        //#region Photon Overrides

        //public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        //{
            
        //}

        //public override void OnDisconnectedFromPhoton()
        //{
            
        //}

        //public override void OnJoinedRoom()
        //{
            
        //}

        //public override void OnConnectedToMaster()
        //{
             
        //}

        //override Onn

        #endregion //Photon Overrides

    }

}