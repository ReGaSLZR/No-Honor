using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ReGaSLZR
{

    [RequireComponent(typeof(CharacterItemPicker))]
    public class CharacterItemUser : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private AWeaponAsUsable[] weaponsOnDemand;

        [Header("Runtime set")]

        [SerializeField]
        private PlayerHUD hudView;

        #endregion //Inspector Fields

        private CharacterItemPicker itemPicker;
        private AWeaponAsUsable cachedWeaponOnUse;

        private CompositeDisposable disposables = new CompositeDisposable();

        #region Unity Callbacks

        private void Awake() => itemPicker = GetComponent<CharacterItemPicker>();

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => enabled)
                .Where(_ => Input.GetKeyDown(KeyCode.Return)) //TODO store keycode somewhere
                .Select(_ => itemPicker.GetWeaponPicked().Value)
                .Where(weapon => weapon != null)
                .Where(_ => cachedWeaponOnUse == null || !cachedWeaponOnUse.IsInUse)
                .Subscribe(OnUseWeapon)
                .AddTo(this);

            //this.UpdateAsObservable()
            //    .Where(_ => cachedWeaponOnUse != null)
            //    .Where(_ => cachedWeaponOnUse.IsTargetDetected().Value)
            //    .Subscribe(_ => DamageWeaponVictims())
            //    .AddTo(this);
        }

        #endregion //Unity Callbacks

        #region Public API

        public void SetUp(PlayerHUD hudView) => this.hudView = hudView;

        #endregion //Public API

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
                RefreshDisposable();

                cachedWeaponOnUse = weaponToUse;
                cachedWeaponOnUse.IsTargetDetected()
                    .Where(det => det)
                    .Subscribe(_ => DamageWeaponVictims())
                    .AddTo(disposables);

                hudView.UpdateWeapon(Weapon.None);
                itemPicker.ClearWeapon();
            }
            else
            {
                Debug.LogWarning($"{GetType().Name}.OnUseWeapon() '{weapon.Type}'" +
                    $" still in use. Wait for its duration to be over before you can use it.");
            }
        }

        private void RefreshDisposable()
        {
            disposables.Dispose();
            disposables.Clear();
            disposables = new CompositeDisposable();
        }

        private void DamageWeaponVictims()
        {
            Debug.Log($"DamageWeaponVictims called.");
            var victims = cachedWeaponOnUse.GetTargets();

            if (victims.Count == 0)
            {
                return;
            }

            foreach (var victim in victims)
            {
                victim.ApplyDamage(cachedWeaponOnUse.DamageValue);
            }
        }

        #endregion //Client Impl

    }

}