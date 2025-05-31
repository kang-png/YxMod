using UnityEngine;

public class VolumeFinder : MonoBehaviour
{
	private enum VolumeType
	{
		Custom,
		Box,
		Sphere
	}

	[SerializeField]
	public float volume;

	[SerializeField]
	private VolumeType shapeType;

	private void Start()
	{
		if (shapeType != 0)
		{
			CalculateVolume();
		}
	}

	public void CalculateVolume()
	{
		if (shapeType == VolumeType.Sphere)
		{
			volume = 4.1887903f * Mathf.Pow(base.transform.localScale.x * 0.5f, 3f);
		}
		if (shapeType == VolumeType.Box)
		{
			volume = base.transform.localScale.x * base.transform.localScale.y * base.transform.localScale.z;
		}
	}
}
