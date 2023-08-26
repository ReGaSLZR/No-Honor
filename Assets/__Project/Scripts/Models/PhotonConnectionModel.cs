using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ReGaSLZR
{

    public class PhotonConnectionModel : MonoBehaviourPunCallbacks,
        IConnectionModel.IGetter, IConnectionModel.ISetter

    {

        #region Inspector Fields

        //[SerializeField]
        //private string gameVersion;

        #endregion //Inspector Fields

        #region Private Fields

        private readonly ReactiveProperty<string> rMatchCode = new ReactiveProperty<string>();
        private readonly ReactiveProperty<bool> rIsHost = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> rIsConnected = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<List<PlayerModel>> rPlayers 
            = new ReactiveProperty<List<PlayerModel>>(new List<PlayerModel>());

        private bool isMatchNew = false;

        #endregion //Private Fields

        #region Unity Callbacks

        private void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        #endregion //Unity Callbacks

        #region Connection Model OVERRIDES

        public void Disconnect()
        {
            Debug.Log($"{GetType().Name} Disconnecting from match to restart from scratch. Ignore any disconnection error after this log!! :)");
            rPlayers.Value = new List<PlayerModel>();
            PhotonNetwork.Disconnect();
        }

        public void CreateAndJoinNewMatch(string playerName, string matchCode)
        {
            isMatchNew = true;
            ConnectToRoom(playerName, matchCode);
        }

        public void JoinExistingMatch(string playerName, string matchCode)
        {
            isMatchNew = false;
            ConnectToRoom(playerName, matchCode);
        }

        public IReadOnlyReactiveProperty<bool> IsHost() => rIsHost;
        public IReadOnlyReactiveProperty<string> GetMatchCode() => rMatchCode;
        public IReadOnlyReactiveProperty<bool> IsConnected() => rIsConnected;
        public IReadOnlyReactiveProperty<List<PlayerModel>> GetPlayersInMatch() => rPlayers;

        #endregion //Connection Model OVERRIDES

        private void ConnectToRoom(string playerName, string matchCode)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
            
            rMatchCode.Value = matchCode;
        }

        private void UpdatePlayers()
        {
            RefreshIsHostStatus();
            var players = PhotonNetwork.CurrentRoom.Players.Values;
            Debug.Log($"UpdatePlayers count is: {players.Count}");
            rPlayers.Value.Clear();
            rIsConnected.Value = PhotonNetwork.IsConnected;

            if (players.Count == 0)
            {
                return;
            }

            var list = new List<PlayerModel>();
            foreach (var playa in players)
            {
                list.Add(new PlayerModel(playa.NickName, playa.UserId,
                    playa.IsLocal, playa.IsMasterClient));
            }

            rPlayers.SetValueAndForceNotify(list);
        }

        private void RefreshIsHostStatus() => rIsHost.Value = PhotonNetwork.IsMasterClient;

        #region Photon Overrides

        public override void OnCreatedRoom()
        {
            Debug.Log($"OnCreatedRoom");

            //NOTE: No need for this. Photon makes the Player auto-join their newly created room.
            //PhotonNetwork.JoinRoom(rMatchCode.Value); 
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            //todo
            Debug.Log($"OnCreateRoomFailed '{rMatchCode.Value}' count is: {message}");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"OnJoinedRoom");
            UpdatePlayers();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            //todo
            Debug.Log($"OnJoinRoomFailed '{rMatchCode.Value}' count is: {message}");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            rMatchCode.Value = string.Empty;
            rIsConnected.SetValueAndForceNotify(false);
        }

        public override void OnConnectedToMaster()
        {
            rIsConnected.SetValueAndForceNotify(true);

            if (isMatchNew)
            {
                PhotonNetwork.CreateRoom(rMatchCode.Value);
            }
            else
            {
                PhotonNetwork.JoinRoom(rMatchCode.Value);
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
            => RefreshIsHostStatus();

        public override void OnPlayerEnteredRoom(Player newPlayer) => UpdatePlayers();

        public override void OnPlayerLeftRoom(Player otherPlayer) => UpdatePlayers();

        #endregion //Photon Overrides

    }

}