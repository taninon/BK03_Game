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
	[SerializeField] PreviousFukidashi PreviousFukidashi;
	bool isAnimation;
	bool isShow = false;
	private float fukidashiUpperYLimit;
	private float fukidashiLowerYLimit;

	[SerializeField] Vector2 minSize;
	[SerializeField] Vector2 maxSize;

	[SerializeField] Vector2 overlapOffset;
	float characterWidth{get {return (messageText.characterSpacing / 100 * messageText.fontSize) + messageText.fontSize;}}

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
	}

	private CharacterFukidashiState currentFukidashiState = new CharacterFukidashiState();
	private CharacterFukidashiState beforeFukidashiState = new CharacterFukidashiState();

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

		currentFukidashiState.characterLabel = name;
		currentFukidashiState.windowPosLabel = windowPos;
    }

	private void AdjustSize(string text)
	{
		var textOffsetX = Mathf.Abs(messageTextRectTrans.offsetMin.x)+ Mathf.Abs(messageTextRectTrans.offsetMax.x);
		var maxLineCount = (maxSize.x - textOffsetX)/ characterWidth;

		if(text.Count() < maxLineCount)
		{
			rootRectTrans.SetWidth(Mathf.Clamp((text.Count() + 1) * characterWidth + textOffsetX,minSize.x,maxSize.x));
			rootRectTrans.SetHeight(minSize.y);
		}
		else
		{
			rootRectTrans.SetWidth(maxSize.x);
			var textOffsetY = Mathf.Abs(messageTextRectTrans.offsetMin.y) + Mathf.Abs(messageTextRectTrans.offsetMax.y);
			var rowCount = 2 + text.Count() / Mathf.CeilToInt((maxSize.x - textOffsetX) / characterWidth);
			rootRectTrans.SetHeight(Mathf.Clamp(rowCount * characterWidth + textOffsetY,minSize.y,maxSize.y));
		}
	}

	public override void OnTextChanged(AdvMessageWindow window)
    {
		if (isShow)
		{
			PreviousFukidashi.SetUsed(rootRectTrans,TextPro.TextMeshPro.text);
		}
		
        base.OnTextChanged(window);
        AdjustSize(window.Text.OriginalText);

        if (engine.Page.CharacterLabel == null)
        {
            Debug.Log("キャラクター指定なしの吹き出し指定");
            return;
        }

        SetDisplayAnimation();
    }

    private void SetDisplayAnimation()
    {
        var windowPosLabel = GetAutoWindowPosLabel(currentFukidashiState.windowPosLabel);
		Vector2 mouthPos = GetMouthPos(windowPosLabel);

        SetPosition(mouthPos);
		var toMovePos = GetAdjustPosition(GetPosByRowLabel(windowPosLabel) + mouthPos);
	
		if(PreviousFukidashi.RootPosition == toMovePos)
		{
			toMovePos += overlapOffset;
		}

       DoMove(toMovePos);
    }

	private Vector2 GetMouthPos(string windowPosLabel)
	{
        if (windowPosLabel.ToLower() == "pov")
        {
			return new Vector2(engine.Param.GetParameterFloat("fukidashiPovMousePosX"), engine.Param.GetParameterFloat("fukidashiPovMousePosY"));
		}
		else
		{
	        var targetCharacter = engine.GraphicManager.CharacterManager.AllGraphics().FirstOrDefault(g => g.name == engine.Page.CharacterLabel).RenderObject;
    	    return GetCharacterPos(targetCharacter) + GetPosByRowLabel("MouthPos");
		}
	}

    private string GetAutoWindowPosLabel(string windowPosLabel)
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
			isShow = true;
			isAnimation = false;
        });
	}
}
