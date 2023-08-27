using UnityEngine;

namespace ReGaSLZR
{

    [System.Serializable]
    public class WeaponIconModel
    {

        #region Inspector Fields

        [SerializeField]
        private Weapon weapon;

        [SerializeField]
        private Sprite icon;

        #endregion //Inspector Fields

        #region Accessors

        public Weapon Weapon => weapon;
        public Sprite Icon => icon;

        #endregion //Accessors

    }
}