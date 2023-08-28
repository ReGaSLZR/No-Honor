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

        [Header("For Setting Up in Runtime")]

        [SerializeField]
        private PlayerHUD viewHud;

        #endregion //Inspector Fields

        #region Client Impl

        private void SetUpAsNPC()
        {
            movt.enabled = false;
            itemUser.enabled = false;
            itemPicker.enabled = false;

            stats.View.SetIndicatorIsDisplayed(false);
            gameObject.name = Constants.DEFAULT_NPC_NAME;
        }

        private void SetUpAsLocalPlayer()
        {
            itemPicker.SetUp(viewHud);
            itemUser.SetUp(viewHud);

            var model = stats.Model.Value;
            model.isLocalPlayer = true;
            stats.UpdateModel(model);
        }

        #endregion //Client Impl

        #region Public API

        public void SetUp(bool isBot, PlayerHUD viewHud)
        {
            this.isBot = isBot;
            this.viewHud = viewHud;

            if (isBot)
            {
                SetUpAsNPC();
            }
            else
            {
                SetUpAsLocalPlayer();
            }
        }

        #endregion //Public API

    }

}