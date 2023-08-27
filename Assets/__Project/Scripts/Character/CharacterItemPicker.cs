using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace ReGaSLZR
{

    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterItemPicker : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private GameObject fxOnPickup;

        #endregion //Inspector Fields

        private ReactiveProperty<WeaponAsPickable> rWeaponPicked = new ReactiveProperty<WeaponAsPickable>();

        #region Unity Callbacks

        private void Start()
        {
            this.OnCollisionEnter2DAsObservable()
                .Where(coll => coll.gameObject.tag.Equals(Tag.Item.ToString()))
                .Select(coll => coll.gameObject.GetComponent<WeaponAsPickable>())
                .Where(weapon => weapon != null)
                .Subscribe(weapon => rWeaponPicked.Value = weapon)
                .AddTo(this);

            rWeaponPicked
                .Where(weapon => weapon != null)
                .Subscribe(OnPickUp)
                .AddTo(this);

            fxOnPickup.SetActive(false);
        }

        #endregion //Unity Callbacks

        #region Public API

        public IReadOnlyReactiveProperty<WeaponAsPickable> GetWeaponPicked() => rWeaponPicked;

        public void ClearWeapon() => rWeaponPicked.Value = null;

        #endregion //Public API

        #region Client Impl

        private void OnPickUp(WeaponAsPickable weapon)
        {
            fxOnPickup.transform.position = weapon.gameObject.transform.position;
            fxOnPickup.SetActive(true);
        }

        #endregion //Client Impl

    }

}