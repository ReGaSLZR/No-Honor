using NaughtyAttributes;
using UnityEngine;
using UniRx;
using System.Collections;

namespace ReGaSLZR
{

    public class CharacterBrain : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private bool isNPC;

        [Space]

        [SerializeField]
        private uint framesBeforeNPCSetUp = 5;

        [Header("Components")]

        [SerializeField]
        private CharacterMovement movt;

        [SerializeField]
        private CharacterStatsView statsView;

        [SerializeField]
        private CharacterItemPicker itemPicker;

        #endregion //Inspector Fields

        #region Unity Callbacks

        private IEnumerator Start()
        {
            var frames = 0;
            while (frames < framesBeforeNPCSetUp)
            {
                yield return null;
                frames++;
            }

            if (isNPC)
            {
                movt.enabled = false;
                Destroy(itemPicker);
            }
        }

        #endregion //Unity Callbacks

    }

}