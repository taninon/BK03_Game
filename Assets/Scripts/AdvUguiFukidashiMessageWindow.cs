using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;
using System.Linq;
using System;
using DG.Tweening;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UtageExtensions;
using TMPro;
using MoreMountains.Tools;

public class AdvUguiFukidashiMessageWindow : AdvUguiMessageWindowTMP
{
	public GameObject RootChildren { get { return rootChildren; } }
	[SerializeField] RectTransform rootRectTrans;
	[SerializeField] TextMeshProUGUI messageText;
	[SerializeField] RectTransform messageTextRectTrans;
	bool isAnimation;

	private float fukidashiUpperYLimit;
	private float fukidashiLowerYLimit;

	[SerializeField] Vector2 minSize;
	[SerializeField] Vector2 maxSize;


    public override void OnInit(AdvMessageWindowManager windowManager)
    {
        base.OnInit(windowManager);
		SetConstVariable().Forget();
    }

	private async UniTaskVoid SetConstVariable()
	{
		await UniTask.WaitUntil(() =>engine.Param.IsInit);
		fukidashiUpperYLimit = engine.Param.GetParameterFloat("fukidashiUpperYLimit");
		fukidashiLowerYLimit = engine.Param.GetParameterFloat("fukidashiLowerYLimit");
	}

	internal struct CharacterFukidashiState
	{
		internal string characterLabel;
		internal string windowPosLabel;
		internal Vector2 rootPos;

		internal CharacterFukidashiState(string characterLabel,string windowPosLabel,Vector2 pos)
		{
			this.characterLabel = characterLabel;
			this.windowPosLabel = windowPosLabel;
			this.rootPos = pos;
		}
	}

	IEnumerable<CharacterFukidashiState> characterFukidashiStates;

	private CharacterFukidashiState currentFukidashiState;
	private CharacterFukidashiState beforeFukidashiState;

	public void SetPosition(Vector2 pos)
	{
		currentFukidashiState.rootPos = GetAdjustPosition(pos);
		rootRectTrans.anchoredPosition = currentFukidashiState.rootPos;
	}

	private Vector2 GetAdjustPosition(Vector2 pos)
	{
		var returnPos = pos;
		returnPos.y = Mathf.Clamp(pos.y,fukidashiLowerYLimit,fukidashiUpperYLimit);
		return returnPos;
	}

	private int GetLineCount(string text)
	{
		var lines = text.Split("\n");
		return lines.Select(_=>_.Count()).OrderByDescending(_=> _).FirstOrDefault();
	}

	private Vector2 GetCharacterPos(AdvGraphicBase targetCharacter)
    {
        return new Vector2(targetCharacter.gameObject.transform.position.x * 100f, targetCharacter.gameObject.transform.position.y * 100f);
    }

	private Vector2 GetPosByRowLabel(string rowLabel)
	{
		string posRaw;
		engine.Page.CharacterInfo.Graphic.Main.RowData.TryParseCell<string>(rowLabel, out posRaw);
		if (posRaw == null)
		{
			return new Vector2(0, 0);
		}
		
		var pos = posRaw.Split("v");
		return new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));
	}
	
	public void SetCharacter(string name,string windowPos)
    {
		beforeFukidashiState = currentFukidashiState;
		currentFukidashiState = new CharacterFukidashiState(name,windowPos,rootRectTrans.anchoredPosition);
    }

	private void AdjustSize(string text)
	{

		var textOffsetX = Mathf.Abs(messageTextRectTrans.offsetMin.x)+ Mathf.Abs(messageTextRectTrans.offsetMax.x);
		var maxLineCount = (maxSize.x - textOffsetX)/messageText.fontSize;

		if(text.Count() < maxLineCount)
		{
			rootRectTrans.SetWidth(Mathf.Clamp((text.Count() + 1) * messageText.fontSize + textOffsetX,minSize.x,maxSize.x));
			rootRectTrans.SetHeight(minSize.y);
		}
		else
		{
			rootRectTrans.SetWidth(maxSize.x);
			var textOffsetY = Mathf.Abs(messageTextRectTrans.offsetMin.y) + Mathf.Abs(messageTextRectTrans.offsetMax.y);
			var rowCount = 1 + text.Count() / Mathf.CeilToInt((maxSize.x - textOffsetX) / messageText.fontSize);
			rootRectTrans.SetHeight(Mathf.Clamp(rowCount  * messageText.fontSize + textOffsetY,minSize.y,maxSize.y));
		}
	}

	public override void OnTextChanged(AdvMessageWindow window)
	{
		base.OnTextChanged(window);
		Debug.Log("Count"+GetLineCount(window.Text.OriginalText));
		AdjustSize(window.Text.OriginalText);

		if (engine.Page.CharacterLabel== null)
		{
			Debug.Log("キャラクター指定なしの吹き出し指定");
			return;
		}

		var targetCharacter = engine.GraphicManager.CharacterManager.AllGraphics().FirstOrDefault(g => g.name == engine.Page.CharacterLabel).RenderObject;
		var mouthPos = GetCharacterPos(targetCharacter) + GetPosByRowLabel("MouthPos");
		var windowPosLabel = GetAutoWindowPosLabel(currentFukidashiState.windowPosLabel,targetCharacter);

		var toMovePos = GetAdjustPosition(GetPosByRowLabel(windowPosLabel) + mouthPos); 
		SetPosition(mouthPos);
		DoMove(toMovePos);
	}

	private string GetAutoWindowPosLabel(string windowPosLabel,AdvGraphicBase character)
	{
		if(windowPosLabel == null)
		{
			return "LeftUp";
		}
		return currentFukidashiState.windowPosLabel;
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

		rootRectTrans.transform.localScale = Vector3.zero;
		rootRectTrans.DOScale(Vector3.one,0.2f);
		rootRectTrans.DOLocalMove(toPos, 0.2f).OnComplete(() =>
        {
			isAnimation = false;
        });
	}


}
