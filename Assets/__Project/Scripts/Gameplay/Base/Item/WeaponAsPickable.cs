using UnityEngine;

namespace ReGaSLZR
{

    [RequireComponent(typeof(Collider2D))]
    public class WeaponAsPickable : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private Weapon type;

        //[SerializeField]
        //private uint quantity = 1;

        #endregion //Inspector Fields

        #region Accessors

        public Weapon Type => type;
        //public uint Quantity => quantity;

        #endregion //Accessors

        private void OnEnable()
        {
            gameObject.tag = Tag.Item.ToString();
        }

    }

}