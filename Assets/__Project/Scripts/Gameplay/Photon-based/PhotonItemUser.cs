using Photon.Pun;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ReGaSLZR
{

    public class PhotonItemUser : CharacterItemUser
    {

        [Header("Photon-specific")]

        [SerializeField]
        private PhotonView photonView;

        [SerializeField]
        private CharacterStats stats;

        protected override void Start()
        {
            if (photonView.Owner.IsLocal)
            {
                base.Start();

                this.UpdateAsObservable()
                    .Where(_ => cachedWeaponOnUse != null)
                    .Where(_ => !cachedWeaponOnUse.IsInUse().Value)
                    .Where(_ => stats.Model.Value.weapon != Weapon.None)
                    .Subscribe(_ => stats.UpdateWeapon(Weapon.None))
                    .AddTo(this);

                this.UpdateAsObservable()
                    .Where(_ => (cachedWeaponOnUse != null) 
                            && cachedWeaponOnUse.IsInUse().Value)
                    .Where(_ => stats.Model.Value.weapon == Weapon.None)
                    .Subscribe(_ => stats.UpdateWeapon(cachedWeaponOnUse.WeaponType))
                    .AddTo(this);
            }
            else
            {
                stats.Model
                    .Where(model => model.weapon == Weapon.None)
                    .Subscribe(model => DisableAllWeapons())
                    .AddTo(this);

                stats.Model
                    .Where(model => model.weapon != Weapon.None)
                    .Subscribe(model => EnableWeapon(model.weapon))
                    .AddTo(this);
            }
        }

        private void DisableAllWeapons()
        {
            foreach (var weaponOnDemand in weaponsOnDemand)
            {
                weaponOnDemand.Deactivate();
            }
        }

        private void EnableWeapon(Weapon weapon)
        {
            DisableAllWeapons();

            foreach (var weaponOnDemand in weaponsOnDemand)
            {
                if (weaponOnDemand.WeaponType == weapon)
                {
                    UseWeapon(weaponOnDemand);
                }
            }
        }

    }

}