using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace ReGaSLZR
{

    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterItemPicker : MonoBehaviour
    {

        [SerializeField]
        private GameObject fxOnPickup;

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

            this.OnCollisionExit2DAsObservable()
                .Where(coll => rWeaponPicked.Value != null)
                .Where(coll => coll.gameObject.GetHashCode() 
                        == rWeaponPicked.Value.gameObject.GetHashCode())
                .Subscribe(_ => rWeaponPicked.Value = null)
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