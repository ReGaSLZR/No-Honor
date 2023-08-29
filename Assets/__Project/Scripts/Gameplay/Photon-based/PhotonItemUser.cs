using Photon.Pun;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ReGaSLZR
{

    public class PhotonItemUser : CharacterItemUser
    {

        #region Inspector Fields

        [Header("Photon-specific")]

        [SerializeField]
        private PhotonView photonView;

        [SerializeField]
        private CharacterStats stats;

        #endregion //Inspector Fields

        #region Unity Callbacks

        protected override void Start()
        {
            if (photonView.Owner.IsLocal)
            {
                base.Start();
            }
        }

        #endregion //Unity Callbacks

        #region Client Impl

        [PunRPC]
        private void RPC_ReflectWeaponUsage(string playerId, int weapon)
        {
            Debug.Log($"{GetType().Name}.RPC_ReflectWeaponUsage()", gameObject);

            if (!playerId.Equals(photonView.Owner.UserId))
            {
                return;
            }

            var wea = (Weapon)weapon;
            if (wea == Weapon.None)
            {
                DisableAllWeapons();
            }
            else
            {
                EnableWeapon(wea);
            }
        }

        protected override void SetUpWeponTargetDetection()
        {
            base.SetUpWeponTargetDetection();

            if (photonView.Owner.IsLocal) 
            {
                cachedWeaponOnUse.IsInUse()
                    .Where(inUse => inUse)
                    .Subscribe(_ => photonView.RPC("RPC_ReflectWeaponUsage",
                        RpcTarget.Others, photonView.Owner.UserId, 
                        (int) cachedWeaponOnUse.WeaponType))
                    .AddTo(disposables);

                cachedWeaponOnUse.IsInUse()
                    .Where(inUse => !inUse)
                    .Subscribe(_ => photonView.RPC("RPC_ReflectWeaponUsage",
                        RpcTarget.Others, photonView.Owner.UserId,
                        (int)Weapon.None))
                    .AddTo(disposables);
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

        #endregion //Client Impl

    }

}