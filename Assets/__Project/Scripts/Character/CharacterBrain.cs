using NaughtyAttributes;
using UnityEngine;
using UniRx;

namespace ReGaSLZR
{

    public class CharacterBrain : MonoBehaviour
    {

        #region Inspector Fields

        [Header("Components")]

        [SerializeField]
        [Required]
        private CharacterMovement movt;

        [SerializeField]
        [Required]
        private CharacterStatsView statsView;

        [SerializeField]
        [Required]
        private CharacterItemPicker itemPicker;

        //[Space]



        #endregion //Inspector Fields

        #region Unity Callbacks

        private void Start()
        {
            
        }

        #endregion //Unity Callbacks

    }

}