using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class AdvCommandCharacterCustom : AdvCommandCharacter
{
	AdvUguiFukidashiMessageWindow fukidashiWindow;
	public AdvCommandCharacterCustom(StringGridRow row, AdvSettingDataManager dataManager,AdvUguiFukidashiMessageWindow fukidashiWindow)
		: base(row, dataManager)
	{
		this.fukidashiWindow = fukidashiWindow;
	}


	public override void DoCommand(AdvEngine engine)
	{
		base.DoCommand(engine);
		SetCharacter(engine);
	}

	private void SetCharacter(AdvEngine engine)
	{
		if(fukidashiWindow == null)
		{
			return;
		}

		fukidashiWindow.SetCustomParam(this.RowData);
	}

}
