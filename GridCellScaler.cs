using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridCellScaler : MonoBehaviour
{
	private GridLayoutGroup gridLayoutGroup;

	private RectTransform rect;

	private float height;

	private int cellCount = 2;

	private AutoNavigation m_NavigationObject;

	[SerializeField]
	private int Rows = 4;

	[SerializeField]
	private int Columns = 2;

	private void Start()
	{
		gridLayoutGroup = GetComponent<GridLayoutGroup>();
		rect = GetComponent<RectTransform>();
		gridLayoutGroup.cellSize = new Vector2(rect.rect.height, rect.rect.height);
		cellCount = GetComponentsInChildren<RectTransform>().Length;
	}

	private void OnEnable()
	{
		UpdateCellDimensions();
	}

	private void OnRectTransformDimensionsChange()
	{
		UpdateCellDimensions();
	}

	private void UpdateCellDimensions()
	{
		if (gridLayoutGroup != null && rect != null)
		{
			float y = (rect.rect.height - gridLayoutGroup.spacing.y * (float)Rows) / (float)Rows;
			float x = (rect.rect.width - gridLayoutGroup.spacing.x * (float)Columns) / (float)Columns;
			gridLayoutGroup.cellSize = new Vector2(x, y);
		}
	}
}
