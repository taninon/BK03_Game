using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;
using System.Linq;
using System;
using DG.Tweening;
using Unity.VisualScripting;

public class AdvUguiFukidashiMessageWindow : AdvUguiMessageWindowTMP
{
	public GameObject RootChildren { get { return rootChildren; } }

	[SerializeField] RectTransform rootRectTrans;

	[SerializeField] private RectTransform previousRootChildren;

	bool isAnimation;

	private float fukidashiYUpperLimit;
	private float fukidashiYLowerLimit;
    public override void OnInit(AdvMessageWindowManager windowManager)
    {
        base.OnInit(windowManager);
		SetConstVariable();
    }

	private void SetConstVariable()
	{
		fukidashiYUpperLimit = engine.Param.GetParameterFloat("fukidashiYUpperLimit");
		fukidashiYLowerLimit = engine.Param.GetParameterFloat("fukidashiYLowerLimit");
	}

	internal class CharacterFukidashiState
	{
		internal string characterLabel;
		internal string posLabel;
		internal Vector2 rootPos;

		internal CharacterFukidashiState(string characterLabel,string posLabel,Vector2 pos)
		{
			this.characterLabel = characterLabel;
			this.posLabel = posLabel;
			this.rootPos = pos;
		}
	}

	List<CharacterFukidashiState> characterFukidashiStates;

	private CharacterFukidashiState currentFukidashiState;

	public void SetPosition(Vector2 pos)
	{
		currentFukidashiState.rootPos = pos;
		rootRectTrans.anchoredPosition = pos;
	}

	private Vector2 GetAdjustPosition(Vector2 pos)
	{
		var returnPos = pos;
		returnPos.y = Mathf.Clamp(pos.y,fukidashiYLowerLimit,fukidashiYUpperLimit);
		return returnPos;
	}


	private Vector2 GetCharacterPos(AdvGraphicBase targetCharacter)
    {
        return new Vector2(targetCharacter.gameObject.transform.position.x * 100f, targetCharacter.gameObject.transform.position.y * 100f);
    }

	private Vector2 GetPosByRowName(string rowPosName)
	{
		string posRaw;
		engine.Page.CharacterInfo.Graphic.Main.RowData.TryParseCell<string>(rowPosName, out posRaw);
		if (posRaw == null)
		{
			return new Vector2(0, 0);
		}
		
		var pos = posRaw.Split("v");
		return new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));
	}
	
	public void SetCharacter(string name,string windowPos)
    {
		currentFukidashiState = new CharacterFukidashiState(name,windowPos,rootRectTrans.anchoredPosition);
    }

	public override void OnTextChanged(AdvMessageWindow window)
	{
		base.OnTextChanged(window);

		var targetCharacter = engine.GraphicManager.CharacterManager.AllGraphics().FirstOrDefault(g => g.name == engine.Page.CharacterLabel).RenderObject;

		SetPosition(GetCharacterPos(targetCharacter) + GetPosByRowName("MouthPos"));
		DoMove(GetPosByRowName(currentFukidashiState.posLabel));
	}
	
	protected override void UpdateCurrent()
    {
		if (isAnimation)
		{
			return;
		}

		base.UpdateCurrent();
    }

	public void DoMove(UnityEngine.Vector2 toPos)
	{
		isAnimation = true;
		rootRectTrans.DOAnchorPos(toPos, 0.3f).SetRelative().OnComplete(() =>
        {
			isAnimation = false;
        });
	}


}
