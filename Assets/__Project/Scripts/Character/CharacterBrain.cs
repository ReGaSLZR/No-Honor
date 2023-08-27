using NaughtyAttributes;
using UnityEngine;
using UniRx;
using System.Collections;
using UniRx.Triggers;

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

        [Space]

        [SerializeField]
        private AWeaponAsUsable[] weaponsOnDemand;

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
                yield break;
            }

            itemPicker.GetWeaponPicked()
                .Where(weapon => weapon != null)
                .Subscribe(OnPickUpWeapon)
                .AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Return)) //TODO store keycode somewhere
                .Select(_ => itemPicker.GetWeaponPicked().Value)
                .Where(weapon => weapon != null)
                .Subscribe(OnUseWeapon)
                .AddTo(this);
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private void OnUseWeapon(WeaponAsPickable weapon)
        {
            AWeaponAsUsable weaponToUse = null;
            foreach (var weaponOnDemand in weaponsOnDemand)
            {
                if (weaponOnDemand.WeaponType == weapon.Type)
                {
                    weaponToUse = weaponOnDemand;
                    break;
                }
            }

            if (weaponToUse == null)
            {
                Debug.LogWarning($"{GetType().Name}.OnUseWeapon() failed. Couldn't find {weapon.Type} from list.");
                return;
            }

            var isUsed = weaponToUse.AttemptUse();
            if (isUsed)
            {
                hudView.UpdateWeapon(Weapon.None);
                itemPicker.ClearWeapon();
            }
            else 
            {
                Debug.LogWarning($"{GetType().Name}.OnUseWeapon() '{weapon.Type}'" +
                    $" still in use. Wait for its duration to be over before you can use it.");
            }
        }

        private void OnPickUpWeapon(WeaponAsPickable weapon)
        {
            hudView.UpdateWeapon(weapon.Type);
            weapon.gameObject.SetActive(false);
        }

        #endregion //Client Impl

        #region Public API

        public void SetUpAsPlayer(PlayerHUD hudView)
        {
            isNPC = false;
            this.hudView = hudView;
        }

        #endregion //Public API

    }

}