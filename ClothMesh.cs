using UnityEngine;

public class ClothMesh : MonoBehaviour
{
	public AudioSource sailAudio;

	public float springForce = 10f;

	public float damperForce = 1f;

	public float bendSpringForce = 10f;

	public float bendDamperForce = 1f;

	public Transform c1;

	public Transform c2;

	public Transform c3;

	public int subdiv = 3;

	public Rigidbody c1fix;

	public Rigidbody c2fix;

	public Vector3 wind;

	private Vector3[] vertices;

	private Vector3[] bodyPositions;

	private Vector3[] bodyPositionsLocal;

	private float[] weights;

	private Rigidbody[] bodies;

	private SpringJoint[] springs;

	private Mesh mesh;

	public static float[] lift = new float[11]
	{
		0f, 0.5f, 1.3f, 1.6f, 1.5f, 1.4f, 1f, 0.7f, 0.4f, 0.3f,
		0f
	};

	public static float[] drag = new float[10] { 0.01f, 0.1f, 0.15f, 0.2f, 0.3f, 0.4f, 0.5f, 0.7f, 1f, 1.3f };

	public float pressureLinear = 20f;

	public float pressureSquare = 10f;

	public float falloffPower = 1f;

	public float refSpeed = 1f;

	public float bendWind = 0.5f;

	public float transferToBoat = 0.75f;

	public Vector3 forwardDirection = Vector3.forward;

	public float bendClose = 0.5f;

	public float bendBeam = 0.4f;

	public float bendRun;

	private void Start()
	{
		bodies = new Rigidbody[subdiv * subdiv];
		bodyPositions = new Vector3[subdiv * subdiv];
		bodyPositionsLocal = new Vector3[subdiv * subdiv];
		weights = new float[subdiv * subdiv];
		for (int i = 0; i < subdiv; i++)
		{
			Vector3 vector = Vector3.Lerp(c1.position, c2.position, 1f * (float)i / (float)(subdiv - 1));
			Vector3 b = Vector3.Lerp(c1.position, c3.position, 1f * (float)i / (float)(subdiv - 1));
			float num = 1f * (float)i / (float)(subdiv - 1);
			for (int j = 0; j <= i; j++)
			{
				float num2 = ((i != 0) ? (1f * (float)j / (float)i) : 0f);
				float num3 = 0.25f + num * (1f - num2);
				weights[i * subdiv + j] = num3;
				Vector3 pos = ((i != 0) ? Vector3.Lerp(vector, b, 1f * (float)j / (float)i) : vector);
				CreateMass(i * subdiv + j, pos);
				if (i > 0 && j < i)
				{
					CreateSpring(i, j, i - 1, j, springForce, damperForce);
				}
				if (j > 0)
				{
					CreateSpring(i, j, i, j - 1, springForce, damperForce);
				}
				if (i > 0 && j > 0)
				{
					CreateSpring(i, j, i - 1, j - 1, springForce, damperForce);
				}
				if (i > 1 && j < i - 1)
				{
					CreateSpring(i, j, i - 2, j, bendSpringForce, bendDamperForce);
				}
				if (j > 1)
				{
					CreateSpring(i, j, i, j - 2, bendSpringForce, bendDamperForce);
				}
				if (i > 1 && j > 1)
				{
					CreateSpring(i, j, i - 2, j - 2, bendSpringForce, bendDamperForce);
				}
				if (j == 0)
				{
					bodies[i * subdiv + j].gameObject.AddComponent<FixedJoint>().connectedBody = c1fix;
				}
				else if (c2fix != null && i == subdiv - 1)
				{
					bodies[i * subdiv + j].gameObject.AddComponent<FixedJoint>().connectedBody = c2fix;
				}
			}
		}
		int[] array = new int[subdiv * (subdiv - 1) / 2 * 6 * 2 - (subdiv - 1) * 6];
		vertices = new Vector3[array.Length];
		int num4 = 0;
		for (int k = 0; k < subdiv - 1; k++)
		{
			for (int l = 0; l <= k; l++)
			{
				array[num4] = num4++;
				array[num4] = num4++;
				array[num4] = num4++;
				array[num4] = num4++;
				array[num4] = num4++;
				array[num4] = num4++;
				if (l < k)
				{
					array[num4] = num4++;
					array[num4] = num4++;
					array[num4] = num4++;
					array[num4] = num4++;
					array[num4] = num4++;
					array[num4] = num4++;
				}
			}
		}
		UpdateVertices();
		mesh = new Mesh();
		mesh.name = "Sail";
		mesh.vertices = vertices;
		mesh.triangles = array;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		GetComponent<MeshFilter>().sharedMesh = mesh;
		sailAudio.transform.parent = bodies[subdiv / 2 * subdiv + subdiv / 3].transform;
		sailAudio.transform.localPosition = Vector3.zero;
	}

