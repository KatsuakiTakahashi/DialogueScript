using System.Collections.Generic;

namespace DialogueScript
{
    public class CommandList
    {
        // 命令の種類と命令の引数のデータをもつリストです
        public readonly List<Command> commands = new();

        public CommandList(string scenario, ScenarioFile.Format format)
        {
            if (scenario == "")
            {
                throw new System.Exception("シナリオの中身が存在しません。");
            }

            // 命令型の設定中かどうか
            bool isCommandType = true;

            // 現在の命令型とその引数
            string currentCommandType = "";
            string currentCommandArgument = "";

            // 先頭末尾の空白文字を削除しコマンドを作成したのちリストに格納します
            void CommandSet()
            {
                commands.Add(new Command(currentCommandType, currentCommandArgument));

                currentCommandType = "";
                currentCommandArgument = "";
            }

            switch (format)
            {
                // シナリオファイルの拡張子が.txtだった場合の判別
                case ScenarioFile.Format.txt:
                    // シナリオの最後まで判別
                    for (int scenarioIndex = 0; scenarioIndex < scenario.Length; scenarioIndex++)
                    {
                        // 先頭の','を回避します
                        if (scenarioIndex == 0 && scenario[scenarioIndex] == '<')
                        {
                            scenarioIndex++;
                        }

                        // 命令中ではないときに'<'が来たら命令かどうか判別開始
                        if (scenario[scenarioIndex] == '<' && isCommandType != true)
                        {
                            if (!(scenarioIndex == scenario.Length - 1))
                            {
                                scenarioIndex++;
                            }

                            // 連続して'<'が続いていた場合命令ではないと判定
                            if (scenario[scenarioIndex] != '<')
                            {
                                isCommandType = true;

                                // 命令を設定し命令と命令の引数を初期化
                                CommandSet();
                            }
                        }
                        // 命令中だったときに'>'が来たら命令終了と判定
                        else if (scenario[scenarioIndex] == '>' && isCommandType == true)
                        {
                            if (!(scenarioIndex == scenario.Length - 1))
                            {
                                scenarioIndex++;
                                isCommandType = false;
                            }
                        }
                        // 命令中だったら命令に入力。
                        if (isCommandType)
                        {
                            currentCommandType += scenario[scenarioIndex];
                        }
                        // 命令中じゃなかったら命令の引数に入力。
                        else
                        {
                            currentCommandArgument += scenario[scenarioIndex];
                        }

                        // シナリオの最後まで到達したら命令を設定
                        if (scenarioIndex == scenario.Length - 1)
                        {
                            // 命令を設定し命令と命令の引数を初期化
                            CommandSet();
                        }
                    }
                    break;

                // シナリオファイルの拡張子が.csvだった場合の判別
                case ScenarioFile.Format.csv:
                    // シナリオの最後まで判別
                    for (int scenarioIndex = 0; scenarioIndex < scenario.Length; scenarioIndex++)
                    {
                        // 改行コードがLFだった場合の判別
                        if (scenario[scenarioIndex] == '\n')
                        {
                            if (isCommandType)
                            {
                                isCommandType = false;
                            }
                            else
                            {
                                isCommandType = true;

                                // 命令を設定し命令と命令の引数を初期化
                                CommandSet();
                            }
                        }

                        // 改行コードがCRLFだった場合の判別
                        if (scenario[scenarioIndex] == '\r')
                        {
                            if (!(scenarioIndex == scenario.Length - 1))
                            {
                                scenarioIndex++;
                            }

                            if (scenario[scenarioIndex] == '\n')
                            {
                                if (isCommandType)
                                {
                                    isCommandType = false;
                                }
                                else
                                {
                                    isCommandType = true;

                                    // 命令を設定し命令と命令の引数を初期化
                                    CommandSet();
                                }
                            }
                        }

                        // ','が来た時に命令か判別開始
                        if (scenario[scenarioIndex] == ',')
                        {
                            if (!(scenarioIndex == scenario.Length - 1))
                            {
                                scenarioIndex++;
                            }

                            // 連続して','が来てないときは命令判定を変更
                            if (scenario[scenarioIndex] != ',')
                            {
                                if (isCommandType)
                                {
                                    isCommandType = false;
                                }
                                else
                                {
                                    isCommandType = true;

                                    // 命令を設定し命令と命令の引数を初期化
                                    CommandSet();
                                }
                            }
                        }

                        // 命令中だったら命令に入力。
                        if (isCommandType)
                        {
                            currentCommandType += scenario[scenarioIndex];
                        }
                        // 命令中じゃなかったら命令の引数に入力。
                        else
                        {
                            currentCommandArgument += scenario[scenarioIndex];
                        }

                        // シナリオの最後まで到達したら命令を設定
                        if (scenarioIndex == scenario.Length - 1)
                        {
                            // 命令を設定し命令と命令の引数を初期化
                            CommandSet();
                        }
                    }
                    break;
            }
        }
    }
}
