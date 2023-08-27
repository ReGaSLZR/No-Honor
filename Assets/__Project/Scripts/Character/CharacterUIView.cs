using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReGaSLZR
{

    public class CharacterUIView : MonoBehaviour
    {

        #region Inspector Fields

        [Header("UI Elements")]

        [SerializeField]
        private GameObject localPlayerIndicator;

        [SerializeField]
        private TextMeshProUGUI textCharacterName;

        [SerializeField]
        private Slider sliderHealth;

        [SerializeField]
        private Image sliderFill;

        [Header("Settings")]

        [SerializeField]
        [Tooltip("Arrange from lowest to highest.")]
        private Color[] colorHealthStatus;

        [SerializeField]
        private Color colorNameLocal;

        [SerializeField]
        private Color colorNameEnemy;

        [SerializeField]
        private string localCharacterName;

        #endregion //Inspector Fields

        #region Public API

        public void SetUp(PlayerModel playerModel)
        {
            textCharacterName.text = playerModel.IsLocalPlayer 
                ? localCharacterName : playerModel.PlayerName;
            textCharacterName.color = playerModel.IsLocalPlayer 
                ? colorNameLocal : colorNameEnemy;
            localPlayerIndicator.SetActive(playerModel.IsLocalPlayer);

            sliderHealth.minValue = PlayerModel.PLAYER_HEALTH_DEAD;
            sliderHealth.maxValue = PlayerModel.PLAYER_HEALTH_MAX;
            sliderHealth.value = Mathf.Clamp(playerModel.Health, 
                PlayerModel.PLAYER_HEALTH_DEAD, PlayerModel.PLAYER_HEALTH_MAX);
            sliderFill.color = GetColor((int)sliderHealth.value);
        }

        #endregion //Public API

        #region Client Impl

        private Color GetColor(int value)
        {
            var part = PlayerModel.PLAYER_HEALTH_MAX / colorHealthStatus.Length;
            var index = 0;

            while (index < colorHealthStatus.Length)
            {
                if (value <= (part*(index+1)))
                {
                    return colorHealthStatus[index];
                }

                index++;
            }

            return Color.black;
        }

        #endregion //Client Impl

    }

}