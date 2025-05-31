using UnityEngine;

namespace Yondernauts.LayerManager;

public class LayerMap : ScriptableObject
{
	[SerializeField]
	private int[] m_Map;

	public int TransformLayer(int old)
	{
		if (old < 0)
		{
			old = 0;
		}
		if (old >= 32)
		{
			old = 0;
		}
		return m_Map[old];
	}

	public int TransformMask(int old)
	{
		int num = 0;
		for (int i = 0; i < 32; i++)
		{
			int num2 = (old >> i) & 1;
			num |= num2 << TransformLayer(i);
		}
		return num;
	}
}
