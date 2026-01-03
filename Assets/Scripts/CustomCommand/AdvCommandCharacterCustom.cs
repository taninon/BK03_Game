using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class AdvCommandCharacterCustom : AdvCommandCharacter
{
	public AdvCommandCharacterCustom(StringGridRow row, AdvSettingDataManager dataManager)
		: base(row, dataManager)
	{

	}


	public override void DoCommand(AdvEngine engine)
	{
		base.DoCommand(engine);

		IAdvMessageWindow window;

		if (!engine.MessageWindowManager.UiMessageWindowManager.AllWindows.TryGetValue("Fukidashi", out window))
		{
			Debug.LogError("FukidashiWindowが取得できなかった");
			return;
		}

		var fukidashiWindow = window as AdvUguiFukidashiMessageWindow;
		if (fukidashiWindow == null)
		{
			Debug.LogError("AdvUguiFukidashiMessageWindowのキャストに失敗");
			return;
		}

		fukidashiWindow.SetCharacter(ParseCell<string>(AdvColumnName.Arg1),ParseCell<string>("WindowPos"));
	}

}
