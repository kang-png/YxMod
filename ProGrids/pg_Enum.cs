using UnityEngine;

namespace ProGrids;

public static class pg_Enum
{
	public static Vector3 InverseAxisMask(Vector3 v, Axis axis)
	{
		switch (axis)
		{
		case Axis.X:
		case Axis.NegX:
			return Vector3.Scale(v, new Vector3(0f, 1f, 1f));
		case Axis.Y:
		case Axis.NegY:
			return Vector3.Scale(v, new Vector3(1f, 0f, 1f));
		case Axis.Z:
		case Axis.NegZ:
			return Vector3.Scale(v, new Vector3(1f, 1f, 0f));
		default:
			return v;
		}
	}

	public static Vector3 AxisMask(Vector3 v, Axis axis)
	{
		switch (axis)
		{
		case Axis.X:
		case Axis.NegX:
			return Vector3.Scale(v, new Vector3(1f, 0f, 0f));
		case Axis.Y:
		case Axis.NegY:
			return Vector3.Scale(v, new Vector3(0f, 1f, 0f));
		case Axis.Z:
		case Axis.NegZ:
			return Vector3.Scale(v, new Vector3(0f, 0f, 1f));
		default:
			return v;
		}
	}

	public static float SnapUnitValue(SnapUnit su)
	{
		return su switch
		{
			SnapUnit.Meter => 1f, 
			SnapUnit.Centimeter => 0.01f, 
			SnapUnit.Millimeter => 0.001f, 
			SnapUnit.Inch => 0.025399987f, 
			SnapUnit.Foot => 0.3048f, 
			SnapUnit.Yard => 1.09361f, 
			SnapUnit.Parsec => 5f, 
			_ => 1f, 
		};
	}
}
