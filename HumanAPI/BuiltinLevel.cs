using Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanAPI;

public class BuiltinLevel : Level
{
	public Transform debugSpawnPoint;

	public GameObject gamePrefab;

	public GameObject resourcePrefab;

	public float customCloudNearClipStart = 5f;

	public float customCloudNearClipEnd = 10f;

	public float customCloudFarClipStart = 150f;

	public float customCloudFarClipEnd = 200f;

	public bool underwaterGravity;

	private void Start()
	{
	}

	protected override void Awake()
	{
		base.Awake();
		if (resourcePrefab != null)
		{
			Object.Instantiate(resourcePrefab);
		}
		CloudSystem.OnSystemInit += delegate
		{
			CloudSystem.instance.nearClipStart = customCloudNearClipStart;
			CloudSystem.instance.nearClipEnd = customCloudNearClipEnd;
			CloudSystem.instance.farClipStart = customCloudFarClipStart;
			CloudSystem.instance.farClipEnd = customCloudFarClipEnd;
		};
	}

	private void OnDisable()
	{
		FreeRoamCam.CleanUp();
		if (underwaterGravity)
		{
			Physics.gravity = new Vector3(0f, -9.81f, 0f);
		}
	}

	protected override void OnEnable()
	{
		if (Game.instance == null)
		{
			Object.Instantiate(gamePrefab);
			Dependencies.Initialize<App>();
			for (int i = 0; i < Game.instance.levelCount; i++)
			{
				if (string.Equals(SceneManager.GetActiveScene().name, Game.instance.levels[i]))
				{
					Game.instance.currentLevelNumber = i;
				}
			}
			App.instance.BeginLevel();
		}
		base.OnEnable();
		if (underwaterGravity)
		{
			Physics.gravity = new Vector3(0f, -5f, 0f);
		}
	}

	public override void CompleteLevel()
	{
		base.CompleteLevel();
		DisableOnExit.ExitingLevel(this);
	}
}
