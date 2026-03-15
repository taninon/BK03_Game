using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using R3.Triggers;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MaskPositionProvider : MonoBehaviour
{
    public Material targetMaterial; // 切り抜かれる側のマテリアル
    private RectTransform rectTransform;

    void Update()
    {
        if (targetMaterial == null) return;
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        // マスクの4角のワールド座標を取得
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        // シェーダーにマスクの範囲（最小座標と最大座標）を渡す
        targetMaterial.SetVector("_MaskRect", new Vector4(corners[0].x, corners[0].y, corners[2].x, corners[2].y));
    }
}