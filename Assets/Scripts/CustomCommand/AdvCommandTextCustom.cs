using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utage;
using System.Linq;
public class AdvCommandTextCustom : AdvCommandText
{
    public AdvCommandTextCustom(StringGridRow row, AdvSettingDataManager dataManager) : base(row, dataManager)
    {

    }

    public override void DoCommand(AdvEngine engine)
    {
        base.DoCommand(engine);

        var a = ParseCellOptional<string>("WindowPos", "");
        //Debug.Log("WindowPos" + a);
        return;

        //吹き出しの場合の処理
        if (engine.MessageWindowManager.CurrentWindow.Name == "Fukidashi")
        {
            var messageWindow = engine.MessageWindowManager.CurrentWindow.MessageWindow as AdvUguiFukidashiMessageWindow;
            if (!messageWindow)
            {
                return;
            }

            var targetCharacter = engine.GraphicManager.CharacterManager.AllGraphics().FirstOrDefault(g => g.name == engine.Page.CharacterLabel).RenderObject;

            Debug.Log("x:" + targetCharacter.gameObject.transform.position.x);
            Debug.Log("y:" + targetCharacter.gameObject.transform.position.y);

            messageWindow.SetPosition(GetCharacterPos(targetCharacter) + GetMouthPos(engine));
            
//            var currentRect = messageWindow.RootChildren.transform as RectTransform;
//            var mouthPosCorrection = new Vector2(engine.Page.CharacterInfo.Graphic.Main.RowData.ParseCell<float>("MouthPosX"), engine.Page.CharacterInfo.Graphic.Main.RowData.ParseCell<float>("MouthPosY"));
//            currentRect.anchoredPosition = new Vector2(targetCharacter.gameObject.transform.position.x * 100f, targetCharacter.gameObject.transform.position.y * 100f) + mouthPosCorrection;
        }
    }

    private Vector2 GetCharacterPos(AdvGraphicBase targetCharacter)
    {
        return new Vector2(targetCharacter.gameObject.transform.position.x * 100f, targetCharacter.gameObject.transform.position.y * 100f);
    }

    private Vector2 GetMouthPos(AdvEngine engine)
    {
        return new Vector2(engine.Page.CharacterInfo.Graphic.Main.RowData.ParseCell<float>("MouthPosX"), engine.Page.CharacterInfo.Graphic.Main.RowData.ParseCell<float>("MouthPosY"));
    }

}
