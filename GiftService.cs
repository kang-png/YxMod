using System;
using System.Collections;
using UnityEngine;

public class GiftService : MonoBehaviour, IDependency
{
	public TextAsset jsonStatus;

	public TextAsset jsonDeliver;

	public static XmasStatus status;

	public static GiftService instance;

	private int activeCalls;

	public static event Action<XmasStatus> statusUpdated;

	private void Awake()
	{
		instance = this;
	}

	private void SetStatus(string json)
	{
		try
		{
			XmasStatus xmasStatus = JsonUtility.FromJson<XmasStatus>(json);
			if (xmasStatus != null)
			{
				status = xmasStatus;
				if (GiftService.statusUpdated != null)
				{
					GiftService.statusUpdated(status);
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void RefreshStatus(Action onResponse = null)
	{
		if (activeCalls <= 0)
		{
			StartCoroutine(GetStatusCoroutine(onResponse));
		}
	}

	private IEnumerator GetStatusCoroutine(Action onResponse)
	{
		activeCalls++;
		WWW www = new WWW("https://hff.terahard.org/api/get-status.php");
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.Log(www.error);
			SetStatus("{\"prize\":\"Guns\"}");
			onResponse?.Invoke();
		}
		else
		{
			SetStatus(www.text);
			onResponse?.Invoke();
		}
		www.Dispose();
		activeCalls--;
	}

	public void DeliverGift(ulong user, uint gift)
	{
		StartCoroutine(DeliverGiftCoroutine(user, gift));
	}

	private IEnumerator DeliverGiftCoroutine(ulong user, uint gift)
	{
		activeCalls++;
		WWW www = new WWW($"https://hff.terahard.org/api/deliver-gift.php?userId={user}&giftId={gift}");
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.Log(www.error);
		}
		else
		{
			SetStatus(www.text);
		}
		www.Dispose();
		activeCalls--;
	}

	public void Initialize()
	{
		RefreshStatus();
	}
}
