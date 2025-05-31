using System;
using UnityEngine;

namespace Multiplayer;

public struct NetQuaternion
{
	public byte sel;

	public int x;

	public int y;

	public int z;

	public const float quatrange = 0.707107f;

	public ushort bits;

	public NetQuaternion(byte sel, int a, int b, int c, ushort bits)
	{
		this.sel = sel;
		x = a;
		y = b;
		z = c;
		this.bits = bits;
	}

	public NetQuaternion(byte sel, float a, float b, float c, float drop, ushort bits)
	{
		if (drop < 0f)
		{
			a = 0f - a;
			b = 0f - b;
			c = 0f - c;
		}
		this.sel = sel;
		x = NetFloat.Quantize(a, 0.707107f, bits);
		y = NetFloat.Quantize(b, 0.707107f, bits);
		z = NetFloat.Quantize(c, 0.707107f, bits);
		this.bits = bits;
	}

	public static NetQuaternion Quantize(Quaternion q, ushort bits)
	{
		float num = Mathf.Abs(q.x);
		float num2 = Mathf.Abs(q.y);
		float num3 = Mathf.Abs(q.z);
		float num4 = Mathf.Abs(q.w);
		return (byte)((num > num2) ? ((num > num3) ? ((!(num > num4)) ? 3 : 0) : ((!(num3 > num4)) ? 3 : 2)) : ((num2 > num3) ? ((num2 > num4) ? 1 : 3) : ((!(num3 > num4)) ? 3 : 2))) switch
		{
			0 => new NetQuaternion(0, q.y, q.z, q.w, q.x, bits), 
			1 => new NetQuaternion(1, q.x, q.z, q.w, q.y, bits), 
			2 => new NetQuaternion(2, q.x, q.y, q.w, q.z, bits), 
			3 => new NetQuaternion(3, q.x, q.y, q.z, q.w, bits), 
			_ => throw new InvalidOperationException("can't get here"), 
		};
	}

	public Quaternion Dequantize()
	{
		float num = NetFloat.Dequantize(x, 0.707107f, bits);
		float num2 = NetFloat.Dequantize(y, 0.707107f, bits);
		float num3 = NetFloat.Dequantize(z, 0.707107f, bits);
		float w = Mathf.Sqrt(1f - num * num - num2 * num2 - num3 * num3);
		return sel switch
		{
			0 => new Quaternion(w, num, num2, num3), 
			1 => new Quaternion(num, w, num2, num3), 
			2 => new Quaternion(num, num2, w, num3), 
			3 => new Quaternion(num, num2, num3, w), 
			_ => throw new InvalidOperationException("can't get here"), 
		};
	}

	public static NetQuaternionDelta Delta(NetQuaternion from, NetQuaternion to, ushort bitlarge)
	{
		int num = to.x - from.x;
		int num2 = to.y - from.y;
		int num3 = to.z - from.z;
		int num4 = 1 << bitlarge - 1;
		if (num < -num4 || num >= num4 || num2 < -num4 || num2 >= num4 || num3 < -num4 || num3 >= num4)
		{
			NetQuaternionDelta result = default(NetQuaternionDelta);
			result.isRelative = false;
			result.sel = to.sel;
			result.x = to.x;
			result.y = to.y;
			result.z = to.z;
			return result;
		}
		NetQuaternionDelta result2 = default(NetQuaternionDelta);
		result2.isRelative = true;
		result2.sel = to.sel;
		result2.x = num;
		result2.y = num2;
		result2.z = num3;
		return result2;
	}

	public static NetQuaternionDelta WorstCaseDelta(NetQuaternion from, NetQuaternion to)
	{
		NetQuaternionDelta result = default(NetQuaternionDelta);
		result.isRelative = false;
		result.sel = to.sel;
		result.x = to.x;
		result.y = to.y;
		result.z = to.z;
		return result;
	}

	public static NetQuaternion AddDelta(NetQuaternion from, NetQuaternionDelta delta)
	{
		if (delta.isRelative)
		{
			return new NetQuaternion(delta.sel, from.x + delta.x, from.y + delta.y, from.z + delta.z, from.bits);
		}
		return new NetQuaternion(delta.sel, delta.x, delta.y, delta.z, from.bits);
	}

	public void Write(NetStream stream)
	{
		stream.Write(sel, 2);
		stream.Write(x, bits);
		stream.Write(y, bits);
		stream.Write(z, bits);
	}

	public static NetQuaternion Read(NetStream stream, ushort bits)
	{
		return new NetQuaternion((byte)(stream.ReadInt32(2) & 3), stream.ReadInt32(bits), stream.ReadInt32(bits), stream.ReadInt32(bits), bits);
	}

	public static bool operator ==(NetQuaternion a, NetQuaternion b)
	{
		return a.sel == b.sel && a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static bool operator !=(NetQuaternion a, NetQuaternion b)
	{
		return !(a == b);
	}

	public override string ToString()
	{
		Quaternion quaternion = Dequantize();
		return $"({sel},{x},{y},{z}) ({quaternion.x},{quaternion.y},{quaternion.z},{quaternion.w}) {quaternion.eulerAngles}";
	}
}
