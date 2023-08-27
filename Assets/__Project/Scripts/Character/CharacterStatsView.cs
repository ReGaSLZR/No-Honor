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

        private ReactiveProperty<int> rHealthChange = new ReactiveProperty<int>(0);
        private float healthChangeRectPositionX;

        #endregion //Private Fields

        #region Unity Callbacks

        private void Awake()
        {
            healthChangeRectPositionX = rectHealthChange.localPosition.x;

            sliderHealth.minValue = PlayerModel.PLAYER_HEALTH_DEAD;
            sliderHealth.maxValue = PlayerModel.PLAYER_HEALTH_MAX;   
        }

        private void Start()
        {
            rHealthChange
                .Subscribe(AnimateHealthChangeFX)
                .AddTo(this);

            textHealthChange.CrossFadeAlpha(0f, 0f, true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                UpdateHealth((int)sliderHealth.value - 4);
            }
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

        public IReadOnlyReactiveProperty<int> GetHealthDiminished() => rHealthChange;

        #endregion //Public API

        #region Client Impl

        private void AnimateHealthChangeFX(int healthChange)
        {
            StopAllCoroutines();
            StartCoroutine(C_AnimateHealthChangeFX(healthChange));
        }

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

        private void UpdateHealth(int newHealth)
        {
            var presentHealth = (int)sliderHealth.value;   
            var newHealthValue = Mathf.Clamp(newHealth,
                PlayerModel.PLAYER_HEALTH_DEAD, PlayerModel.PLAYER_HEALTH_MAX);
            sliderHealth.value = newHealthValue;
            sliderFill.color = GetColor(newHealthValue);

            var dim = Mathf.Clamp(presentHealth - newHealthValue, 
                0, PlayerModel.PLAYER_HEALTH_MAX);
            rHealthChange.SetValueAndForceNotify(-dim);
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