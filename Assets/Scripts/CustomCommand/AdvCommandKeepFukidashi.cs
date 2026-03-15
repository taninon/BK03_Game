using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class AdvCommandKeepFukidashi : AdvCommand
{
	AdvUguiFukidashiMessageWindow fukidashiWindow;

    public AdvCommandKeepFukidashi(StringGridRow row, AdvUguiFukidashiMessageWindow fukidashi) : base(row)
    {
        fukidashiWindow = fukidashi;
    }

	//コマンド実行
	public override void DoCommand(AdvEngine engine)
	{
        fukidashiWindow.SetKeepFukidashi(ParseCellOptional<bool>("Arg1", false));
	}
}
