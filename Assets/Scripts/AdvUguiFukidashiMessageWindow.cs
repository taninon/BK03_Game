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

	[SerializeField] RectTransform moveRectTrans;
	[SerializeField] RectTransform textRectTrans;
	[SerializeField] RectTransform fukidashiBack;
	[SerializeField] TextMeshProUGUI messageText;
	[SerializeField] RectTransform messageTextRectTrans;
	bool isAnimation;
	bool isShow = false;

	[SerializeField] Vector2 minSize;
	[SerializeField] Vector2 textMinSize;
	[SerializeField] Vector2 maxSize;
	[SerializeField] Vector2 fukidashiOffSet;

	[SerializeField] Vector2 overlapOffset;
	float characterWidth { get { return (messageText.characterSpacing / 100 * messageText.fontSize) + messageText.fontSize; } }

	[SerializeField] bool isTategaki;

	AdvMessageWindow _messageWindow;

	[SerializeField] FukidashiBackImage backImage;

	private GameObject previous;
	private CanvasGroup previousCanvas;
	public override void OnInit(AdvMessageWindowManager windowManager)
	{
		base.OnInit(windowManager);
		SetConstVariable().Forget();
	}

	private async UniTaskVoid SetConstVariable()
	{
		await UniTask.WaitUntil(() => engine.Param.IsInit);
		//	fukidashiUpperYLimit = engine.Param.GetParameterFloat("fukidashiUpperYLimit");
		//	fukidashiLowerYLimit = engine.Param.GetParameterFloat("fukidashiLowerYLimit");
	}

	internal struct CharacterFukidashiState
	{
		internal string characterLabel;
		internal string windowPosLabel;
		internal Vector2 rootPos;
		internal string fukidashiType;
		internal bool textWait;
		internal bool keep;
		internal Vector2 offSet;
	}

	private CharacterFukidashiState currentFukidashiState = new CharacterFukidashiState();
	private CharacterFukidashiState beforeFukidashiState = new CharacterFukidashiState();

	public void SetPosition(Vector2 pos)
	{
		currentFukidashiState.rootPos = pos;
		moveRectTrans.anchoredPosition = currentFukidashiState.rootPos;
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


	public void SetCustomParam(StringGridRow row)
	{
		beforeFukidashiState = currentFukidashiState;

		row.TryParseCell<string>("Arg1", out currentFukidashiState.characterLabel);
		row.TryParseCell<string>("WindowPos", out currentFukidashiState.windowPosLabel);
		row.TryParseCell<string>("FukidashiType", out currentFukidashiState.fukidashiType);
		currentFukidashiState.textWait = true;

		string textWait;

		if (row.TryParseCell<string>("TextWait", out textWait))
		{
			currentFukidashiState.textWait = !(textWait.ToLower() == "off");
		}
	}

	private void AdjustSize(int width, int rowCount, RectTransform targetRect, Vector2 min, Vector2 max)
	{
		var textOffsetX = fukidashiOffSet.x * 2;
		var setWidth = Mathf.Clamp((width + 1) * characterWidth + textOffsetX, min.x, max.x);

		if (setWidth > targetRect.GetWith())
		{
			targetRect.SetWidth(setWidth);
		}

		var textOffsetY = fukidashiOffSet.y * 2;
		var setHeight = Mathf.Clamp(rowCount * characterWidth * 1.5f + textOffsetY, min.y, max.y);
		targetRect.SetHeight(setHeight);
	}


	private void AdjustSize(string text, RectTransform targetRect, Vector2 min, Vector2 max)
	{
		int rowCount = GetLineCount(text);

		var textOffsetX = Mathf.Abs(messageTextRectTrans.offsetMin.x) + Mathf.Abs(messageTextRectTrans.offsetMax.x);
		targetRect.SetWidth(Mathf.Clamp((GetlongestLengthCount(text) + 1) * characterWidth + textOffsetX, min.x, max.x));

		var textOffsetY = Mathf.Abs(messageTextRectTrans.offsetMin.y) + Mathf.Abs(messageTextRectTrans.offsetMax.y);
		targetRect.SetHeight(Mathf.Clamp(rowCount * characterWidth * 1.5f + textOffsetY, min.y, max.y));

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
		if (string.IsNullOrEmpty(text)) return (0, 1);

		var lines = text.Split('\n');
		int totalLines = 1;
		int currentWidth = nowWidth;

		foreach (var line in lines)
		{
			if (line.Length + 1 < currentWidth)
			{
				totalLines++;
				currentWidth -= line.Length;
			}
		}
		return (currentWidth, totalLines);
	}

	public override void OnTextChanged(AdvMessageWindow window)
	{
		_messageWindow = window;

		if (previous != null)
		{
			previousCanvas = previous.GetComponent<CanvasGroup>();
			previousCanvas.DOFade(0, 0.5f).OnComplete(() => Destroy(previousCanvas.gameObject));
		}

		if (currentFukidashiState.keep)
		{
			previous = Instantiate(moveRectTrans.gameObject, this.transform);
			previous.transform.SetAsFirstSibling();
		}

		if (isTategaki)
		{
			window.SetText(TategakiUtl.Modifi(window.Text.OriginalText));
		}

		base.OnTextChanged(window);
		currentTextLength = -1;

		//	AdjustTextSize(window.Text.NoneMetaString);
		AdjustSize("　", fukidashiBack, minSize, maxSize);
		if (engine.Page.CharacterLabel == null)
		{
			Debug.Log("キャラクター指定なしの吹き出し指定");
			return;
		}


		if (currentFukidashiState.textWait)
		{
			engine.Page.Status = AdvPage.PageStatus.WaitInputInPage;
		}
		else
		{
			SetFullSize();
		}

		SetDisplayAnimation();
	}


	private void SetFukidashiType()
	{
		string fukidashiType = currentFukidashiState.fukidashiType;
		if (currentFukidashiState.fukidashiType.IsNullOrEmpty())
		{
			fukidashiType = "normal";
		}

		SetFukidashiBack(fukidashiType);
	}

	public void SetFukidashiBack(string type)
	{
		backImage.SetFukidashiType(type);
	}

	public void SetKeepFukidashi(bool value)
	{
		currentFukidashiState.keep = value;
	}


	private void SetDisplayAnimation()
	{
		var windowPosLabel = GetAutoWindowPosLabel(currentFukidashiState.windowPosLabel);
		backImage.SetPosition(windowPosLabel);
		Vector2 mouthPos = GetMouthPos(windowPosLabel);


		SetPosition(mouthPos);

		var toMovePos = GetPosByRowLabel(windowPosLabel) + currentFukidashiState.offSet;

		SetFukidashiType();
		DoMove(toMovePos, moveRectTrans);
	}

	private Vector2 GetMouthPos(string windowPosLabel)
	{
		var targetCharacter = engine.GraphicManager.CharacterManager.AllGraphics().FirstOrDefault(g => g.name == engine.Page.CharacterLabel);
		if (targetCharacter == null)
		{
			return GetPosByRowLabel(windowPosLabel);
		}
		return GetCharacterPos(targetCharacter.RenderObject) + GetPosByRowLabel("MouthPos");
	}

	private string GetAutoWindowPosLabel(string windowPosLabel)
	{
		if (windowPosLabel == null)
		{
			return "LeftUp";
		}
		return currentFukidashiState.windowPosLabel;
	}

	public void SetFukidashiRootOffset(Vector2 offset)
	{
		currentFukidashiState.offSet = offset;
	}

	int currentTextLength;
	protected override void UpdateCurrent()
	{
		if (isAnimation)
		{
			return;
		}

		base.UpdateCurrent();

		if (_messageWindow != null && currentFukidashiState.textWait == true)
		{
			if (currentTextLength < Engine.Page.CurrentTextLength)
			{
				currentTextLength = Engine.Page.CurrentTextLength;
				if (Engine.Page.CurrentTextLength < _messageWindow.Text.NoneMetaString.Length)
				{
					AdjustSize(GetCurrentLineCount(_messageWindow.Text.NoneMetaString, currentTextLength).Item1, GetCurrentLineCount(_messageWindow.Text.NoneMetaString, currentTextLength).Item2, fukidashiBack, minSize, maxSize);
				}
			}
		}
	}

	public void DoMove(UnityEngine.Vector2 toPos, RectTransform targetRect)
	{
		isAnimation = true;

		targetRect.transform.localScale = Vector3.zero;
		targetRect.DOScale(Vector3.one, 0.2f);
		targetRect.DOLocalMove(toPos, 0.2f).OnComplete(() =>
		{
			isAnimation = false;
			engine.Page.Status = AdvPage.PageStatus.SendChar;
		});
	}

	//AdvPage OnEndTextに登録する
	public void OnEndText()
	{
		if (_messageWindow != null)
		{
			SetFullSize();
		}
	}

	private void SetFullSize()
	{
		AdjustSize(
			GetlongestLengthCount(_messageWindow.Text.NoneMetaString),
			_messageWindow.Text.NoneMetaString.Split('\n').Length,
			fukidashiBack,
			minSize,
			maxSize
			);
	}
}
