using System.Collections;
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
        private CanvasGroup movementInstructions;

        [Space]

        [SerializeField]
        private GameObject labelNoWeapon;

        [SerializeField]
        private GameObject labelHasWeapon;

        [Header("Settings")]

        [SerializeField]
        [Range(5f, 15f)]
        private float delayInstructionsCrossfade;

        [SerializeField]
        [Range(3f, 10f)]
        private float durationInstructionsCrossfade;

        [SerializeField]
        private WeaponIconModel[] weaponIcons;

        #endregion //Inspector Fields

        #region Unity Callbacks

        private void Start()
        {
            StartCoroutine(C_DelayHideInstructions());
            UpdateWeapon(Weapon.None, 0);
        }

        #endregion //Unity Callbacks

        #region Public API

        public void UpdateWeapon(Weapon weapon, uint quantity = 0)
        {
            imageWeaponIcon.sprite = GetWeaponSprite(weapon);

            var hasWeapon = weapon != Weapon.None;
            labelHasWeapon.SetActive(hasWeapon);
            labelNoWeapon.SetActive(!hasWeapon);
            textWeaponUse.text = quantity.ToString();
            textWeaponUse.gameObject.SetActive(quantity > 0);
        }

        #endregion //Public API

        #region Client Impl

        private IEnumerator C_DelayHideInstructions()
        {
            yield return new WaitForSeconds(delayInstructionsCrossfade);

            var timeElapsed = 0f;
            while (timeElapsed < durationInstructionsCrossfade)
            {
                movementInstructions.alpha = Mathf.Lerp(1f, 0f, 
                    timeElapsed / durationInstructionsCrossfade);
                yield return null;
                timeElapsed += Time.deltaTime;
            }
            
        }

        private Sprite GetWeaponSprite(Weapon weapon)
        {
            foreach (var icon in weaponIcons)
            {
                if (weapon == icon.Weapon)
                {
                    return icon.Icon;
                }
            }
            return null;
        }

        #endregion //Client Impl

    }

}