using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class AdvUguiTategakiMessageWindow: AdvUguiMessageWindowTMP
{
    [SerializeField] int widthTextCount;
    private bool isAnimation;
	public override void OnTextChanged(AdvMessageWindow window)
    {
		window.SetText(TategakiUtl.Modifi(window.Text.OriginalText));
        base.OnTextChanged(window);

        Debug.Log("Row" + window.Text.NoneMetaString.Length / widthTextCount);
    }
}
