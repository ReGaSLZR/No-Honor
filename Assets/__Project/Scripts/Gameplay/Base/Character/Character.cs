using UnityEngine;

namespace ReGaSLZR
{

    public class Character : MonoBehaviour
    {

        #region Inspector Fields

        [Header("Components")]

        [SerializeField]
        protected CharacterMovement movt;

        [SerializeField]
        protected CharacterStats stats;

        [SerializeField]
        protected CharacterItemPicker itemPicker;

        [SerializeField]
        protected CharacterItemUser itemUser;

        [SerializeField]
        protected TargetDetector[] targetDetectors;

        #endregion //Inspector Fields

        public CharacterStats Stats => stats;

        #region Client Impl

        protected void SetUpAsNPC()
        {
            movt.enabled = false;
            itemUser.enabled = false;
            //itemPicker.enabled = false;

            stats.View.SetIndicatorIsDisplayed(false);
            gameObject.name = Constants.DEFAULT_NPC_NAME;
        }

        protected void SetUpAsLocalPlayer(PlayerHUD hud)
        {
            itemPicker.SetUp(hud);
            itemUser.SetUp(hud);
            stats.SetUp(hud);

            var model = stats.Model.Value;
            model.isLocalPlayer = true;
            stats.UpdateModel(model);

            foreach (var detector in targetDetectors)
            {
                detector.gameObject.SetActive(false);
            }
        }

        #endregion //Client Impl

        #region Public API

        public void SetUp(bool isBot, PlayerHUD viewHud)
        {
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