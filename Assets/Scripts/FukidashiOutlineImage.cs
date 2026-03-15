using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;
public class FukidashiOutlineImage : Image
{   
    [SerializeField] float thickness;
    [SerializeField] RectTransform outLineRect;
    [SerializeField] RectTransform thisRect;

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        if (outLineRect != null)
        {
            outLineRect.SetWidth(thisRect.GetWith() + thickness);
            outLineRect.SetHeight(thisRect.GetHeight() + thickness);
        }
    }
}
