using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DialogueScript
{
    public class DialogueScript : MonoBehaviour
    {
        // �V�i���I�t�@�C���̃t�@�C�������w�肵�Ă��������B
        private static readonly string scenarioFileName = "Scenario.txt";

        // �V�i���I�t�@�C������f�[�^�����o���܂�
        private static readonly ScenarioFile scenarioFile = new (scenarioFileName);

        // �V�i���I�t�@�C���̃f�[�^����R�}���h���X�g���쐬���܂�
        private readonly CommandList commandList = new (scenarioFile.scenario, scenarioFile.ScenarioFormat);
        private Func<string, Task> commandMethod = null;

        // �I������
        private bool isEnd = false;
        // �V�i���I�����������삷�邩�m�F�o���܂��B
        [SerializeField]
        private bool isDebugMode = false;
        private bool isSkip = false;
        private int skipSpeed = 100;
        private bool isAuto = false;

        private GameObject dialogueGameObject = null;
        // ���b�Z�[�W�̕\������w�肵�Ă�������
        [SerializeField]
        private TextMeshProUGUI messageTextBox = null;
        // �����̕\���Ԋu
        private int messageInterval = 20;
        // �����҂̖��O�̕\������w�肵�Ă�������
        [SerializeField]
        private TextMeshProUGUI nameTextBox = null;
        private Character[] characters = null;

        // ���݂̖��ߔԍ�
        private int currentCommandNum = -1;

        // ���߂�ǉ��������ꍇ�͂����Őݒ���s���Ă��������B
        [SerializeField]
        private List<UserCommand> userCommands = new ();

        private void Awake()
        {
            dialogueGameObject = gameObject;
            characters = FindObjectsOfType<Character>();
        }

        private void Start()
        {
            Debug.Log(scenarioFile.scenario);

            if (isDebugMode)
            {
                while (!isEnd)
                {
                    NextCommand();
                }
            }
            else
            {
                NextCommand();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                NextCommand();
            }
        }

        // �n���ꂽ���O�̊֐��̃f���Q�[�g�^���쐬���܂��B
        public static Func<string, Task> CreateCommand(string commandType)
        {
            MethodInfo methodInfo = typeof(DialogueScript).GetMethod(commandType);

            if (methodInfo == null)
            {
                return null;
            }

            return (Func<string, Task>)Delegate.CreateDelegate(typeof(Func<string, Task>), null, methodInfo);
        }

        // ���̖��߂����s���܂��B
        public Task NextCommand()
        {
            if (isEnd)
            {
                throw new Exception("���ׂĂ̖��߂��I����Ă��܂������߂̎��s���Ăяo����܂����B");
            }

            // �Ō�̖��߂�������I�������L�������܂��B
            if (currentCommandNum == commandList.commands.Count - 1)
            {
                isEnd = true;
                return Task.CompletedTask;
            }

            // ���݂̖��ߔԍ��𑝂₵�܂��B
            currentCommandNum++;

            // ���߂̌^��ݒ肵�܂��B
            commandMethod = commandList.commands[currentCommandNum].type;
            // ���߂̈�����ݒ肵�Ď��s���܂��B
            commandMethod.DynamicInvoke(this, new string(commandList.commands[currentCommandNum].argument));

            // �Ō�̖��߂�������I�������L�������܂��B
            if (currentCommandNum == commandList.commands.Count - 1)
            {
                isEnd = true;
            }

            return Task.CompletedTask;
        }

        // Dialogue�X�N���v�g�̕\����\����ݒ肵�܂��B
        public�@Task SetActive (string boolText)
        {
            if (boolText == "true")
            {
                dialogueGameObject.SetActive(true);

                return Task.CompletedTask;
            }
            else if (boolText == "false")
            {
                dialogueGameObject.SetActive(false);

                return Task.CompletedTask;
            }
            else
            {
                throw new Exception("<SetActive>�̌�ɂ� true �������́@false�@��ݒ肵�Ă��������B");
            }
        }

        // �e�L�X�g�{�b�N�X�ɕ�����\�����܂�
        public async Task Message(string messageText)
        {
            if (messageTextBox == null)
            {
                throw new ArgumentNullException("���b�Z�[�W�̕\�����ݒ肵�Ă�������");
            }

            if (isSkip)
            {
                // �e�L�X�g�{�b�N�X����ɂ��܂�
                messageTextBox.text = "";

                for (int messageTextIndex = 0; messageTextIndex < messageText.Length; messageTextIndex++)
                {
                    await Task.Delay(skipSpeed / messageText.Length);

                    messageTextBox.text += messageText[messageTextIndex];
                }

                await Task.Delay(skipSpeed);
                await NextCommand();
            }
            else if (messageInterval != 0)
            {
                // �e�L�X�g�{�b�N�X����ɂ��܂�
                messageTextBox.text = "";

                for (int messageTextIndex = 0; messageTextIndex < messageText.Length; messageTextIndex++)
                {
                    await Task.Delay(messageInterval);

                    messageTextBox.text += messageText[messageTextIndex];
                }
            }
            else
            {
                messageTextBox.text = messageText;
            }
        }

        // �����̕\���Ԋu��ݒ肵�܂��B
        public Task MessageInterval(string messageIntervalText)
        {
            // �n���ꂽ�����������ȊO��������ݒ肵�܂���B
            if (!int.TryParse(messageIntervalText, out int value))
            {
                throw new Exception("���b�Z�[�W�C���^�[�o���̐ݒ�l�ɐ����ł͂Ȃ����̂��n����܂����B : " + messageIntervalText);
            }

            messageInterval = value;

            NextCommand();

            return Task.CompletedTask;
        }

        // �����҂̖��O��\�����܂��B
        public Task Name(string nameText)
        {
            if (nameTextBox == null)
            {
                throw new ArgumentNullException("�����҂̖��O�̕\�����ݒ肵�Ă�������");
            }

            nameTextBox.text = nameText;

            NextCommand();

            return Task.CompletedTask;
        }

        // �L�����N�^�[�̏�Ԃ�ω������܂��B
        public Task CharacterMode(string characterNameAndModeNameText)
        {
            bool isCharacterName = true;
            string characterName = "";
            string modeName = "";

            for (int textIndex = 0; textIndex < characterNameAndModeNameText.Length; textIndex++)
            {
                if ((characterNameAndModeNameText[textIndex] == '_' || characterNameAndModeNameText[textIndex] == '�Q') && isCharacterName)
                {
                    textIndex++;
                    isCharacterName = false;
                }

                if (isCharacterName)
                {
                    characterName += characterNameAndModeNameText[textIndex];
                }
                else
                {
                    modeName += characterNameAndModeNameText[textIndex];
                }
            }

            for (int characterIndex = 0; characterIndex < characters.Length; characterIndex++)
            {
                if (characters[characterIndex].CharacterName == characterName)
                {
                    characters[characterIndex].SetCharacterMode(modeName);

                    NextCommand();

                    return Task.CompletedTask;
                }
            }

            throw new ArgumentNullException("���݂��Ȃ��L�����N�^�[�������͑��݂��Ȃ����[�h�ł��B : " + characterNameAndModeNameText);
        }

        // �V�[�����ړ����܂��B
        public Task LoadScene(string sceneNameText)
        {
            SceneManager.LoadScene(sceneNameText);

            return Task.CompletedTask;
        }

        // ���[�U�[���쐬�������߂ł��B
        public Task UserCommand(string commandNameText)
        {
            // ���߂̖��̂���v�f�ԍ���ݒ肵�܂��B
            int index = userCommands.FindIndex(item => item.CommandName == commandNameText);

            if (index == -1)
            {
                throw new Exception("�쐬����ĂȂ����߂��Ԉ�������̂��n����܂����B : " + commandNameText);
            }

            userCommands[index].CommandEvent.Invoke();

            NextCommand();

            return Task.CompletedTask;
        }
    }
}