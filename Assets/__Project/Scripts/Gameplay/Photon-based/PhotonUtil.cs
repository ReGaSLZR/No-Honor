using ExitGames.Client.Photon;
using Photon.Realtime;

namespace ReGaSLZR
{

    public static class PhotonUtil
    {

        #region Client Impl

        private static T GetValue<T>(this Hashtable props, string key, T defaultValue)
        {
            if (props.ContainsKey(key))
            {
                return (T) props[key];
            }

            return defaultValue;
        }

        private static void SetValue(this Hashtable props, string key, object value)
        {
            if (props.ContainsKey(key))
            {
                props[key] = value;
            }
            else
            {
                props.Add(key, value);
            }
        }

        #endregion //Client Impl

        #region Public API

        public static PlayerModel GetPlayerModel(this Player player)
        {
            var model = new PlayerModel(
                player.NickName, player.UserId, player.IsLocal, player.IsMasterClient);

            var props = player.CustomProperties;

            model.health = props.GetValue(
                PlayerProperty.INT_HEALTH.ToString(), PlayerModel.PLAYER_HEALTH_MAX);
            model.weapon = (Weapon)(props.GetValue(
                PlayerProperty.INT_WEAPON.ToString(), (int)Weapon.None));
            model.isWinner = props.GetValue(
                PlayerProperty.BOOL_IS_WINNER.ToString(), false);
            model.surviveTime = props.GetValue(
                PlayerProperty.INT_SURVIVE.ToString(), 0);

            return model;
        }

        public static Hashtable ResetPlayerModel(this Player player)
        {
            var hash = player.CustomProperties;

            hash.SetValue(PlayerProperty.INT_HEALTH.ToString(), PlayerModel.PLAYER_HEALTH_MAX);
            hash.SetValue(PlayerProperty.INT_WEAPON.ToString(), (int)Weapon.None);
            hash.SetValue(PlayerProperty.BOOL_IS_WINNER.ToString(), false);
            hash.SetValue(PlayerProperty.INT_SURVIVE.ToString(), 0);

            return hash;
        }

        public static Hashtable GetProperties(this PlayerModel playerModel)
        {
            var hash = new Hashtable();

            hash.Add(PlayerProperty.INT_HEALTH.ToString(), playerModel.health);
            hash.Add(PlayerProperty.INT_WEAPON.ToString(), playerModel.weapon);
            hash.Add(PlayerProperty.BOOL_IS_WINNER.ToString(), playerModel.isWinner);
            hash.Add(PlayerProperty.INT_SURVIVE.ToString(), playerModel.surviveTime);

            return hash;
        }

        #endregion //Public API

    }

}