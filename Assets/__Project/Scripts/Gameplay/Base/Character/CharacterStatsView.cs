using NaughtyAttributes;
using System.Collections;
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
        private GameObject rootView;

        [SerializeField]
        private GameObject localPlayerIndicator;

        [SerializeField]
        private TextMeshProUGUI textCharacterName;

        [SerializeField]
        private TextMeshProUGUI textHealthChange;

        [SerializeField]
        private RectTransform rectHealthChange;

        [SerializeField]
        private Slider sliderHealth;

        [SerializeField]
        private Image sliderFill;

        [Header("Settings")]

        [SerializeField]
        [Tooltip("Arrange from lowest to highest.")]
        private Color[] colorHealthStatus;

        [Space]

        [SerializeField]
        private Color colorNameLocal;

        [SerializeField]
        private Color colorNameEnemy;

        [Space]

        [SerializeField]
        private Color colorHealthDamage;

        [SerializeField]
        private Color colorHealthRegen;

        [Space]

        [SerializeField]
        private string localCharacterName;

        [SerializeField]
        [MinMaxSlider(-1f, 1f)]
        private Vector2 healthChangePositionY;

        [SerializeField]
        [Range(0.5f, 2f)]
        private float healthChangeLerpDuration;

        #endregion //Inspector Fields

        #region Private Fields

        
        private float healthChangeRectPositionX;

        #endregion //Private Fields

        #region Unity Callbacks

        private void Awake()
        {
            healthChangeRectPositionX = rectHealthChange.localPosition.x;

            sliderHealth.minValue = PlayerModel.PLAYER_HEALTH_DEAD;
            sliderHealth.maxValue = PlayerModel.PLAYER_HEALTH_MAX;   

            textHealthChange.CrossFadeAlpha(0f, 0f, true);
        }

        #endregion //Unity Callbacks

        #region Public API

        public void SetIsDisplayed(bool isDisplayed) => rootView.SetActive(isDisplayed);

        public void SetIndicatorIsDisplayed(bool isDisplayed) 
            => localPlayerIndicator.SetActive(isDisplayed);

        public void UpdateView(PlayerModel playerModel)
        {
            textCharacterName.text = playerModel.isLocalPlayer 
                ? localCharacterName : playerModel.playerName;
            textCharacterName.color = playerModel.isLocalPlayer 
                ? colorNameLocal : colorNameEnemy;
            localPlayerIndicator.SetActive(playerModel.isLocalPlayer);

            sliderHealth.value = playerModel.health;
            sliderFill.color = GetColor(playerModel.health);
        }

        public void AnimateHealthChangeFX(int healthChange)
        {
            StopAllCoroutines();
            StartCoroutine(C_AnimateHealthChangeFX(healthChange));
        }

        #endregion //Public API

        #region Client Impl

        private IEnumerator C_AnimateHealthChangeFX(int healthChange)
        {
            var origin = new Vector2(healthChangeRectPositionX, healthChangePositionY.x);
            var destination = new Vector2(healthChangeRectPositionX, healthChangePositionY.y);
            var timeElapsed = 0f;

            textHealthChange.color = (healthChange>=0) ? colorHealthRegen : colorHealthDamage;
            textHealthChange.CrossFadeAlpha(1f, 0f, true);
            textHealthChange.text = healthChange.ToString();

            rectHealthChange.localPosition = origin;
            textHealthChange.CrossFadeAlpha(0f, healthChangeLerpDuration, true);

            while (timeElapsed < healthChangeLerpDuration)
            {
                rectHealthChange.localPosition = Vector2.Lerp(origin, destination,
                    timeElapsed / healthChangeLerpDuration);
                yield return null;
                timeElapsed += Time.deltaTime;
            }

            rectHealthChange.localPosition = destination;
            textHealthChange.CrossFadeAlpha(0f, 0f, true);
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

            return colorHealthStatus[colorHealthStatus.Length - 1];
        }

        #endregion //Client Impl

    }

}