using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReGaSLZR
{

    public class PhotonConnectionModel : MonoBehaviourPunCallbacks,
        IConnectionModel.IGetter, IConnectionModel.ISetter

    {

        #region Private Fields

        private readonly ReactiveProperty<string> rMatchCode = new ReactiveProperty<string>();
        private readonly ReactiveProperty<string> rConnectionIssue = new ReactiveProperty<string>();

        private readonly ReactiveProperty<MatchModel> rMatchModel = new ReactiveProperty<MatchModel>(new MatchModel());

        private readonly ReactiveProperty<bool> rIsHost = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> rIsConnected = new ReactiveProperty<bool>();

        private readonly ReactiveProperty<List<PlayerModel>> rPlayers 
            = new ReactiveProperty<List<PlayerModel>>(new List<PlayerModel>());

        private bool isMatchNew = false;

        #endregion //Private Fields

        #region Unity Callbacks

        private void Start() => PhotonNetwork.AutomaticallySyncScene = true;

        #endregion //Unity Callbacks

        #region Connection Model OVERRIDES

        public void Disconnect()
        {
            Debug.Log($"{GetType().Name} Disconnecting from match to restart from scratch." + $" Ignore any potential disconnection error log after this log!! :)");
            rPlayers.Value = new List<PlayerModel>();
            PhotonNetwork.Disconnect();
        }

        public void CreateAndJoinNewMatch(string playerName, string matchCode)
        {
            isMatchNew = true;
            EstablishConnection(playerName, matchCode);
        }

        public void JoinExistingMatch(string playerName, string matchCode)
        {
            isMatchNew = false;
            EstablishConnection(playerName, matchCode);
        }

        public void SynchLoadScene(string scene) => PhotonNetwork.LoadLevel(scene);
        public void SetRoomProperty(MatchProperty key, object value)
        {  
            if (!rIsHost.Value)
            {
                Debug.Log($"{GetType().Name}.SetSceneLoadingStatus() not allowed because localPlayer is NOT the Host.");
                return;
            }

            var props = PhotonNetwork.CurrentRoom.CustomProperties;
            props[key.ToString()] = value;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        public IReadOnlyReactiveProperty<bool> IsHost() => rIsHost;
        public IReadOnlyReactiveProperty<MatchModel> GetMatchModel() => rMatchModel;
        public IReadOnlyReactiveProperty<string> GetMatchCode() => rMatchCode;
        public IReadOnlyReactiveProperty<string> GetConnectionIssue() => rConnectionIssue;
        public IReadOnlyReactiveProperty<bool> IsConnected() => rIsConnected;
        public IReadOnlyReactiveProperty<List<PlayerModel>> GetPlayersInMatch() => rPlayers;

        #endregion //Connection Model OVERRIDES

        private RoomOptions CreateFreshRoomOptions()
        {
            var roomOptions = new RoomOptions();
            roomOptions.PublishUserId = true;
            roomOptions.CustomRoomProperties = new Hashtable();
            roomOptions.CustomRoomProperties.Add(
                MatchProperty.BOOL_IS_LOADING.ToString(), false);
            //TODO add more props here...

            return roomOptions;
        }

        private void EstablishConnection(string playerName, string matchCode)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            
            rMatchCode.Value = matchCode;
        }

        private void UpdatePlayers()
        {
            PhotonNetwork.SetPlayerCustomProperties(
                PhotonNetwork.LocalPlayer.ResetPlayerModel());
            RefreshIsHostStatus();

            var players = PhotonNetwork.CurrentRoom.Players.Values;
            Debug.Log($"{GetType().Name} UpdatePlayers: Count is: {players.Count}");
            rPlayers.Value.Clear();

            rIsConnected.Value = PhotonNetwork.IsConnected;

            if (players.Count == 0)
            {
                return;
            }

            var list = new List<PlayerModel>();
            foreach (var playa in players)
            {
                list.Add(playa.GetPlayerModel());
            }

            rPlayers.SetValueAndForceNotify(list);
        }

        private void RefreshIsHostStatus() => rIsHost.Value = PhotonNetwork.IsMasterClient;

        #region Photon Overrides

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            Debug.Log($"{GetType().Name} OnRoomPropertiesUpdate");

            var matchProps = new MatchModel();
            var currProps = PhotonNetwork.CurrentRoom.CustomProperties;
            
            matchProps.isSceneLoading = currProps.TryGetValue(
                MatchProperty.BOOL_IS_LOADING.ToString(), out var isLoading) 
                ? (bool)isLoading : false;
            //TODO add more... (force update props)

            rMatchModel.SetValueAndForceNotify(matchProps);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log($"{GetType().Name} OnCreatedRoom");

            //NOTE: No need for this. Photon makes the Player auto-join their newly created room.
            //PhotonNetwork.JoinRoom(rMatchCode.Value); 
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log($"{GetType().Name} OnCreateRoomFailed '{rMatchCode.Value}': {message}");
            rConnectionIssue.Value = message;
            Disconnect();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"{GetType().Name} OnJoinedRoom");
            UpdatePlayers();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"OnJoinRoomFailed '{rMatchCode.Value}': {message}");
            rConnectionIssue.Value = message;
            Disconnect();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            rMatchCode.Value = string.Empty;
            rIsConnected.SetValueAndForceNotify(false);
        }

        public override void OnConnectedToMaster()
        {
            rIsConnected.SetValueAndForceNotify(PhotonNetwork.IsConnected);
            Debug.Log($"{GetType().Name} Is Connected: {rIsConnected.Value}");

            if (isMatchNew)
            {
                PhotonNetwork.CreateRoom(rMatchCode.Value, CreateFreshRoomOptions());
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