using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReGaSLZR
{

    public class LeaderboardListItemView : PlayerListItemView
    {

        #region Inspector Fields

        [Header("Leaderboard Specific Elements")]

        [SerializeField]
        private TextMeshProUGUI textDetails;

        [SerializeField]
        private Image imageIcon;

        [Space]

        [SerializeField]
        private Color colorWinner;

        [SerializeField]
        private Sprite spriteWinner;

        [SerializeField]
        private Sprite spriteLoser;

        [SerializeField]
        private string detailsValue;

        [SerializeField]
        private string detailsAsWinner;

        #endregion //Inspector Fields

        #region Overrides

        public override void SetUp(PlayerModel player)
        {
            base.SetUp(player);

            textName.color = player.isWinner ? colorWinner : textName.color;
            imageIcon.sprite = player.isWinner ? spriteWinner : spriteLoser;

            textDetails.color = player.isWinner ? colorWinner : colorNormal;
            textDetails.text = player.isWinner 
                ? detailsAsWinner : string.Format(detailsValue, player.surviveTime);
        }

        #endregion //Overrides

    }

}