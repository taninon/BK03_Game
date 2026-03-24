using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FukidashiBackImage : MonoBehaviour
{
	[SerializeField] Image fukidashiLine;
	[SerializeField] Image fukidashi;

	[SerializeField] List<GameObject> Shippo_L_U;
	[SerializeField] List<GameObject> Shippo_L_D;
	[SerializeField] List<GameObject> Shippo_R_U;
	[SerializeField] List<GameObject> Shippo_R_D;

	[SerializeField] GameObject shock;
	[SerializeField] GameObject normal;

	[SerializeField] GameObject off;

	[SerializeField] Animator shippoAnimation;

	private string positionLabel;
	public void SetFukidashiType(string type)
	{
		shock.SetActive(type.ToLower() == "shock");
		normal.SetActive(type.ToLower() == "normal");
		off.SetActive(type.ToLower() == "off");
	}

	private void SetVisible(List<GameObject> targetShippos, bool isVisible)
	{
		if (normal.activeInHierarchy)
		{
			foreach (var target in targetShippos)
			{
				target.SetActive(isVisible);
			}
		}
	}

	private void AllInvisible()
	{
		SetVisible(Shippo_L_U, false);
		SetVisible(Shippo_L_D, false);
		SetVisible(Shippo_R_U, false);
		SetVisible(Shippo_R_D, false);
	}

	private void SetLeftDown()
	{
		AllInvisible();
		SetVisible(Shippo_L_D, true);
	}

	private void SetLeftUp()
	{
		AllInvisible();
		SetVisible(Shippo_L_U, true);

	}

	private void SetRightDown()
	{
		AllInvisible();
		SetVisible(Shippo_R_D, true);
	}

	private void SetRightUp()
	{
		AllInvisible();
		SetVisible(Shippo_R_U, true);
	}

	public void ShowShippo()
	{
		shippoAnimation.SetTrigger("Show");
		switch (positionLabel)
		{
			case "leftdown":
				SetLeftDown();
				break;
			case "leftup":
				SetLeftUp();
				break;
			case "rightdown":
				SetRightDown();
				break;
			case "rightup":
				SetRightUp();
				break;
			case "pov":
				AllInvisible();
				break;
			default:
				break;
		}
	}

	public void SetPosition(string position)
	{
		AllInvisible();
		positionLabel = position.ToLower();
	}
}
