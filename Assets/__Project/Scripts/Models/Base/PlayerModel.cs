namespace ReGaSLZR
{

    [System.Serializable]
    public class PlayerModel
    {

        #region Constants

        public const int PLAYER_HEALTH_MAX = 100;
        public const int PLAYER_HEALTH_DEAD = 0;

        #endregion //Constants

        #region Public Fields

        public bool isLocalPlayer;
        public bool isHost;
        public string playerName = Constants.DEFAULT_NPC_NAME;
        public string playerId;

        public int health = PLAYER_HEALTH_MAX;
        public Weapon weapon = Weapon.None;

        #endregion //Public Fields

        #region Public API

        public PlayerModel() { }

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