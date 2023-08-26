using UnityEngine;

namespace ReGaSLZR
{

    [System.Serializable]
    public class PlayerModel
    {

        #region Private Fields

        private bool isLocalPlayer;
        private bool isHost;
        private string playerName;
        private string playerId;

        #endregion //Private Fields

        #region Accessors

        public bool IsLocalPlayer => isLocalPlayer;
        public bool IsHost => isHost;
        public string PlayerName => playerName;
        public string PlayerId => playerId;

        #endregion //Accessors

        #region Public API

        public PlayerModel(
            string playerName, string playerId, bool isLocalPlayer, bool isHost)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            this.isLocalPlayer = isLocalPlayer;
            this.isHost = isHost;
        }

        #endregion //Public API

    }

}