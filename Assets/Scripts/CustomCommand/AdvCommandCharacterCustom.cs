using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class AdvCommandCharacterCustom : AdvCommandCharacter
{
		public AdvCommandCharacterCustom(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row,dataManager)
		{

		}

        
		public override void DoCommand(AdvEngine engine)
		{
            base.DoCommand(engine);
			Debug.Log("MousePosX"+characterInfo.Graphic.Main.RowData.ParseCell<float>("MousePosX"));
			Debug.Log("MousePosX"+characterInfo.Graphic.Main.RowData.ParseCell<float>("MousePosY"));
		}
}