	private void UpdateVertices()
	{
		for (int i = 0; i < subdiv; i++)
		{
			for (int j = 0; j <= i; j++)
			{
				Vector3 position = bodies[i * subdiv + j].position;
				bodyPositions[i * subdiv + j] = position;
				ref Vector3 reference = ref bodyPositionsLocal[i * subdiv + j];
				reference = base.transform.InverseTransformPoint(position);
			}
		}
		int num = 0;
		for (int k = 0; k < subdiv - 1; k++)
		{
			for (int l = 0; l <= k; l++)
			{
				ref Vector3 reference2 = ref vertices[num];
				ref Vector3 reference3 = ref vertices[num + 3];
				reference2 = (reference3 = bodyPositionsLocal[k * subdiv + l]);
				ref Vector3 reference4 = ref vertices[num + 1];
				ref Vector3 reference5 = ref vertices[num + 5];
				reference4 = (reference5 = bodyPositionsLocal[(k + 1) * subdiv + l]);
				ref Vector3 reference6 = ref vertices[num + 2];
				ref Vector3 reference7 = ref vertices[num + 4];
				reference6 = (reference7 = bodyPositionsLocal[(k + 1) * subdiv + l + 1]);
				num += 6;
				if (l < k)
				{
					ref Vector3 reference8 = ref vertices[num];
					ref Vector3 reference9 = ref vertices[num + 3];
					reference8 = (reference9 = bodyPositionsLocal[k * subdiv + l]);
					ref Vector3 reference10 = ref vertices[num + 1];
					ref Vector3 reference11 = ref vertices[num + 5];
					reference10 = (reference11 = bodyPositionsLocal[k * subdiv + l + 1]);
					ref Vector3 reference12 = ref vertices[num + 2];
					ref Vector3 reference13 = ref vertices[num + 4];
					reference12 = (reference13 = bodyPositionsLocal[(k + 1) * subdiv + l + 1]);
					num += 6;
				}
			}
		}
	}

	private void FixedUpdate()
	{
		UpdateVertices();
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		GetComponent<MeshFilter>().sharedMesh = mesh;
		ApplyAeroDynamic(apply: true);
	}

	public static float Sample(float[] values, float deg)
	{
		deg /= 10f;
		int num = Mathf.FloorToInt(deg);
		if (num >= values.Length - 1)
		{
			return values[values.Length - 1];
		}
		return Mathf.Lerp(values[num], values[num + 1], deg - (float)num);
	}

	private void ApplyAeroDynamic(bool apply)
	{
		if (!apply)
		{
			Gizmos.color = Color.red;
		}
		float num = Vector3.Dot(wind.normalized, base.transform.TransformDirection(forwardDirection));
		if (num > 0f)
		{
			bendWind = Mathf.Lerp(bendBeam, bendRun, num);
		}
		else
		{
			bendWind = Mathf.Lerp(bendBeam, bendClose, 0f - num);
		}
		for (int i = 1; i < subdiv; i++)
		{
			for (int j = 1; j <= i; j++)
			{
				Vector3 vector = bodyPositions[(i - 1) * subdiv + j - 1];
				Vector3 vector2 = bodyPositions[i * subdiv + j - 1];
				Vector3 vector3 = bodyPositions[i * subdiv + j];
				Vector3 vector4 = Vector3.Cross(vector - vector2, vector - vector3);
				float magnitude = vector4.magnitude;
				vector4 = vector4.normalized;
				Vector3 vector5 = wind - bodies[i * subdiv + j - 1].velocity;
				float num2 = Vector3.Dot(vector5.normalized, vector4);
				if (num2 < 0f)
				{
					num2 *= -1f;
					vector4 *= -1f;
				}
				vector4 = (vector4 - vector5.normalized * bendWind).normalized;
				float num3 = vector5.magnitude / refSpeed;
				Vector3 vector6 = (pressureLinear * num3 + pressureSquare * num3 * num3) * magnitude * Mathf.Pow(num2, falloffPower) * vector4;
				if (apply)
				{
					bodies[i * subdiv + j].AddForce(vector6 * (1f - transferToBoat));
					c1fix.AddForceAtPosition(transferToBoat * vector6 * weights[i * subdiv + j], vector3);
				}
				else
				{
					Gizmos.DrawRay(vector3, vector6);
				}
			}
		}
	}

	public void OnDrawGizmosSelected()
	{
		if (bodyPositions != null)
		{
			ApplyAeroDynamic(apply: false);
		}
	}

	private void CreateMass(int idx, Vector3 pos)
	{
		GameObject gameObject = new GameObject(idx.ToString());
		gameObject.AddComponent<SphereCollider>().radius = 0.1f;
		Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
		gameObject.tag = "Target";
		gameObject.transform.position = pos;
		gameObject.transform.SetParent(base.transform, worldPositionStays: true);
		bodies[idx] = rigidbody;
	}

	private void CreateSpring(int r1, int c1, int r2, int c2, float springForce, float damperForce)
	{
		CreateSpring(r1 * subdiv + c1, r2 * subdiv + c2, springForce, damperForce);
	}

	private void CreateSpring(int idx1, int idx2, float springForce, float damperForce)
	{
		Rigidbody rigidbody = bodies[idx1];
		Rigidbody rigidbody2 = bodies[idx2];
		SpringJoint springJoint = rigidbody.gameObject.AddComponent<SpringJoint>();
		springJoint.autoConfigureConnectedAnchor = false;
		springJoint.connectedBody = rigidbody2;
		Vector3 anchor = (springJoint.connectedAnchor = Vector3.zero);
		springJoint.anchor = anchor;
		float minDistance = (springJoint.maxDistance = (rigidbody.position - rigidbody2.position).magnitude);
		springJoint.minDistance = minDistance;
		springJoint.spring = springForce;
		springJoint.damper = damperForce;
	}
}
