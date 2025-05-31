using UnityEngine;

public class MeshHooks : MonoBehaviour
{
	public Transform[] hooks;

	private Vector3[] vertexPos;

	private Vector3[] verts;

	private int[] vertexHook;

	private Matrix4x4[] matrices;

	private Mesh mesh;

	private MeshFilter meshFilter;

	private MeshCollider meshCollider;

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		mesh = meshFilter.mesh;
		vertexPos = mesh.vertices;
		meshCollider = GetComponent<MeshCollider>();
		if (meshCollider != null)
		{
			meshCollider.sharedMesh = mesh;
		}
		vertexHook = new int[vertexPos.Length];
		Vector3[] array = new Vector3[hooks.Length];
		for (int i = 0; i < hooks.Length; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = hooks[i].position;
		}
		for (int j = 0; j < vertexPos.Length; j++)
		{
			Vector3 vector = base.transform.TransformPoint(vertexPos[j]);
			float num = float.MaxValue;
			int num2 = 0;
			for (int k = 0; k < array.Length; k++)
			{
				float sqrMagnitude = (array[k] - vector).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					num2 = k;
				}
			}
			ref Vector3 reference2 = ref vertexPos[j];
			reference2 = hooks[num2].InverseTransformPoint(vector);
			vertexHook[j] = num2;
		}
		verts = new Vector3[vertexPos.Length];
		matrices = new Matrix4x4[hooks.Length];
	}

	private void LateUpdate()
	{
		for (int i = 0; i < hooks.Length; i++)
		{
			ref Matrix4x4 reference = ref matrices[i];
			reference = base.transform.worldToLocalMatrix * hooks[i].localToWorldMatrix;
		}
		for (int j = 0; j < vertexPos.Length; j++)
		{
			ref Vector3 reference2 = ref verts[j];
			reference2 = matrices[vertexHook[j]].MultiplyPoint3x4(vertexPos[j]);
		}
		mesh.vertices = verts;
		mesh.RecalculateBounds();
		meshFilter.sharedMesh = mesh;
		if (meshCollider != null)
		{
			meshCollider.sharedMesh = mesh;
		}
	}
}
