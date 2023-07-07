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
        // シナリオファイルのファイル名を指定してください。
        private static readonly string scenarioFileName = "Scenario.txt";

        // シナリオファイルからデータを取り出します
        private static readonly ScenarioFile scenarioFile = new (scenarioFileName);

        // シナリオファイルのデータからコマンドリストを作成します
        private readonly CommandList commandList = new (scenarioFile.scenario, scenarioFile.ScenarioFormat);
        private Func<string, Task> commandMethod = null;

        // 終了判定
        private bool isEnd = false;
        // シナリオが正しく動作するか確認出来ます。
        [SerializeField]
        private bool isDebugMode = false;
        private bool isSkip = false;
        private int skipSpeed = 100;
        private bool isAuto = false;

        private GameObject dialogueGameObject = null;
        // メッセージの表示先を指定してください
        [SerializeField]
        private TextMeshProUGUI messageTextBox = null;
        // 文字の表示間隔
        private int messageInterval = 20;
        // 発言者の名前の表示先を指定してください
        [SerializeField]
        private TextMeshProUGUI nameTextBox = null;
        private Character[] characters = null;

        // 現在の命令番号
        private int currentCommandNum = -1;

        // 命令を追加したい場合はここで設定を行ってください。
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

        // 渡された名前の関数のデリゲート型を作成します。
        public static Func<string, Task> CreateCommand(string commandType)
        {
            MethodInfo methodInfo = typeof(DialogueScript).GetMethod(commandType);

            if (methodInfo == null)
            {
                return null;
            }

            return (Func<string, Task>)Delegate.CreateDelegate(typeof(Func<string, Task>), null, methodInfo);
        }

        // 次の命令を実行します。
        public Task NextCommand()
        {
            if (isEnd)
            {
                throw new Exception("すべての命令が終わっていますが命令の実行が呼び出されました。");
            }

            // 最後の命令だったら終了判定を有効化します。
            if (currentCommandNum == commandList.commands.Count - 1)
            {
                isEnd = true;
                return Task.CompletedTask;
            }

            // 現在の命令番号を増やします。
            currentCommandNum++;

            // 命令の型を設定します。
            commandMethod = commandList.commands[currentCommandNum].type;
            // 命令の引数を設定して実行します。
            commandMethod.DynamicInvoke(this, new string(commandList.commands[currentCommandNum].argument));

            // 最後の命令だったら終了判定を有効化します。
            if (currentCommandNum == commandList.commands.Count - 1)
            {
                isEnd = true;
            }

            return Task.CompletedTask;
        }

        // Dialogueスクリプトの表示非表示を設定します。
        public　Task SetActive (string boolText)
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
                throw new Exception("<SetActive>の後には true もしくは　false　を設定してください。");
            }
        }

        // テキストボックスに文字を表示します
        public async Task Message(string messageText)
        {
            if (messageTextBox == null)
            {
                throw new ArgumentNullException("メッセージの表示先を設定してください");
            }

            if (isSkip)
            {
                // テキストボックスを空にします
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
                // テキストボックスを空にします
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

        // 文字の表示間隔を設定します。
        public Task MessageInterval(string messageIntervalText)
        {
            // 渡された引数が数字以外だったら設定しません。
            if (!int.TryParse(messageIntervalText, out int value))
            {
                throw new Exception("メッセージインターバルの設定値に数字ではないものが渡されました。 : " + messageIntervalText);
            }

            messageInterval = value;

            NextCommand();

            return Task.CompletedTask;
        }

        // 発言者の名前を表示します。
        public Task Name(string nameText)
        {
            if (nameTextBox == null)
            {
                throw new ArgumentNullException("発言者の名前の表示先を設定してください");
            }

            nameTextBox.text = nameText;

            NextCommand();

            return Task.CompletedTask;
        }

        // キャラクターの状態を変化させます。
        public Task CharacterMode(string characterNameAndModeNameText)
        {
            bool isCharacterName = true;
            string characterName = "";
            string modeName = "";

            for (int textIndex = 0; textIndex < characterNameAndModeNameText.Length; textIndex++)
            {
                if ((characterNameAndModeNameText[textIndex] == '_' || characterNameAndModeNameText[textIndex] == '＿') && isCharacterName)
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

            throw new ArgumentNullException("存在しないキャラクターもしくは存在しないモードです。 : " + characterNameAndModeNameText);
        }

        // シーンを移動します。
        public Task LoadScene(string sceneNameText)
        {
            SceneManager.LoadScene(sceneNameText);

            return Task.CompletedTask;
        }

        // ユーザーが作成した命令です。
        public Task UserCommand(string commandNameText)
        {
            // 命令の名称から要素番号を設定します。
            int index = userCommands.FindIndex(item => item.CommandName == commandNameText);

            if (index == -1)
            {
                throw new Exception("作成されてない命令か間違った名称が渡されました。 : " + commandNameText);
            }

            userCommands[index].CommandEvent.Invoke();

            NextCommand();

            return Task.CompletedTask;
        }
    }
}