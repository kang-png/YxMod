using UnityEngine;

public class WaterGrid : WaterBody
{
	public Vector3 flow;

	public int gridWidth = 10;

	public int gridHeight = 10;

	public float x0;

	public float z0;

	public float dx;

	public float dz;

	public float scanPad = 0.5f;

	public float[] heightmap;

	private const float min = -100f;

	public Collider[] volumes;

	private Vector3 globalFlow;

	private void Start()
	{
		if (volumes.Length == 0)
		{
			volumes = new Collider[1] { GetComponent<MeshCollider>() };
		}
		globalFlow = base.transform.TransformVector(flow);
	}

	public void Rebuild()
	{
		heightmap = new float[gridWidth * gridHeight];
		Bounds bounds = volumes[0].bounds;
		for (int i = 1; i < volumes.Length; i++)
		{
			bounds.Encapsulate(volumes[i].bounds);
		}
		x0 = bounds.min.x + 0.1f;
		z0 = bounds.min.z + 0.1f;
		dx = bounds.size.x / (float)(gridWidth - 1);
		dz = bounds.size.z / (float)(gridHeight - 1);
		Vector3 origin = new Vector3(0f, bounds.max.y + 1f, 0f);
		Vector3 direction = new Vector3(0f, -1f, 0f);
		float y = bounds.size.y;
		for (int j = 0; j < gridWidth; j++)
		{
			for (int k = 0; k < gridHeight; k++)
			{
				origin.x = x0 + dx * (float)j;
				origin.z = z0 + dz * (float)k;
				if (j == 0)
				{
					origin.x += dx * scanPad;
				}
				if (j == gridWidth - 1)
				{
					origin.x -= dx * scanPad;
				}
				if (k == 0)
				{
					origin.z += dz * scanPad;
				}
				if (k == gridHeight - 1)
				{
					origin.z -= dz * scanPad;
				}
				Ray ray = new Ray(origin, direction);
				float num = float.MinValue;
				for (int l = 0; l < volumes.Length; l++)
				{
					if (volumes[l].Raycast(ray, out var hitInfo, y))
					{
						num = Mathf.Max(num, hitInfo.point.y);
					}
				}
				if (num > bounds.max.y + 1f || num < bounds.min.y - 1f)
				{
					num = -100f;
				}
				heightmap[k * gridWidth + j] = num;
			}
		}
	}

	public void OnDrawGizmosSelected()
	{
		if (heightmap == null)
		{
			return;
		}
		Gizmos.color = Color.yellow;
		for (int i = 0; i < gridWidth; i++)
		{
			for (int j = 0; j < gridHeight - 1; j++)
			{
				float num = heightmap[j * gridWidth + i];
				float num2 = heightmap[(j + 1) * gridWidth + i];
				if (num != -100f && num2 != -100f)
				{
					Gizmos.DrawLine(new Vector3(x0 + dx * (float)i, num, z0 + dz * (float)j), new Vector3(x0 + dx * (float)i, num2, z0 + dz * (float)(j + 1)));
				}
			}
		}
		for (int k = 0; k < gridWidth - 1; k++)
		{
			for (int l = 0; l < gridHeight; l++)
			{
				float num3 = heightmap[l * gridWidth + k];
				float num4 = heightmap[l * gridWidth + k + 1];
				if (num3 != -100f && num4 != -100f)
				{
					Gizmos.DrawLine(new Vector3(x0 + dx * (float)k, num3, z0 + dz * (float)l), new Vector3(x0 + dx * (float)(k + 1), num4, z0 + dz * (float)l));
				}
			}
		}
	}

	public override float SampleDepth(Vector3 pos, out Vector3 velocity)
	{
		velocity = globalFlow;
		float num = (pos.x - x0) / dx;
		float num2 = (pos.z - z0) / dz;
		int num3 = Mathf.FloorToInt(num);
		int num4 = Mathf.FloorToInt(num2);
		int num5 = num3 + 1;
		int num6 = num4 + 1;
		if (num3 < 0)
		{
			num3 = (num5 = 0);
		}
		if (num5 >= gridWidth)
		{
			num3 = (num5 = gridWidth - 1);
		}
		if (num4 < 0)
		{
			num4 = (num6 = 0);
		}
		if (num6 >= gridHeight)
		{
			num4 = (num6 = gridHeight - 1);
		}
		num -= (float)num3;
		num2 -= (float)num4;
		float num7 = heightmap[num4 * gridWidth + num3];
		float num8 = heightmap[num6 * gridWidth + num3];
		float num9 = heightmap[num4 * gridWidth + num5];
		float num10 = heightmap[num6 * gridWidth + num5];
		if (num7 == -100f)
		{
			num7 = num8;
		}
		if (num8 == -100f)
		{
			num8 = num7;
		}
		if (num9 == -100f)
		{
			num9 = num10;
		}
		if (num10 == -100f)
		{
			num10 = num9;
		}
		float num11 = Mathf.Lerp(num7, num8, num);
		float num12 = Mathf.Lerp(num9, num10, num);
		if (num11 == -100f)
		{
			num11 = num12;
		}
		if (num12 == -100f)
		{
			num12 = num11;
		}
		float num13 = Mathf.Lerp(num11, num12, num2);
		return num13 - pos.y;
	}
}
