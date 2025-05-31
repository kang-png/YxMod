using System;
using UnityEngine;

public static class UL_RayMatrices
{
	private static readonly Vector2[] GRID_2x2 = GenerateGrid(2, 2);

	private static readonly Vector2[] GRID_3x3 = GenerateGrid(3, 3);

	private static readonly Vector2[] GRID_4x4 = GenerateGrid(4, 4);

	private static readonly Vector2[] GRID_5x5 = GenerateGrid(5, 5);

	private static readonly Vector2[] GRID_6x6 = GenerateGrid(6, 6);

	private static readonly Vector2[] GRID_7x7 = GenerateGrid(7, 7);

	private static readonly Vector2[] GRID_8x8 = GenerateGrid(8, 8);

	private static readonly Vector2[] GRID_9x9 = GenerateGrid(9, 9);

	private static readonly Vector2[] GRID_10x10 = GenerateGrid(10, 10);

	private static readonly Vector2[] GRID_11x11 = GenerateGrid(11, 11);

	private static readonly Vector2[] GRID_12x12 = GenerateGrid(12, 12);

	private static readonly Vector2[] GRID_13x13 = GenerateGrid(13, 13);

	private static readonly Vector2[] GRID_14x14 = GenerateGrid(14, 14);

	private static readonly Vector2[] GRID_15x15 = GenerateGrid(15, 15);

	public static readonly Vector2[][] GRID = new Vector2[14][]
	{
		GRID_2x2, GRID_3x3, GRID_4x4, GRID_5x5, GRID_6x6, GRID_7x7, GRID_8x8, GRID_9x9, GRID_10x10, GRID_11x11,
		GRID_12x12, GRID_13x13, GRID_14x14, GRID_15x15
	};

	private static readonly Vector3[] SPHERE_2x2 = GenerateSphere(4);

	private static readonly Vector3[] SPHERE_3x3 = GenerateSphere(9);

	private static readonly Vector3[] SPHERE_4x4 = GenerateSphere(16);

	private static readonly Vector3[] SPHERE_5x5 = GenerateSphere(25);

	private static readonly Vector3[] SPHERE_6x6 = GenerateSphere(36);

	private static readonly Vector3[] SPHERE_7x7 = GenerateSphere(49);

	private static readonly Vector3[] SPHERE_8x8 = GenerateSphere(64);

	private static readonly Vector3[] SPHERE_9x9 = GenerateSphere(81);

	private static readonly Vector3[] SPHERE_10x10 = GenerateSphere(100);

	private static readonly Vector3[] SPHERE_11x11 = GenerateSphere(121);

	private static readonly Vector3[] SPHERE_12x12 = GenerateSphere(144);

	private static readonly Vector3[] SPHERE_13x13 = GenerateSphere(169);

	private static readonly Vector3[] SPHERE_14x14 = GenerateSphere(196);

	private static readonly Vector3[] SPHERE_15x15 = GenerateSphere(225);

	public static readonly Vector3[][] SPHERE = new Vector3[14][]
	{
		SPHERE_2x2, SPHERE_3x3, SPHERE_4x4, SPHERE_5x5, SPHERE_6x6, SPHERE_7x7, SPHERE_8x8, SPHERE_9x9, SPHERE_10x10, SPHERE_11x11,
		SPHERE_12x12, SPHERE_13x13, SPHERE_14x14, SPHERE_15x15
	};

	private static Vector2[] GenerateGrid(int width, int height)
	{
		Vector2[] array = new Vector2[width * height];
		float num = 1f / (float)width;
		float num2 = 1f / (float)height;
		int num3 = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				ref Vector2 reference = ref array[num3++];
				reference = new Vector2(2f * ((float)i + 0.5f) * num - 1f, 2f * ((float)j + 0.5f) * num2 - 1f);
			}
		}
		return array;
	}

	private static Vector3[] GenerateSphere(int count)
	{
		Vector3[] array = new Vector3[count];
		float num = (float)Math.PI * (3f - Mathf.Sqrt(5f));
		float num2 = 2f / (float)count;
		for (int i = 0; i < count; i++)
		{
			float num3 = (float)i * num2 - 1f + num2 / 2f;
			float num4 = Mathf.Sqrt(1f - num3 * num3);
			float f = (float)i * num;
			float x = Mathf.Cos(f) * num4;
			float z = Mathf.Sin(f) * num4;
			ref Vector3 reference = ref array[i];
			reference = new Vector3(x, num3, z);
		}
		return array;
	}
}
