using UnityEngine;

namespace ReGaSLZR
{

    [RequireComponent(typeof(Collider2D))]
    public class WeaponAsPickable : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private Weapon type;

        #endregion //Inspector Fields

        #region Accessors

        public Weapon Type => type;

        #endregion //Accessors

        private void OnEnable()
        {
            gameObject.tag = Tag.Item.ToString();
        }

    }

}