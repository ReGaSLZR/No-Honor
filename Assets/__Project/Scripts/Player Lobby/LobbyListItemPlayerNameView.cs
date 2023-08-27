using TMPro;
using UnityEngine;

namespace ReGaSLZR
{

    public class LobbyListItemPlayerNameView : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private TextMeshProUGUI textName;

        [Space]

        [SerializeField]
        private Color colorNormal;

        [SerializeField]
        private Color colorHighlighted;

        [Space]

        [SerializeField]
        private string prefixHost;

        [SerializeField]
        private string suffixLocalPlayer;

        #endregion //Inspector Fields

        #region Public API

        public void SetUp(PlayerModel player)
        {
            textName.text = string.Concat(player.isHost ? prefixHost : string.Empty, 
                player.playerName, player.isLocalPlayer ? suffixLocalPlayer : string.Empty);

            textName.color = player.isLocalPlayer ? colorHighlighted : colorNormal;
        }

        #endregion //Public API

    }

}