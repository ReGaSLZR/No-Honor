using UnityEngine;

namespace ReGaSLZR
{

    public class Character : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private bool isBot;

        [Space]

        [Header("Components")]

        [SerializeField]
        private CharacterMovement movt;

        [SerializeField]
        private CharacterStats stats;

        [SerializeField]
        private CharacterItemPicker itemPicker;

        [SerializeField]
        private CharacterItemUser itemUser;

        #endregion //Inspector Fields

        public CharacterStats Stats => stats;

        #region Client Impl

        private void SetUpAsNPC()
        {
            movt.enabled = false;
            itemUser.enabled = false;
            itemPicker.enabled = false;

            stats.View.SetIndicatorIsDisplayed(false);
            gameObject.name = Constants.DEFAULT_NPC_NAME;
        }

        private void SetUpAsLocalPlayer(PlayerHUD hud)
        {
            itemPicker.SetUp(hud);
            itemUser.SetUp(hud);
            stats.SetUp(hud);

            var model = stats.Model.Value;
            model.isLocalPlayer = true;
            stats.UpdateModel(model);
        }

        #endregion //Client Impl

        #region Public API

        public void SetUp(bool isBot, PlayerHUD viewHud)
        {
            this.isBot = isBot;

            if (isBot)
            {
                SetUpAsNPC();
            }
            else
            {
                SetUpAsLocalPlayer(viewHud);
            }
        }

        #endregion //Public API

    }

}