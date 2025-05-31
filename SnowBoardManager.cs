using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnowBoardManager : MonoBehaviour
{
	private static SnowBoardManager instance;

	private List<SnowBoard> snowBoards = new List<SnowBoard>();

	public static SnowBoardManager Instance
	{
		[CompilerGenerated]
		get
		{
			SnowBoardManager result = instance ?? Object.Instantiate(new GameObject("SnowBoardManager")).AddComponent<SnowBoardManager>();
			instance = result;
			return result;
		}
	}

	private void Start()
	{
		SceneManager.sceneUnloaded += OnSceneUnloaded;
	}

	private void OnSceneUnloaded(Scene arg0)
	{
		snowBoards.Clear();
		instance = null;
	}

	public void AddSnowBoard(SnowBoard board)
	{
		snowBoards.Add(board);
	}

	public void RemoveSnowBoard(SnowBoard board)
	{
		snowBoards.Remove(board);
	}

	public SnowBoard GetHumansBoard(Human human)
	{
		foreach (SnowBoard snowBoard in snowBoards)
		{
			SnowBoard.Binding[] bindings = snowBoard.bindings;
			foreach (SnowBoard.Binding binding in bindings)
			{
				if (binding.boundTo == human)
				{
					return snowBoard;
				}
			}
		}
		return null;
	}
}
