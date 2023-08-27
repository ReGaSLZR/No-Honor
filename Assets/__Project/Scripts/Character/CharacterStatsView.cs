using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ReGaSLZR
{

    public class CharacterStatsView : MonoBehaviour
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

        #region Private Fields

        private ReactiveProperty<bool> rIsHealthDiminished = new ReactiveProperty<bool>(false);

        #endregion //Private Fields

        #region Unity Callbacks

        private void Awake()
        {
            sliderHealth.minValue = PlayerModel.PLAYER_HEALTH_DEAD;
            sliderHealth.maxValue = PlayerModel.PLAYER_HEALTH_MAX;
        }

        #endregion //Unity Callbacks

        #region Public API

        public void UpdateView(PlayerModel playerModel)
        {
            textCharacterName.text = playerModel.IsLocalPlayer 
                ? localCharacterName : playerModel.PlayerName;
            textCharacterName.color = playerModel.IsLocalPlayer 
                ? colorNameLocal : colorNameEnemy;
            localPlayerIndicator.SetActive(playerModel.IsLocalPlayer);

            UpdateHealth(playerModel.Health);
        }

        public IReadOnlyReactiveProperty<bool> IsHealthDiminished() => rIsHealthDiminished;

        #endregion //Public API

        #region Client Impl

        private void UpdateHealth(int newHealth)
        {
            var presentHealth = (int)sliderHealth.value;   
            var newHealthValue = Mathf.Clamp(newHealth,
                PlayerModel.PLAYER_HEALTH_DEAD, PlayerModel.PLAYER_HEALTH_MAX);
            sliderHealth.value = newHealthValue;
            sliderFill.color = GetColor(newHealthValue);

            rIsHealthDiminished.SetValueAndForceNotify(newHealthValue < presentHealth);
        }

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