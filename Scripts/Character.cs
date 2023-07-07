using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueScript
{
    public class Character : MonoBehaviour
    {
        // キャラクターの名前を設定してださい。
        [SerializeField]
        private string characterName;
        public string CharacterName { get => characterName; set => characterName = value; }

        private Image characterImage;
        private Animator characterAnimator;

        [System.Serializable]
        private struct CharacterMode
        {
            [SerializeField]
            private string modeName;
            public string ModeName { get => modeName; set => modeName = value; }

            [SerializeField]
            private Sprite sprite;
            public Sprite Sprite { get => sprite; set => sprite = value; }

            [SerializeField]
            private string animationTrigger;
            public string AnimationTrigger { get => animationTrigger; set => animationTrigger = value; }
        }
        [SerializeField]
        private List<CharacterMode> characterModes;

        private void Awake()
        {
            if (CharacterName == null)
            {
                throw new System.Exception("キャラクターの名前を設定してください。");
            }

            characterImage = GetComponentInChildren<Image>();
            characterAnimator = GetComponent<Animator>();
        }

        // キャラクターの状態を設定して反映させます。
        public void SetCharacterMode(string modeNameText)
        {
            for (int characterModeIndex = 0; characterModeIndex < characterModes.Count; characterModeIndex++)
            {
                if(characterModes[characterModeIndex].ModeName == modeNameText)
                {
                    if(characterModes[characterModeIndex].Sprite != null)
                    {
                        characterImage.sprite = characterModes[characterModeIndex].Sprite;
                    }

                    if (characterModes[characterModeIndex].AnimationTrigger != "")
                    {
                        characterAnimator.SetTrigger(characterModes[characterModeIndex].AnimationTrigger);
                    }

                    return;
                }
            }

            throw new System.Exception("存在しないキャラクターモードです。 : " + modeNameText);
        }
    }
}