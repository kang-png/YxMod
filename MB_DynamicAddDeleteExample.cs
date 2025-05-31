using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB_DynamicAddDeleteExample : MonoBehaviour
{
	public GameObject prefab;

	private List<GameObject> objsInCombined = new List<GameObject>();

	private MB3_MultiMeshBaker mbd;

	private GameObject[] objs;

	private float GaussianValue()
	{
		float num;
		float num3;
		do
		{
			num = 2f * Random.Range(0f, 1f) - 1f;
			float num2 = 2f * Random.Range(0f, 1f) - 1f;
			num3 = num * num + num2 * num2;
		}
		while (num3 >= 1f);
		num3 = Mathf.Sqrt(-2f * Mathf.Log(num3) / num3);
		return num * num3;
	}

	private void Start()
	{
		mbd = GetComponentInChildren<MB3_MultiMeshBaker>();
		int num = 10;
		GameObject[] array = new GameObject[num * num];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				GameObject gameObject = Object.Instantiate(prefab);
				array[i * num + j] = gameObject.GetComponentInChildren<MeshRenderer>().gameObject;
				float num2 = Random.Range(-4f, 4f);
				float num3 = Random.Range(-4f, 4f);
				gameObject.transform.position = new Vector3(3f * (float)i + num2, 0f, 3f * (float)j + num3);
				float y = Random.Range(0, 360);
				gameObject.transform.rotation = Quaternion.Euler(0f, y, 0f);
				Vector3 localScale = Vector3.one + Vector3.one * GaussianValue() * 0.15f;
				gameObject.transform.localScale = localScale;
				if ((i * num + j) % 3 == 0)
				{
					objsInCombined.Add(array[i * num + j]);
				}
			}
		}
		mbd.AddDeleteGameObjects(array, null, disableRendererInSource: true);
		mbd.Apply();
		objs = objsInCombined.ToArray();
		StartCoroutine(largeNumber());
	}

	private IEnumerator largeNumber()
	{
		while (true)
		{
			yield return new WaitForSeconds(1.5f);
			mbd.AddDeleteGameObjects(null, objs, disableRendererInSource: true);
			mbd.Apply();
			yield return new WaitForSeconds(1.5f);
			mbd.AddDeleteGameObjects(objs, null, disableRendererInSource: true);
			mbd.Apply();
		}
	}

	private void OnGUI()
	{
		GUILayout.Label("Dynamically instantiates game objects. \nRepeatedly adds and removes some of them\n from the combined mesh.");
	}
}
