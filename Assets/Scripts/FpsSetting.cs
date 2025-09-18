using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utage;
using UtageExtensions;
public class FpsSetting : MonoBehaviour, IAdvSystemSaveDataCustom
{
	[SerializeField] int targetFrameRate = 60;

	public string SaveKey => "FpsSetting";

    public void OnWrite(BinaryWriter writer)
    {
		writer.Write(QualitySettings.vSyncCount);  // int
		writer.Write(targetFrameRate);  // int
    }

    public void OnRead(BinaryReader reader)
    {
        QualitySettings.vSyncCount= reader.ReadInt32();
		Application.targetFrameRate = reader.ReadInt32();
    }

	private void Awake()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = targetFrameRate;
	}
}

