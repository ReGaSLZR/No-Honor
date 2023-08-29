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
        protected CharacterAnimator characterAnimator;

        [SerializeField]
        protected AWeaponAsUsable[] weaponsOnDemand;

        [Header("Runtime set")]

        [SerializeField]
        protected PlayerHUD hudView;

        #endregion //Inspector Fields

        protected CharacterItemPicker itemPicker;
        protected AWeaponAsUsable cachedWeaponOnUse;
        protected float cachedWeaponUseTimeEnd;

        protected CompositeDisposable disposables = new CompositeDisposable();

        #region Unity Callbacks

        protected virtual void Awake() => itemPicker = GetComponent<CharacterItemPicker>();

        protected virtual void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => enabled)
                .Where(_ => Input.GetKeyDown(KeyCode.Return)) //TODO store keycode somewhere
                .Select(_ => itemPicker.GetWeaponPicked().Value)
                .Where(weapon => weapon != null)
                .Where(_ => cachedWeaponOnUse == null || !cachedWeaponOnUse.IsInUse().Value)
                .Subscribe(OnUseWeapon)
                .AddTo(this);
        }

        #endregion //Unity Callbacks

        #region Public API

        public void SetUp(PlayerHUD hudView) => this.hudView = hudView;

        #endregion //Public API

        #region Client Impl

        protected virtual void UseWeapon(AWeaponAsUsable weaponToUse)
        {
            if(weaponToUse == null)
            {
                Debug.LogWarning($"{GetType().Name}.OnUseWeapon() failed. Couldn't find weapon from list.");
                return;
            }

            var isUsed = weaponToUse.AttemptUse();
            if (isUsed)
            {
                RefreshDisposable();

                cachedWeaponOnUse = weaponToUse;
                characterAnimator.AnimateAttackWithWeapon(cachedWeaponOnUse.WeaponType);
                SetUpWeponTargetDetection();

                if (hudView != null) 
                {
                    hudView.UpdateWeapon(Weapon.None);
                }

                itemPicker.ClearWeapon();
            }
            else
            {
                Debug.LogWarning($"{GetType().Name}.OnUseWeapon() '{weaponToUse.WeaponType}'" +
                    $" still in use. Wait for its duration to be over.");
            }
        }

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

            UseWeapon(weaponToUse);
        }

        protected virtual void SetUpWeponTargetDetection()
        {
            cachedWeaponUseTimeEnd = 0f;

            cachedWeaponOnUse.IsTargetDetected()
                .Where(det => det)
                .Where(_ => !cachedWeaponOnUse.IsDamageOverTime)
                .Subscribe(_ => DamageWeaponVictims())
                .AddTo(disposables);

            if (cachedWeaponOnUse.IsDamageOverTime)
            {
                this.UpdateAsObservable()
                    .Where(_ => cachedWeaponOnUse.IsTargetDetected().Value)
                    .Where(_ => cachedWeaponUseTimeEnd < Time.time)
                    .Subscribe(_ => DamageWeaponVictims())
                    .AddTo(disposables);

                this.UpdateAsObservable()
                    .Where(_ => cachedWeaponOnUse.IsDamageOverTime)
                    .Where(_ => cachedWeaponOnUse.IsInUse().Value)
                    .Subscribe(_ => characterAnimator.AnimateAttackWithWeapon(cachedWeaponOnUse.WeaponType))
                    .AddTo(disposables);
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
            Debug.Log($"DamageWeaponVictims called.", gameObject);
            var victims = cachedWeaponOnUse.GetTargets();
            cachedWeaponUseTimeEnd = cachedWeaponOnUse.DamageOverTimeInterval + Time.time; 

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