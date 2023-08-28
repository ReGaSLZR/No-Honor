using UniRx;
using UnityEngine;

namespace ReGaSLZR
{

    [RequireComponent(typeof(Collider2D))]
    public class CharacterStats : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        protected CharacterStatsView viewStats;

        [SerializeField]
        protected PlayerHUD viewHud;

        #endregion //Inspector Fields

        protected readonly ReactiveProperty<PlayerModel> rModel 
            = new ReactiveProperty<PlayerModel>(new PlayerModel());

        protected readonly ReactiveProperty<bool> rIsPlayerDead 
            = new ReactiveProperty<bool>(false);

        protected readonly ReactiveProperty<int> rHealthChange 
            = new ReactiveProperty<int>(0);

        #region Accessors

        public CharacterStatsView View => viewStats;

        public IReadOnlyReactiveProperty<bool> IsPlayerDead() => rIsPlayerDead;
        public IReadOnlyReactiveProperty<int> GetHealthDiminished() => rHealthChange;
        public IReadOnlyReactiveProperty<PlayerModel> Model => rModel;

        #endregion //Accessors

        #region Unity Callbacks

        protected virtual void Start()
        {
            rIsPlayerDead
                .Where(_ => viewHud != null)
                .Subscribe(isDead => viewHud.SetIsPlayerActive(!isDead))
                .AddTo(this);

            rIsPlayerDead
                .Where(isDead => isDead)
                .Subscribe(_ => RecordSurviveTime())
                .AddTo(this);
        }

        #endregion //Unity Callbacks

        #region Public API

        public void SetUp(PlayerHUD hud) => viewHud = hud;

        //TODO remove! For Unity Editor debugging only.
        [NaughtyAttributes.Button] public void Test() => ApplyDamage(67);

        public void ApplyDamage(int damage)
        {
            Debug.Log($"ApplyDamage {damage} on {gameObject.name}", gameObject);
            rModel.Value.health = Mathf.Clamp((rModel.Value.health - damage), PlayerModel.PLAYER_HEALTH_DEAD, PlayerModel.PLAYER_HEALTH_MAX);

            if (rModel.Value.health > PlayerModel.PLAYER_HEALTH_DEAD)
            {
                viewStats.AnimateHealthChangeFX(-damage);
            }

            UpdateModel(rModel.Value);
        }

        public void RecordSurviveTime()
        {
            viewStats.SetIsDisplayed(false);

            var model = rModel.Value;
            model.surviveTime = (int)Time.timeSinceLevelLoad;
            model.isWinner = false;
            UpdateModel(model);
        }

        public void MarkAsWinner()
        {
            var model = rModel.Value;
            model.isWinner = true;
            UpdateModel(model);
        }

        public void UpdateWeapon(Weapon weapon)
        {
            var model = rModel.Value;
            model.weapon = weapon;
            UpdateModel(model);
        }

        public void UpdateModel(PlayerModel model, bool isForcedAnimate = false)
        {
            rIsPlayerDead.Value = Mathf.Clamp(model.health,
                PlayerModel.PLAYER_HEALTH_DEAD, PlayerModel.PLAYER_HEALTH_MAX)
                == PlayerModel.PLAYER_HEALTH_DEAD;

            var dim = Mathf.Clamp(rModel.Value.health - model.health,
                PlayerModel.PLAYER_HEALTH_DEAD, PlayerModel.PLAYER_HEALTH_MAX);
            rHealthChange.SetValueAndForceNotify(-dim);

            if (isForcedAnimate && (dim != 0))
            {
                viewStats.AnimateHealthChangeFX(dim);
            }

            rModel.SetValueAndForceNotify(model);
            viewStats.UpdateView(model);
        }

        #endregion //Public API

        #region Client Impl


        #endregion //Client Impl

    }

}