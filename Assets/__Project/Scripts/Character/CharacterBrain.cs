using NaughtyAttributes;
using UnityEngine;
using UniRx;
using System.Collections;

namespace ReGaSLZR
{

    public class CharacterBrain : MonoBehaviour
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
                Destroy(itemPicker);

                statsView.SetIsDisplayed(false);
            }
        }

        #endregion //Unity Callbacks

        #region Public API

        public void SetUpAsPlayer(PlayerHUD hudView)
        {
            isNPC = false;
            this.hudView = hudView;
        }

        #endregion //Public API

    }

}