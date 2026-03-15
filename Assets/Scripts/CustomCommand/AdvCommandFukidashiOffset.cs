using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class AdvCommandFukidashiOffset :  AdvCommand
{

	AdvUguiFukidashiMessageWindow fukidashiWindow;

    public AdvCommandFukidashiOffset(StringGridRow row, AdvUguiFukidashiMessageWindow fukidashi) : base(row)
    {
        fukidashiWindow = fukidashi;
    }

	//コマンド実行
	public override void DoCommand(AdvEngine engine)
	{
        var x = ParseCellOptional<float>("Arg1", 0);
        var y = ParseCellOptional<float>("Arg2", 0);
        fukidashiWindow.SetFukidashiRootOffset(new Vector2(x,y));
	}
}
