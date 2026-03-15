using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utage;
using UtageExtensions;

public class FukidashiOutline : MonoBehaviour
{
    [SerializeField] float thickness;
    [SerializeField] RectTransform targetRect;
    [SerializeField] RectTransform thisRect;

    void Start()
    {
      var trigger = gameObject.AddComponent<ObservableRectTransformTrigger>();
      //RectTransformが変化した時を購読(処理を登録)、AddToは不要
      trigger.OnRectTransformDimensionsChangeAsObservable().Subscribe(x => OnRectChange(x));
    }

    private void OnRectChange(R3.Unit unit)
    {

            thisRect.SetWidth(targetRect.GetWith() + thickness);
            thisRect.SetHeight(targetRect.GetHeight() + thickness);
        
    }
/*
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        if (target != null)
        {
            target.SetWidth(thisRect.GetWith() + thickness);
            target.SetHeight(thisRect.GetHeight() + thickness);
        }
    }
    */

}
