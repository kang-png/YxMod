using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace I2.Loc;

public class Localize : MonoBehaviour
{
	public bool UsePlatformSpecificLocalization;

	public List<PlatformSpecificLocalization> platformToModifyFor = new List<PlatformSpecificLocalization>();

	private string LastLocalizedLanguage;

	public string mTerm = string.Empty;

	public string mTermSecondary = string.Empty;

	public bool LocalizeOnAwake = true;

	public Object mTarget;

	public string Term
	{
		get
		{
			return mTerm;
		}
		set
		{
			SetTerm(value);
		}
	}

	public string SecondaryTerm
	{
		get
		{
			return mTermSecondary;
		}
		set
		{
			SetTerm(null, value);
		}
	}

	public void SetTerm(string primary, string secondary = null)
	{
		if (!string.IsNullOrEmpty(primary))
		{
			mTerm = primary;
		}
		if (!string.IsNullOrEmpty(secondary))
		{
			mTermSecondary = secondary;
		}
		OnLocalize(Force: true);
	}

	private void Awake()
	{
		if (mTarget == null)
		{
			mTarget = GetComponent<TextMeshPro>();
			if (mTarget == null)
			{
				mTarget = GetComponent<TextMeshProUGUI>();
			}
		}
		if (LocalizeOnAwake)
		{
			OnLocalize();
		}
	}

	public void OnLocalize(bool Force = false)
	{
		if (string.IsNullOrEmpty(mTerm) || mTarget == null || (!Force && (!base.enabled || base.gameObject == null || !base.gameObject.activeInHierarchy)) || string.IsNullOrEmpty(LocalizationManager.CurrentLanguage) || (!Force && LastLocalizedLanguage == LocalizationManager.CurrentLanguage))
		{
			return;
		}
		LastLocalizedLanguage = LocalizationManager.CurrentLanguage;
		TMP_Text tMP_Text = mTarget as TextMeshProUGUI;
		if (tMP_Text == null)
		{
			tMP_Text = mTarget as TextMeshPro;
		}
		if (UsePlatformSpecificLocalization)
		{
			foreach (PlatformSpecificLocalization item in platformToModifyFor)
			{
				if (item.currentPlatform == Platforms.windows)
				{
					mTerm = item.Term;
				}
			}
		}
		string termTranslation = LocalizationManager.GetTermTranslation(mTerm);
		if ((bool)tMP_Text)
		{
			tMP_Text.text = termTranslation;
		}
		else
		{
			Debug.Log("text box wrong type: " + base.name + " " + mTerm);
		}
	}
}
