using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class AdvCommandFukidashiType : AdvCommand
{

	AdvUguiFukidashiMessageWindow fukidashiWindow;

    public AdvCommandFukidashiType(StringGridRow row, AdvUguiFukidashiMessageWindow fukidashi) : base(row)
    {
        fukidashiWindow = fukidashi;
    }

	//コマンド実行
	public override void DoCommand(AdvEngine engine)
	{
        var type = ParseCellOptional<string>("Arg1", "");
        fukidashiWindow.SetFukidashiBack(type);
	}

}
