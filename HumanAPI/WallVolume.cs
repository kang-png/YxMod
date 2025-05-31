using System;
using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class WallVolume : MonoBehaviour
{
	public float gridSize = 0.5f;

	public List<WallVolumeFace> faces = new List<WallVolumeFace>();

	private static Vector3[] vertices;

	private static Vector3[] normals;

	private static int[] tris;

	[NonSerialized]
	public Vector3 gizmoStart;

	[NonSerialized]
	public Vector3 gizmoOffset;

	[NonSerialized]
	public Vector3 gizmoSize;

	[NonSerialized]
	public Color gizmoColor;

	private void Resize<T>(ref T[] array, int minSize, int desiredSize)
	{
		int num = ((array == null) ? minSize : array.Length);
		int num2;
		for (num2 = num; num2 < desiredSize; num2 *= 2)
		{
		}
		while (num2 > minSize && num2 / 2 > desiredSize)
		{
			num2 /= 2;
		}
		if (num2 != num || array == null)
		{
			array = new T[num2];
		}
	}

	public void FillMesh(Mesh mesh, bool forceExact)
	{
		if (forceExact)
		{
			vertices = new Vector3[faces.Count * 4];
			normals = new Vector3[faces.Count * 4];
			tris = new int[faces.Count * 2 * 6];
		}
		else
		{
			Resize(ref vertices, 64, faces.Count * 4);
			Resize(ref normals, 64, faces.Count * 4);
			Resize(ref tris, 96, faces.Count * 6);
		}
		for (int i = 0; i < faces.Count; i++)
		{
			WallVolumeFace wallVolumeFace = faces[i];
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;
			Vector3 vector4;
			switch (wallVolumeFace.orientation)
			{
			case WallVolumeOrientaion.Back:
				vector = new Vector3(wallVolumeFace.posX, wallVolumeFace.posY, wallVolumeFace.posZ);
				vector2 = Vector3.back;
				vector3 = Vector3.up;
				vector4 = Vector3.right;
				break;
			case WallVolumeOrientaion.Right:
				vector = new Vector3(wallVolumeFace.posX + 1, wallVolumeFace.posY, wallVolumeFace.posZ);
				vector2 = Vector3.right;
				vector3 = Vector3.up;
				vector4 = Vector3.forward;
				break;
			case WallVolumeOrientaion.Forward:
				vector = new Vector3(wallVolumeFace.posX + 1, wallVolumeFace.posY, wallVolumeFace.posZ + 1);
				vector2 = Vector3.forward;
				vector3 = Vector3.up;
				vector4 = Vector3.left;
				break;
			case WallVolumeOrientaion.Left:
				vector = new Vector3(wallVolumeFace.posX, wallVolumeFace.posY, wallVolumeFace.posZ + 1);
				vector2 = Vector3.left;
				vector3 = Vector3.up;
				vector4 = Vector3.back;
				break;
			case WallVolumeOrientaion.Down:
				vector = new Vector3(wallVolumeFace.posX, wallVolumeFace.posY, wallVolumeFace.posZ + 1);
				vector2 = Vector3.down;
				vector3 = Vector3.back;
				vector4 = Vector3.right;
				break;
			case WallVolumeOrientaion.Up:
				vector = new Vector3(wallVolumeFace.posX, wallVolumeFace.posY + 1, wallVolumeFace.posZ);
				vector2 = Vector3.up;
				vector3 = Vector3.forward;
				vector4 = Vector3.right;
				break;
			default:
				throw new InvalidOperationException();
			}
			ref Vector3 reference = ref vertices[i * 4];
			reference = vector * gridSize;
			ref Vector3 reference2 = ref vertices[i * 4 + 1];
			reference2 = (vector + vector3) * gridSize;
			ref Vector3 reference3 = ref vertices[i * 4 + 2];
			reference3 = (vector + vector4 + vector3) * gridSize;
			ref Vector3 reference4 = ref vertices[i * 4 + 3];
			reference4 = (vector + vector4) * gridSize;
			normals[i * 4] = vector2;
			normals[i * 4 + 1] = vector2;
			normals[i * 4 + 2] = vector2;
			normals[i * 4 + 3] = vector2;
			tris[i * 6] = i * 4;
			tris[i * 6 + 1] = i * 4 + 1;
			tris[i * 6 + 2] = i * 4 + 2;
			tris[i * 6 + 3] = i * 4;
			tris[i * 6 + 4] = i * 4 + 2;
			tris[i * 6 + 5] = i * 4 + 3;
		}
		for (int j = faces.Count * 6; j < tris.Length; j++)
		{
			tris[j] = 0;
		}
		mesh.name = base.name;
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.triangles = tris;
		mesh.RecalculateBounds();
	}

	private bool IsOpen(int x, int y, int z, WallVolumeOrientaion orientation)
	{
		bool flag = false;
		switch (orientation)
		{
		case WallVolumeOrientaion.Up:
		{
			for (int l = 0; l < faces.Count; l++)
			{
				if (faces[l].posX == x && faces[l].posZ == z && faces[l].posY > y && (faces[l].orientation == WallVolumeOrientaion.Up || faces[l].orientation == WallVolumeOrientaion.Down))
				{
					flag = !flag;
				}
			}
			break;
		}
		case WallVolumeOrientaion.Down:
		{
			for (int n = 0; n < faces.Count; n++)
			{
				if (faces[n].posX == x && faces[n].posZ == z && faces[n].posY < y && (faces[n].orientation == WallVolumeOrientaion.Up || faces[n].orientation == WallVolumeOrientaion.Down))
				{
					flag = !flag;
				}
			}
			break;
		}
		case WallVolumeOrientaion.Left:
		{
			for (int j = 0; j < faces.Count; j++)
			{
				if (faces[j].posY == y && faces[j].posZ == z && faces[j].posX < x && (faces[j].orientation == WallVolumeOrientaion.Left || faces[j].orientation == WallVolumeOrientaion.Right))
				{
					flag = !flag;
				}
			}
			break;
		}
		case WallVolumeOrientaion.Right:
		{
			for (int m = 0; m < faces.Count; m++)
			{
				if (faces[m].posY == y && faces[m].posZ == z && faces[m].posX > x && (faces[m].orientation == WallVolumeOrientaion.Left || faces[m].orientation == WallVolumeOrientaion.Right))
				{
					flag = !flag;
				}
			}
			break;
		}
		case WallVolumeOrientaion.Forward:
		{
			for (int k = 0; k < faces.Count; k++)
			{
				if (faces[k].posX == x && faces[k].posY == y && faces[k].posZ > z && (faces[k].orientation == WallVolumeOrientaion.Forward || faces[k].orientation == WallVolumeOrientaion.Back))
				{
					flag = !flag;
				}
			}
			break;
		}
		case WallVolumeOrientaion.Back:
		{
			for (int i = 0; i < faces.Count; i++)
			{
				if (faces[i].posX == x && faces[i].posY == y && faces[i].posZ < z && (faces[i].orientation == WallVolumeOrientaion.Forward || faces[i].orientation == WallVolumeOrientaion.Back))
				{
					flag = !flag;
				}
			}
			break;
		}
		default:
			throw new InvalidOperationException();
		}
		return flag;
	}

	private void ClearVoxel(int x, int y, int z, int brushX, int brushY, int brushZ)
	{
		for (int i = 0; i < faces.Count; i++)
		{
			WallVolumeFace wallVolumeFace = faces[i];
			if (((wallVolumeFace.posX >= x && wallVolumeFace.posX <= x + brushX - 1) || (wallVolumeFace.posX == x - 1 && wallVolumeFace.orientation == WallVolumeOrientaion.Right) || (wallVolumeFace.posX == x + brushX && wallVolumeFace.orientation == WallVolumeOrientaion.Left)) && ((wallVolumeFace.posY >= y && wallVolumeFace.posY <= y + brushY - 1) || (wallVolumeFace.posY == y - 1 && wallVolumeFace.orientation == WallVolumeOrientaion.Up) || (wallVolumeFace.posY == y + brushY && wallVolumeFace.orientation == WallVolumeOrientaion.Down)) && ((wallVolumeFace.posZ >= z && wallVolumeFace.posZ <= z + brushZ - 1) || (wallVolumeFace.posZ == z - 1 && wallVolumeFace.orientation == WallVolumeOrientaion.Forward) || (wallVolumeFace.posZ == z + brushZ && wallVolumeFace.orientation == WallVolumeOrientaion.Back)))
			{
				faces.RemoveAt(i);
				i--;
			}
		}
	}

	public void AddVoxel(int x, int y, int z, int brushX, int brushY, int brushZ)
	{
		ClearVoxel(x, y, z, brushX, brushY, brushZ);
		for (int i = 0; i < brushX; i++)
		{
			for (int j = 0; j < brushY; j++)
			{
				if (!IsOpen(x + i, y + j, z, WallVolumeOrientaion.Back))
				{
					faces.Add(new WallVolumeFace(x + i, y + j, z, WallVolumeOrientaion.Back));
				}
				if (!IsOpen(x + i, y + j, z + brushZ - 1, WallVolumeOrientaion.Forward))
				{
					faces.Add(new WallVolumeFace(x + i, y + j, z + brushZ - 1, WallVolumeOrientaion.Forward));
				}
			}
		}
		for (int k = 0; k < brushY; k++)
		{
			for (int l = 0; l < brushZ; l++)
			{
				if (!IsOpen(x, y + k, z + l, WallVolumeOrientaion.Left))
				{
					faces.Add(new WallVolumeFace(x, y + k, z + l, WallVolumeOrientaion.Left));
				}
				if (!IsOpen(x + brushX - 1, y + k, z + l, WallVolumeOrientaion.Right))
				{
					faces.Add(new WallVolumeFace(x + brushX - 1, y + k, z + l, WallVolumeOrientaion.Right));
				}
			}
		}
		for (int m = 0; m < brushX; m++)
		{
			for (int n = 0; n < brushZ; n++)
			{
				if (!IsOpen(x + m, y + brushY - 1, z + n, WallVolumeOrientaion.Up))
				{
					faces.Add(new WallVolumeFace(x + m, y + brushY - 1, z + n, WallVolumeOrientaion.Up));
				}
				if (!IsOpen(x + m, y, z + n, WallVolumeOrientaion.Down))
				{
					faces.Add(new WallVolumeFace(x + m, y, z + n, WallVolumeOrientaion.Down));
				}
			}
		}
	}

	public void RemoveVoxel(int x, int y, int z, int brushX, int brushY, int brushZ)
	{
		ClearVoxel(x, y, z, brushX, brushY, brushZ);
		for (int i = 0; i < brushX; i++)
		{
			for (int j = 0; j < brushY; j++)
			{
				if (IsOpen(x + i, y + j, z, WallVolumeOrientaion.Back))
				{
					faces.Add(new WallVolumeFace(x + i, y + j, z - 1, WallVolumeOrientaion.Forward));
				}
				if (IsOpen(x + i, y + j, z + brushZ - 1, WallVolumeOrientaion.Forward))
				{
					faces.Add(new WallVolumeFace(x + i, y + j, z + brushZ, WallVolumeOrientaion.Back));
				}
			}
		}
		for (int k = 0; k < brushY; k++)
		{
			for (int l = 0; l < brushZ; l++)
			{
				if (IsOpen(x, y + k, z + l, WallVolumeOrientaion.Left))
				{
					faces.Add(new WallVolumeFace(x - 1, y + k, z + l, WallVolumeOrientaion.Right));
				}
				if (IsOpen(x + brushX - 1, y + k, z + l, WallVolumeOrientaion.Right))
				{
					faces.Add(new WallVolumeFace(x + brushX, y + k, z + l, WallVolumeOrientaion.Left));
				}
			}
		}
		for (int m = 0; m < brushX; m++)
		{
			for (int n = 0; n < brushZ; n++)
			{
				if (IsOpen(x + m, y + brushY - 1, z + n, WallVolumeOrientaion.Up))
				{
					faces.Add(new WallVolumeFace(x + m, y + brushY, z + n, WallVolumeOrientaion.Down));
				}
				if (IsOpen(x + m, y, z + n, WallVolumeOrientaion.Down))
				{
					faces.Add(new WallVolumeFace(x + m, y - 1, z + n, WallVolumeOrientaion.Up));
				}
			}
		}
	}

	public void OnDrawGizmosSelected()
	{
		if (!(gizmoSize == Vector3.zero))
		{
			Gizmos.color = gizmoColor;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawCube(gizmoStart + gizmoSize / 2f, gizmoSize + gizmoOffset);
			Gizmos.DrawWireCube(gizmoStart + gizmoSize / 2f, gizmoSize + gizmoOffset);
		}
	}
}
