using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRepository : MonoBehaviour
{
	public static TutorialRepository instance;

	public TutorialRepositoryItem[] items;

	private void OnEnable()
	{
		instance = this;
		items = GetComponentsInChildren<TutorialRepositoryItem>();
	}

	public List<string> ListShownItems(int lastAvailableLevel, int lastAvailableCP)
	{
		List<string> list = new List<string>();
		TutorialRepositoryItem[] array = items;
		foreach (TutorialRepositoryItem tutorialRepositoryItem in array)
		{
			if (tutorialRepositoryItem.showInPause && (tutorialRepositoryItem.level < lastAvailableLevel || (tutorialRepositoryItem.level == lastAvailableLevel && tutorialRepositoryItem.checkpoint < lastAvailableCP)))
			{
				string text = tutorialRepositoryItem.GetText();
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(text);
				}
			}
		}
		return list;
	}

	public bool HasAvailableItems()
	{
		return GameSave.IsLevelSelectEnabled();
	}

	public string GetTutorialText(string name)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (string.Equals(items[i].name, name, StringComparison.InvariantCultureIgnoreCase))
			{
				return items[i].GetText();
			}
		}
		return null;
	}
}
