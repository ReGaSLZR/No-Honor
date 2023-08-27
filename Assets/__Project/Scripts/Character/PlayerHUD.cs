using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReGaSLZR
{

    public class PlayerHUD : MonoBehaviour
    {

        #region Inspector Fields

        [Header("UI Elements")]

        [SerializeField]
        private Image imageWeaponIcon;

        [SerializeField]
        private TextMeshProUGUI textWeaponUse;

        [Space]

        [SerializeField]
        private GameObject labelNoWeapon;

        [SerializeField]
        private GameObject labelHasWeapon;

        [Header("Settings")]

        [SerializeField]
        private WeaponIconModel[] weaponIcons;

        #endregion //Inspector Fields

        #region Unity Callbacks


        #endregion //Unity Callbacks

        #region Public API



        #endregion //Public API

    }

}