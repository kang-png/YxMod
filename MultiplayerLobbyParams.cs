using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MultiplayerLobbyParams
{
	public string Name;

	[Range(1f, 31f)]
	public int StartDateDay;

	[Range(1f, 12f)]
	public int StartDateMonth;

	[Range(1f, 31f)]
	public int EndDateDay;

	[Range(1f, 12f)]
	public int EndDateMonth;

	public List<string> LobbyNamesOrder;
}
