using UnityEngine;

namespace HumanAPI.LightLevel;

public class LightBeam : LightBase
{
	public float maxBeamDistance = 50f;

	protected Collider hitCollider;

	protected Collider mycollider;

	protected Material mat;

	public Light light;

	public override Color color
	{
		get
		{
			return light.color;
		}
		set
		{
			if (value.r + value.g + value.b == 0f)
			{
				DisableLight();
				return;
			}
			if (value.a == 0f)
			{
				value.a = 1f;
			}
			mat.color = value;
			light.color = value;
		}
	}

	public virtual float range
	{
		get
		{
			return base.transform.localScale.z;
		}
		protected set
		{
			Vector3 localScale = base.transform.localScale;
			localScale.z = value + 0.4f;
			base.transform.localScale = localScale;
			light.range = value + 0.5f;
		}
	}

	protected virtual void Awake()
	{
		mycollider = GetComponentInChildren<Collider>();
		MeshRenderer componentInChildren = GetComponentInChildren<MeshRenderer>();
		mat = componentInChildren.material;
		componentInChildren.sharedMaterial = mat;
	}

	private void FixedUpdate()
	{
		if (hitCollider != null && !hitCollider.bounds.Intersects(mycollider.bounds))
		{
			Recalculate();
		}
	}

	protected virtual void Recalculate()
	{
		if (Physics.Raycast(base.transform.position, Direction, out var hitInfo, maxBeamDistance, -5, QueryTriggerInteraction.Ignore))
		{
			range = Vector3.Distance(hitInfo.point, base.transform.position);
		}
		else
		{
			range = maxBeamDistance;
		}
		hitCollider = hitInfo.collider;
	}

	public new virtual void SetSize(Bounds b)
	{
		base.transform.localScale = b.size;
	}

	public override void Reset()
	{
		base.Reset();
		maxBeamDistance = 50f;
	}

	public override void EnableLight()
	{
		Recalculate();
		base.EnableLight();
	}

	protected override void ReturnToPool()
	{
		LightPool.DestroyLight(this);
	}

	public override void UpdateLight(LightConsume from)
	{
		if (color.r + color.g + color.b == 0f)
		{
			DisableLight();
			return;
		}
		Recalculate();
		base.UpdateLight(from);
	}

	public override void UpdateLight()
	{
		Recalculate();
		base.UpdateLight();
	}

	public override Vector3 ClosestPoint(Vector3 point)
	{
		return mycollider.ClosestPoint(point);
	}
}
