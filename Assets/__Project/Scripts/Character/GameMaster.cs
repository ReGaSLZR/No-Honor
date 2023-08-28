using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReGaSLZR
{

    public class GameMaster : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private PlayerHUD viewHud;

        [SerializeField]
        private LeaderboardView leaderboardView;

        [SerializeField]
        [Tooltip("The delay is to wait for the Start() in the Character to be done first.")]
        [Range(0, 10)]
        private uint frameDelayRegister = 1;

        #endregion //Inspector Fields

        private readonly List<Character> characters = new List<Character>();

        #region Client Impl

        private IEnumerator C_DelayedSetUpOfCharacter(Character character, bool isBot)
        {
            uint delay = 0;
            while (delay < frameDelayRegister)
            {
                yield return null;
                delay++;
            }

            character.SetUp(isBot, viewHud);
        }

        #endregion //Client Impl

        #region Public API

        public void RegisterCharacter(Character character, bool isBot)
        {
            characters.Add(character);
            StartCoroutine(C_DelayedSetUpOfCharacter(character, isBot));
        }

        #endregion //Public API

    }

}