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
        private float cachedWeaponUseTimeEnd;

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
                cachedWeaponUseTimeEnd = 0f;
                RefreshDisposable();

                cachedWeaponOnUse = weaponToUse;
                SetUpWeponTargetDetection();

                hudView.UpdateWeapon(Weapon.None);
                itemPicker.ClearWeapon();
            }
            else
            {
                Debug.LogWarning($"{GetType().Name}.OnUseWeapon() '{weapon.Type}'" +
                    $" still in use. Wait for its duration to be over before you can use it.");
            }
        }

        private void SetUpWeponTargetDetection()
        {
            cachedWeaponOnUse.IsTargetDetected()
                .Where(det => det)
                .Where(_ => !cachedWeaponOnUse.IsDamageOverTime)
                .Subscribe(_ => DamageWeaponVictims())
                .AddTo(disposables);

            cachedWeaponOnUse.IsTargetDetected()
                .Where(det => det)
                .Where(_ => cachedWeaponOnUse.IsDamageOverTime)
                .Where(_ => cachedWeaponUseTimeEnd < Time.time)
                .Subscribe(_ => DamageWeaponVictims())
                .AddTo(disposables);
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