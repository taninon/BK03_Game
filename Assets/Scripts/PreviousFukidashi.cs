using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;
public class PreviousFukidashi : MonoBehaviour
{
    [SerializeField] Image bgFukidashi;
    [SerializeField] CanvasGroup canvasGroup;
	[SerializeField] RectTransform previousRootRectTrans;
    [SerializeField] TextMeshProUGUI previousText;
    [SerializeField] Color usedColor;
    [SerializeField] float fadeTime = 0.2f;

    public bool IsShow{get {return canvasGroup.alpha > 0.01;}}
    public Vector2 RootPosition{get {return previousRootRectTrans.anchoredPosition;}}


    public void SetUsed(RectTransform currentRootRectTrans,string text)
    {
        canvasGroup.alpha = 1;
        previousRootRectTrans.anchoredPosition = currentRootRectTrans.anchoredPosition;
        previousRootRectTrans.sizeDelta = currentRootRectTrans.sizeDelta;
        bgFukidashi.color = Color.white;
        previousText.text = text;
        bgFukidashi.DOColor(usedColor, fadeTime);
    }

    public void Init()
    {
        canvasGroup.alpha = 0;
    }

}
