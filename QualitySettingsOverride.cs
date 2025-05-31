using UnityEngine;

public class QualitySettingsOverride : MonoBehaviour
{
	[Header("Shadow Distance")]
	public bool overrideShadowDistance;

	public float maxShadowDistance = 50f;

	public float minShadowDistance;

	private float cachedShadowDistance;

	[Header("Shadow Quality")]
	public bool overrideShadowQuality;

	public ShadowQuality minShadowQuality = ShadowQuality.HardOnly;

	private ShadowQuality cachedShadowQuality;

	[Header("Shadow Cascade")]
	public bool overrideShadowCascade;

	public int maxShadowCascade = 2;

	private int cachedShadowCascade;

	[Header("LOD Bias")]
	public bool overrideLODBias;

	public float LODBias = 2f;

	private float cachedLODBias;

	[Header("Anti Aliasing TAA Settings")]
	public GameObject[] objectsToHide;

	private int cachedQualityLevel;

	private void OnEnable()
	{
		cachedQualityLevel = QualitySettings.GetQualityLevel();
		cachedShadowDistance = QualitySettings.shadowDistance;
		cachedShadowQuality = QualitySettings.shadows;
		cachedShadowCascade = QualitySettings.shadowCascades;
		cachedLODBias = QualitySettings.lodBias;
		AdvancedVideoMenu.OnOptionsChanged += SetTAA;
	}

	private void Start()
	{
		SetTAA();
	}

	private void OverrideSettings()
	{
		ResetOverriddenSettings();
		cachedQualityLevel = QualitySettings.GetQualityLevel();
		if (overrideShadowDistance)
		{
			cachedShadowDistance = QualitySettings.shadowDistance;
			if (maxShadowDistance < cachedShadowDistance)
			{
				QualitySettings.shadowDistance = maxShadowDistance;
			}
			else if (minShadowDistance > cachedShadowDistance)
			{
				QualitySettings.shadowDistance = minShadowDistance;
			}
		}
		if (overrideShadowQuality)
		{
			cachedShadowQuality = QualitySettings.shadows;
			if (minShadowQuality > cachedShadowQuality)
			{
				QualitySettings.shadows = minShadowQuality;
			}
		}
		if (overrideShadowCascade)
		{
			cachedShadowCascade = QualitySettings.shadowCascades;
			if (maxShadowCascade < cachedShadowCascade)
			{
				QualitySettings.shadowCascades = maxShadowCascade;
			}
		}
		if (overrideLODBias)
		{
			cachedLODBias = QualitySettings.lodBias;
			QualitySettings.lodBias = LODBias;
		}
	}

	public void SetTAA()
	{
		if (objectsToHide.Length > 0)
		{
			bool active = Options.advancedVideoAA != 3;
			for (int i = 0; i < objectsToHide.Length; i++)
			{
				objectsToHide[i].gameObject.SetActive(active);
			}
		}
		OverrideSettings();
	}

	private void ResetOverriddenSettings()
	{
		int qualityLevel = QualitySettings.GetQualityLevel();
		QualitySettings.SetQualityLevel(cachedQualityLevel);
		if (overrideShadowDistance)
		{
			QualitySettings.shadowDistance = cachedShadowDistance;
		}
		if (overrideShadowCascade)
		{
			QualitySettings.shadowCascades = cachedShadowCascade;
		}
		if (overrideShadowQuality)
		{
			QualitySettings.shadows = cachedShadowQuality;
		}
		if (overrideLODBias)
		{
			QualitySettings.lodBias = cachedLODBias;
		}
		QualitySettings.SetQualityLevel(qualityLevel);
	}

	private void OnDisable()
	{
		ResetOverriddenSettings();
		AdvancedVideoMenu.OnOptionsChanged -= SetTAA;
	}
}
