using TMPro;
using UnityEngine;
using Utage;
using UtageExtensions;

public class AdvUguiTategakiMessageWindow : AdvUguiMessageWindowTMP
{
	[SerializeField] int widthTextCount;

	[SerializeField] float minHeight = 120;

	[SerializeField] RectTransform rectTransform;
	[SerializeField] TextMeshProUGUI messageText;

	float characterWidth { get { return (messageText.characterSpacing / 100 * messageText.fontSize) + messageText.fontSize; } }

	public override void OnTextChanged(AdvMessageWindow window)
	{
		window.SetText(TategakiUtl.Modifi(window.Text.OriginalText));
		base.OnTextChanged(window);

		rectTransform.SetWidth(Mathf.Max(minHeight, minHeight + (GetRowCount(window.Text.NoneMetaString) - 1) * characterWidth * 1.5f));
	}

	private int GetRowCount(string text)
	{
		var rows = text.Split('\n');

		int rowCount = rows.Length;
		for (int i = 0; i < rows.Length; i++)
		{
			rowCount += rows[i].Length / widthTextCount;
		}

		return rowCount;
	}

}
