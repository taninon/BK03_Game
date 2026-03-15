using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
    public class CustomCommandManager : AdvCustomCommandManager
    {
        [SerializeField] AdvUguiFukidashiMessageWindow fukidashiWindow;

        public override void OnBootInit()
        {
            Utage.AdvCommandParser.OnCreateCustomCommandFromID += CreateCustomCommand;
        }

        //AdvEnginのクリア処理のときに呼ばれる
        public override void OnClear()
        {
        }

        //カスタムコマンドの作成用コールバック
        public void CreateCustomCommand(string id, StringGridRow row, AdvSettingDataManager dataManager, ref AdvCommand command)
        {
            switch (id)
            {
                //既存のコマンドを改造コマンドに変えたい場合は、IDで判別
                //コメントアウトを解除すれば、テキスト表示がデバッグログ出力のみに変わる
                case AdvCommandParser.IdCharacter:
                    command = new AdvCommandCharacterCustom(row, dataManager,fukidashiWindow);
                    break;
                case AdvCommandParser.IdText:
                    command = new AdvCommandTextCustom(row, dataManager);
                    break;
                //新しい名前のコマンドを作る
                case "DebugLog":
                    break;
            }

            switch (id.ToLower())
            {
                  case "fukidasi":
                    command = new AdvCommandFukidashi(row,fukidashiWindow);
                    break;
                case "fukidasitype":
                    command = new AdvCommandFukidashiType(row,fukidashiWindow);
                    break;
                case "fukidashikeep":
                    command = new AdvCommandKeepFukidashi(row,fukidashiWindow);
                    break;
                case "fukidashioffset":
                    command = new AdvCommandFukidashiOffset(row,fukidashiWindow);
                    break;
            }
        }
    }
}