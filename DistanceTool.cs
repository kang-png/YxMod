using UnityEngine;

[ExecuteInEditMode]
public class DistanceTool : MonoBehaviour
{
	public string distanceToolName = string.Empty;

	public Color lineColor = Color.yellow;

	public bool initialized;

	public string initialName = "Distance Tool";

	public Vector3 startPoint = Vector3.zero;

	public Vector3 endPoint = new Vector3(0f, 1f, 0f);

	public float distance;

	public float gizmoRadius = 0.1f;

	public bool scaleToPixels;

	public int pixelPerUnit = 128;

	private void OnEnable()
	{
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = lineColor;
		Gizmos.DrawWireSphere(startPoint, gizmoRadius);
		Gizmos.DrawWireSphere(endPoint, gizmoRadius);
		Gizmos.DrawLine(startPoint, endPoint);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = lineColor;
		Gizmos.DrawWireSphere(startPoint, gizmoRadius);
		Gizmos.DrawWireSphere(endPoint, gizmoRadius);
		Gizmos.DrawLine(startPoint, endPoint);
	}
}
