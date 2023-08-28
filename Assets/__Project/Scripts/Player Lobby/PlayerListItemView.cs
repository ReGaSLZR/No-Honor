using TMPro;
using UnityEngine;

namespace ReGaSLZR
{

    public class PlayerListItemView : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        protected TextMeshProUGUI textName;

        [Space]

        [SerializeField]
        protected Color colorNormal;

        [SerializeField]
        protected Color colorHighlighted;

        [Space]

        [SerializeField]
        protected string prefixHost;

        [SerializeField]
        protected string suffixLocalPlayer;

        #endregion //Inspector Fields

        #region Public API

        public virtual void SetUp(PlayerModel player)
        {
            if (player == null)
            {
                return;
            }

            textName.text = string.Concat(player.isHost ? prefixHost : string.Empty, 
                player.playerName, player.isLocalPlayer ? suffixLocalPlayer : string.Empty);

            textName.color = player.isLocalPlayer ? colorHighlighted : colorNormal;
        }

        #endregion //Public API

    }

}