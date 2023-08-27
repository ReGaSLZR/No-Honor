using NaughtyAttributes;
using UnityEngine;
using UniRx;
using System.Collections;
using UniRx.Triggers;

namespace ReGaSLZR
{

    public class Character : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private bool isNPC;

        [Space]

        [SerializeField]
        private uint framesBeforeNPCSetUp = 5;

        [Header("Components")]

        [SerializeField]
        private CharacterMovement movt;

        [SerializeField]
        private CharacterStatsView statsView;

        [SerializeField]
        private CharacterItemPicker itemPicker;

        [SerializeField]
        private CharacterItemUser itemUser;

        [Header("For Setting Up in Runtime")]

        [SerializeField]
        private PlayerHUD hudView;

        #endregion //Inspector Fields

        #region Unity Callbacks

        private IEnumerator Start()
        {
            var frames = 0;
            while (frames < framesBeforeNPCSetUp)
            {
                yield return null;
                frames++;
            }

            if (isNPC)
            {
                movt.enabled = false;
                Destroy(itemUser);
                Destroy(itemPicker);

                statsView.SetIsDisplayed(false);
                yield break;
            }
        }

        #endregion //Unity Callbacks

        #region Client Impl

        #endregion //Client Impl

        #region Public API

        public void SetUpAsPlayer(PlayerHUD hudView)
        {
            isNPC = false;
            this.hudView = hudView;

            itemPicker.SetUp(hudView);
            itemUser.SetUp(hudView);
        }

        #endregion //Public API

    }

}