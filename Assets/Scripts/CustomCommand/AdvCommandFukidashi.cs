using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class AdvCommandFukidashi : AdvCommand
{
    string offsetX;
    string offsetY;

    public AdvCommandFukidashi(StringGridRow row, AdvUguiFukidashiMessageWindow fukidashi) : base(row)
    {
        this.offsetX = ParseCell<string>(AdvColumnName.Arg1).ToLower();
        this.offsetY = ParseCell<string>(AdvColumnName.Arg1).ToLower();
    }

	//コマンド実行
	public override void DoCommand(AdvEngine engine)
	{
        switch (offsetX)
        {
            case "left":
                break;
            case "right":
                break;
            case "center":
                break;
        }            
	}

}
