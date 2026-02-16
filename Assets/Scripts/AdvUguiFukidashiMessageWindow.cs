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
using System.Text;

public class AdvUguiFukidashiMessageWindow : AdvUguiMessageWindowTMP
{
	public GameObject RootChildren { get { return rootChildren; } }
	[SerializeField] RectTransform rootRectTrans;
	[SerializeField] RectTransform fukidashiBack;
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
	float characterWidth { get { return (messageText.characterSpacing / 100 * messageText.fontSize) + messageText.fontSize; } }

	[SerializeField] bool isTategaki;

	AdvMessageWindow _messageWindow;

	public override void OnInit(AdvMessageWindowManager windowManager)
	{
		base.OnInit(windowManager);
		SetConstVariable().Forget();
	}

	private async UniTaskVoid SetConstVariable()
	{
		await UniTask.WaitUntil(() => engine.Param.IsInit);
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
		fukidashiBack.anchoredPosition = currentFukidashiState.rootPos;

	}

	private Vector2 GetAdjustPosition(Vector2 pos)
	{
		var returnPos = pos;
		returnPos.y = Mathf.Clamp(pos.y, fukidashiLowerYLimit, fukidashiUpperYLimit);
		return returnPos;
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

	public void SetCharacter(string name, string windowPos)
	{
		beforeFukidashiState = currentFukidashiState;

		currentFukidashiState.characterLabel = name;
		currentFukidashiState.windowPosLabel = windowPos;
	}
	private void AdjustSize(int width, int rowCount,RectTransform targetRect)
	{
		var textOffsetX = Mathf.Abs(messageTextRectTrans.offsetMin.x) + Mathf.Abs(messageTextRectTrans.offsetMax.x);
		var setWidth = Mathf.Clamp((width + 1) * characterWidth + textOffsetX, minSize.x, maxSize.x);

		if(setWidth > targetRect.GetWith()){
			targetRect.SetWidth(setWidth);
		}

		var textOffsetY = Mathf.Abs(messageTextRectTrans.offsetMin.y) + Mathf.Abs(messageTextRectTrans.offsetMax.y);
		targetRect.SetHeight(Mathf.Clamp(rowCount * characterWidth * 1.5f + textOffsetY, minSize.y, maxSize.y));

	}


	private void AdjustSize(string text,RectTransform targetRect)
	{
		int rowCount = GetLineCount(text);

		var textOffsetX = Mathf.Abs(messageTextRectTrans.offsetMin.x) + Mathf.Abs(messageTextRectTrans.offsetMax.x);
		targetRect.SetWidth(Mathf.Clamp((GetlongestLengthCount(text) + 1) * characterWidth + textOffsetX, minSize.x, maxSize.x));

		var textOffsetY = Mathf.Abs(messageTextRectTrans.offsetMin.y) + Mathf.Abs(messageTextRectTrans.offsetMax.y);
		targetRect.SetHeight(Mathf.Clamp(rowCount * characterWidth * 1.5f + textOffsetY, minSize.y, maxSize.y));

		/*
				if (rowCount == 1)
				{
					rootRectTrans.SetWidth(Mathf.Clamp((text.Length + 1) * characterWidth + textOffsetX, minSize.x, maxSize.x));
					rootRectTrans.SetHeight(minSize.y);
				}
				else
				{
					rootRectTrans.SetWidth(Mathf.Clamp((GetlongestLengthCount(text) + 1) * characterWidth + textOffsetX, minSize.x, maxSize.x));
					var textOffsetY = Mathf.Abs(messageTextRectTrans.offsetMin.y) + Mathf.Abs(messageTextRectTrans.offsetMax.y);
					rootRectTrans.SetHeight(Mathf.Clamp(rowCount * characterWidth * 1.5f + textOffsetY, minSize.y, maxSize.y));
				}
		*/
	}

	private int GetLineCount(string s)
	{
		int n = 0;
		foreach (var c in s)
		{
			if (c == '\n') n++;
		}
		return n + 1;
	}

	private int GetlongestLengthCount(string s)
	{
		var lines = s.Split('\n');
		return lines.OrderByDescending(x => x.Length).First().Length;
	}

	private (int, int) GetCurrentLineCount(string text, int nowWidth)
	{
		if (string.IsNullOrEmpty(text)) return (0,1);

		var lines = text.Split('\n');
		int totalLines = 1;
		int currentWidth = nowWidth;

		foreach (var line in lines)
		{
			if(line.Length < currentWidth)
			{
				totalLines++;
				currentWidth -= line.Length;
			}else{
				break;
			}
		}
		return (currentWidth,totalLines);
	}



	public override void OnTextChanged(AdvMessageWindow window)
	{
		_messageWindow = window;

		if (isShow)
		{
			///	PreviousFukidashi.SetUsed(rootRectTrans,TextPro.TextMeshPro.text);
		}

		if (isTategaki)
		{
			window.SetText(TategakiModifi(window.Text.OriginalText));
		}

		base.OnTextChanged(window);
		currentTextLength = -1;

		AdjustSize(window.Text.NoneMetaString,rootRectTrans);
		AdjustSize("　",fukidashiBack);
		if (engine.Page.CharacterLabel == null)
		{
			Debug.Log("キャラクター指定なしの吹き出し指定");
			return;
		}
		engine.Page.Status = AdvPage.PageStatus.WaitInputInPage;
		SetDisplayAnimation();
	}

	string TategakiModifi(string text)
	{
		var tategaki = new StringBuilder(text);
		tategaki.Insert(0, "<rotate=90>");
		tategaki.Append("</rotate>");

		void Replace(string inText, string replaceText)
		{
			if (text.IndexOf(inText) != -1)
			{
				tategaki.Replace(inText, replaceText);
			}
		}

		Replace("「", "<rotate=0>「</rotate>");
		Replace("」", "<rotate=0>」</rotate>");
		Replace("。", "<voffset=0.55em>。</voffset>");
		Replace("、", "<voffset=0.55em>、</voffset>");
		Replace("ゃ", "<voffset=0.2em>ゃ</voffset>");
		Replace("ゅ", "<voffset=0.2em>ゅ</voffset>");
		Replace("ょ", "<voffset=0.2em>ょ</voffset>");
		Replace("ぁ", "<voffset=0.2em>ぁ</voffset>");
		Replace("ぃ", "<voffset=0.2em>ぃ</voffset>");
		Replace("ぅ", "<voffset=0.2em>ぅ</voffset>");
		Replace("ぇ", "<voffset=0.2em>ぇ</voffset>");
		Replace("ぉ", "<voffset=0.2em>ぉ</voffset>");
		Replace("っ", "<voffset=0.2em>っ</voffset>");
		Replace("ャ", "<voffset=0.2em>ゃ</voffset>");
		Replace("ュ", "<voffset=0.2em>ゅ</voffset>");
		Replace("ョ", "<voffset=0.2em>ょ</voffset>");
		Replace("ァ", "<voffset=0.2em>ぁ</voffset>");
		Replace("ィ", "<voffset=0.2em>ぃ</voffset>");
		Replace("ゥ", "<voffset=0.2em>ぅ</voffset>");
		Replace("ェ", "<voffset=0.2em>ぇ</voffset>");
		Replace("ォ", "<voffset=0.2em>ぉ</voffset>");
		Replace("ヵ", "<voffset=0.2em>っ</voffset>");

		return tategaki.ToString();
	}


	private void SetDisplayAnimation()
	{
		var windowPosLabel = GetAutoWindowPosLabel(currentFukidashiState.windowPosLabel);
		Vector2 mouthPos = GetMouthPos(windowPosLabel);

		SetPosition(mouthPos);
		var toMovePos = GetAdjustPosition(GetPosByRowLabel(windowPosLabel) + mouthPos);

		/*	
				if(PreviousFukidashi.RootPosition == toMovePos)
				{
					toMovePos += overlapOffset;
				}
		*/
		DoMove(toMovePos,fukidashiBack);
		DoMove(toMovePos,rootRectTrans);
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
		if (windowPosLabel == null)
		{
			return "LeftUp";
		}
		return currentFukidashiState.windowPosLabel;
	}

	int currentTextLength;
	protected override void UpdateCurrent()
	{
		if (isAnimation)
		{
			return;
		}

		base.UpdateCurrent();

		if(_messageWindow != null)
		{
			if(currentTextLength < Engine.Page.CurrentTextLength)
			{
				currentTextLength = Engine.Page.CurrentTextLength;
				if(Engine.Page.CurrentTextLength < _messageWindow.Text.NoneMetaString.Length)
				{
					Debug.Log("NowText:"+_messageWindow.Text.NoneMetaString[Engine.Page.CurrentTextLength] + " :"+Engine.Page.CurrentTextLength);
					Debug.Log("now row:"+ GetCurrentLineCount(_messageWindow.Text.NoneMetaString,currentTextLength));
					AdjustSize(GetCurrentLineCount(_messageWindow.Text.NoneMetaString,currentTextLength).Item1,GetCurrentLineCount(_messageWindow.Text.NoneMetaString,currentTextLength).Item2,fukidashiBack);
				}
			
			}
		}
	}

	public void DoMove(UnityEngine.Vector2 toPos,RectTransform targetRect)
	{
		isAnimation = true;

		targetRect.transform.localScale = Vector3.zero;
		targetRect.DOScale(Vector3.one, 0.2f);
		targetRect.DOLocalMove(toPos, 0.2f).OnComplete(() =>
		{
			isShow = true;
			isAnimation = false;
			engine.Page.Status = AdvPage.PageStatus.SendChar;
		});
	}
}
