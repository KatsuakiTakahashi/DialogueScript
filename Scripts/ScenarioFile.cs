using System.IO;
using System.Text;

namespace DialogueScript
{
    public class ScenarioFile
    {
        // シナリオファイルの中身です。
        public readonly string scenario = "";

        // シナリオファイルの拡張子の情報です。
        public enum Format
        {
            txt,
            csv,
        }
        // 現在のシナリオファイルの拡張子です。
        public Format ScenarioFormat { get; private set; } = Format.txt;

        // ファイルネームから自動的に拡張子を判別し中身をscenarioに入れます。
        public ScenarioFile(string scenarioFileName)
        {
            if (scenarioFileName == "")
            {
                throw new System.Exception("シナリオファイルのファイル名を指定してください。");
            }

            // ファイル名からファイルパスを作成します。ファイルパスを変更したい場合はここを変更してください。
            string scenarioFilePath = "Assets/DialogueScript/Scenarios/" + scenarioFileName;

            if (!File.Exists(scenarioFilePath))
            {
                throw new System.Exception("指定されたファイルが見つかりませんでした。\nファイル名 : " + scenarioFileName + "\nファイルパス : " + scenarioFilePath);
            }

            // ファイルネームから拡張子を判別して設定します。
            switch (Path.GetExtension(scenarioFileName))
            {
                case ".txt":
                    ScenarioFormat = Format.txt;
                    break;
                case ".csv":
                    ScenarioFormat = Format.csv;
                    break;
                default:
                    throw new System.Exception("シナリオファイルは拡張子が.txtか.csvのものを使用してください。");
            }

            // ファイルのバイト数からエンコーディングを判別します。
            Encoding encoding;
            byte[] scenarioFileBytes = File.ReadAllBytes(scenarioFilePath);
            using (MemoryStream stream = new MemoryStream(scenarioFileBytes))
            {
                using (StreamReader reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true))
                {
                    // StreamReaderのCurrentEncodingプロパティでエンコーディングを取得
                    encoding = reader.CurrentEncoding;
                }
            }

            // scenarioにシナリオファイルの中身を入れます。
            using (StreamReader streamReader = new(scenarioFilePath, encoding))
            {
                this.scenario = streamReader.ReadToEnd();
                streamReader.Close();
            }
        }
    }
}