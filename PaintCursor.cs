using UnityEngine;
using UnityEngine.UI;

public class PaintCursor : MonoBehaviour
{
	public Color color = Color.white;

	public float cursorKernel = 10f;

	public float cursorFalloff = 10f;

	private RawImage cursor;

	private Material cursorMaterial;

	private RectTransform cursorParentRect;

	public void Initialize()
	{
		cursor = GetComponent<RawImage>();
		cursorMaterial = new Material(cursor.material);
		cursor.material = cursorMaterial;
		cursorParentRect = base.transform.parent.GetComponent<RectTransform>();
	}

	public void Process(bool show)
	{
		if (base.isActiveAndEnabled != show)
		{
			base.gameObject.SetActive(show);
		}
		if (show)
		{
			cursorMaterial.color = color;
			cursorMaterial.SetVector("_PointPos", new Vector4(Input.mousePosition.x, Input.mousePosition.y, 0f, 0f));
			cursorMaterial.SetVector("_ScreenSize", new Vector4(Screen.width, Screen.height, 0f, 0f));
			cursorMaterial.SetFloat("_PointSize", cursorKernel);
			cursorMaterial.SetFloat("_FalloffSize", cursorFalloff);
			float num = (float)(2 * Screen.height) * (cursorFalloff + cursorKernel) + 2f;
			Vector2 anchoredPosition = UICanvas.ScreenPointToLocal(cursorParentRect, Input.mousePosition);
			Vector2 vector = UICanvas.ScreenPointToLocal(cursorParentRect, Input.mousePosition + new Vector3(1f, 1f, 0f) * num);
			cursor.rectTransform.anchoredPosition = anchoredPosition;
			cursor.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, vector.x - anchoredPosition.x);
			cursor.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vector.y - anchoredPosition.y);
		}
	}
}
