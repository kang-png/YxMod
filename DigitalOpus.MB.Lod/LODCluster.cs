using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Lod;

public interface LODCluster
{
	int nextCheckFrame { get; set; }

	bool Contains(Vector3 p);

	bool Intersects(Bounds b);

	bool Intersects(Plane[][] fustrum);

	Vector3 Center();

	void Destroy();

	void Clear();

	bool IsVisible();

	float DistSquaredToPlayer();

	List<LODCombinedMesh> GetCombiners();

	void CheckIntegrity();

	void DrawGizmos();

	LODClusterManager GetClusterManager();

	void RemoveAndRecycleCombiner(LODCombinedMesh cl);

	void AddCombiner(LODCombinedMesh cl);

	LODCombinedMesh SuggestCombiner();

	void AssignLODToCombiner(MB2_LOD l);

	void UpdateSkinnedMeshApproximateBounds();

	void PrePrioritize(Plane[][] fustrum, Vector3[] cameraPositions);

	HashSet<LODCombinedMesh> AdjustForMaxAllowedPerLevel();

	void ForceCheckIfLODsChanged();
}
