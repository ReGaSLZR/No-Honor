using TMPro;
using UnityEngine;

namespace ReGaSLZR
{

    public class LobbyListItemPlayerName : MonoBehaviour
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

        public void SetUp(string playerName, bool isLocalPlayer, bool isHost)
        {
            textName.text = string.Concat(isHost ? prefixHost : string.Empty, playerName,
                isLocalPlayer ? suffixLocalPlayer : string.Empty);

            textName.color = isLocalPlayer ? colorHighlighted : colorNormal;
        }

        #endregion //Public API

    }

}