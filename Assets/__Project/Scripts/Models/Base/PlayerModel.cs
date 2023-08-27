namespace ReGaSLZR
{

    [System.Serializable]
    public class PlayerModel
    {

        #region Constants

        public const int PLAYER_HEALTH_MAX = 100;
        public const int PLAYER_HEALTH_DEAD = 0;

        #endregion //Constants

        #region Private Fields

        private bool isLocalPlayer;
        private bool isHost;
        private string playerName;
        private string playerId;

        private int health;
        private Weapon weapon;

        #endregion //Private Fields

        #region Accessors

        public bool IsLocalPlayer => isLocalPlayer;
        public bool IsHost => isHost;
        public string PlayerName => playerName;
        public string PlayerId => playerId;

        public int Health => health;
        public Weapon Weapon => weapon;

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