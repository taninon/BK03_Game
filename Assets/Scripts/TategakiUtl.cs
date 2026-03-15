using System.Text;
using System;
public static class TategakiUtl
{
    public static string Modifi(string text)
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
		Replace("「", "<rotate=0>「</rotate><rotate=90>");
		Replace("」", "<rotate=0>」</rotate><rotate=90>");
		Replace("～", "<rotate=0>～</rotate><rotate=90>");
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
		Replace("ヵ", "<voffset=0.2em>ヵ</voffset>");
		Replace("ャ", "<voffset=0.2em>ャ</voffset>");
		Replace("ュ", "<voffset=0.2em>ュ</voffset>");
		Replace("ョ", "<voffset=0.2em>ョ</voffset>");
		Replace("ァ", "<voffset=0.2em>ァ</voffset>");
		Replace("ィ", "<voffset=0.2em>ィ</voffset>");
		Replace("ゥ", "<voffset=0.2em>ゥ</voffset>");
		Replace("ェ", "<voffset=0.2em>ェ</voffset>");
		Replace("ォ", "<voffset=0.2em>ォ</voffset>");
		Replace("ッ", "<voffset=0.2em>ッ</voffset>");

		string outputText = tategaki.ToString();
		outputText = outputText.Replace("ー", "<rotate=0>ー</rotate><rotate=90>",StringComparison.OrdinalIgnoreCase);
		outputText = outputText.Replace("ー", "<rotate=0>ー</rotate><rotate=90>",StringComparison.OrdinalIgnoreCase);
		return outputText;
    }

}
