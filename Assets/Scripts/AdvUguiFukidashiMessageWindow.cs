using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;
using System.Linq;
using System;
using DG.Tweening;
public class AdvUguiFukidashiMessageWindow : AdvUguiMessageWindowTMP
{
	public GameObject RootChildren { get { return rootChildren; } }
	string currentCharacter;
	string currentHorizontalOffset;
	string currentVerticalOffset;

	internal class CharacterFukidashiState
	{
		internal string characterLabel;
		internal string horizontalOffset;
		internal string verticalOffset;
		internal float OffsetXValue;
		internal float OffsetYValue;

		internal CharacterFukidashiState(string characterLabel, string horizontalOffset, string verticalOffset, float OffsetXValue, float OffsetYValue)
		{
			this.characterLabel = characterLabel;
			this.horizontalOffset = horizontalOffset;
		}
	}


	List<CharacterFukidashiState> characterFukidashiStates;

	public void SetOffset(string characterLabel, string horizontalOffset, string verticalOffset)
	{
		if (characterFukidashiStates.Any(_ => _.characterLabel == characterLabel))
		{
			var target = characterFukidashiStates.Find(_ => _.characterLabel == characterLabel);

			{
				target.horizontalOffset = horizontalOffset;
			}

			if (!string.IsNullOrEmpty(verticalOffset))
			{
				target.verticalOffset = verticalOffset;
			}
		}
		else
		{
			characterFukidashiStates.Add(new CharacterFukidashiState(characterLabel, horizontalOffset, verticalOffset, 0, 0));
		}

		currentCharacter = characterLabel;
		currentHorizontalOffset = horizontalOffset;
		currentVerticalOffset = verticalOffset;
	}

	public void DoMove(float startX, float startY)
	{
		
	}

	private float GetOffsetX()
	{
		return 0;
	}

	private float GetOffsetY()
	{
		return 0;
	}


	public void SetOffsetValue(string characterLabel, float offsetX, float offsetY)
	{

	}



}
