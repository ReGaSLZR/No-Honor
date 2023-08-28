using UniRx;
using UnityEngine;

namespace ReGaSLZR
{

    [RequireComponent(typeof(Collider2D))]
    public class CharacterStats : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private CharacterStatsView view;

        #endregion //Inspector Fields

        private readonly ReactiveProperty<PlayerModel> rModel 
            = new ReactiveProperty<PlayerModel>(new PlayerModel());

        private readonly ReactiveProperty<bool> rIsPlayerDead 
            = new ReactiveProperty<bool>(false);

        private ReactiveProperty<int> rHealthChange = new ReactiveProperty<int>(0);

        #region Accessors

        public CharacterStatsView View => view;

        public IReadOnlyReactiveProperty<bool> IsPlayerDead() => rIsPlayerDead;
        public IReadOnlyReactiveProperty<int> GetHealthDiminished() => rHealthChange;
        public IReadOnlyReactiveProperty<PlayerModel> Model => rModel;

        #endregion //Accessors

        #region Unity Callbacks



        #endregion //Unity Callbacks

        #region Public API

        public void ApplyDamage(int damage)
        {
            Debug.Log($"{GetType().Name}.ApplyDamage {damage}", gameObject);
            rModel.Value.health = Mathf.Clamp((rModel.Value.health - damage), PlayerModel.PLAYER_HEALTH_DEAD, PlayerModel.PLAYER_HEALTH_MAX);

            if (rModel.Value.health != PlayerModel.PLAYER_HEALTH_DEAD)
            {
                view.AnimateHealthChangeFX(-damage);
            }

            UpdateModel(rModel.Value);
        }

        public void UpdateModel(PlayerModel model)
        {
            rIsPlayerDead.Value = Mathf.Clamp(model.health,
                PlayerModel.PLAYER_HEALTH_DEAD, PlayerModel.PLAYER_HEALTH_MAX)
                == PlayerModel.PLAYER_HEALTH_DEAD;

            var dim = Mathf.Clamp(rModel.Value.health - model.health,
                0, PlayerModel.PLAYER_HEALTH_MAX);
            rHealthChange.SetValueAndForceNotify(-dim);

            rModel.SetValueAndForceNotify(model);
            view.UpdateView(model);
        }

        #endregion //Public API

        #region Client Impl


        #endregion //Client Impl

    }

}