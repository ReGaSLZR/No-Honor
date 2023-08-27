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

        private PlayerModel cachedModel = new PlayerModel();

        #region Accessors

        public CharacterStatsView View => view;

        #endregion //Accessors

        #region Public API

        public void ApplyDamage(int damage)
        {
            Debug.Log($"{GetType().Name}.ApplyDamage {damage}", gameObject);
            cachedModel.health = Mathf.Clamp((cachedModel.health - damage), PlayerModel.PLAYER_HEALTH_DEAD, PlayerModel.PLAYER_HEALTH_MAX);
            UpdateModel(cachedModel);
        }

        public void UpdateModel(PlayerModel model)
        {
            cachedModel = model;
            view.UpdateView(cachedModel);
        }

        #endregion //Public API

        #region Client Impl


        #endregion //Client Impl

    }

}