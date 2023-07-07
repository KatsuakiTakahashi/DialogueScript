using System;
using System.Threading.Tasks;

namespace DialogueScript
{
    public class Command
    {
        // 命令の種類
        public readonly Func<string, Task> type = null;
        // 命令の引数
        public readonly string argument = "";

        // 命令の種類と引数を設定してコマンドを作成します
        public Command(string commandType, string commandArgument)
        {
            if (DialogueScript.CreateCommand(commandType) == null)
            {
                throw new ArgumentNullException("存在しない命令です。 : <" + commandType + '>' + commandArgument);
            }

            type = DialogueScript.CreateCommand(commandType);
            argument = commandArgument.Trim();
        }
    }
}