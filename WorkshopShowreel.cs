using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using I2.Loc;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkshopShowreel : MonoBehaviour, IPointerExitHandler, IEventSystemHandler
{
	public Image panelImage;

	public Image levelImage;

	public Button startButton;

	public Button button;

	public TextMeshProUGUI titleText;

	private int currentHighlight;

	private float rotationSpeed = 3f;

	[SerializeField]
	private Vector2 HFFVRPanelImageSize;

	[SerializeField]
	private Vector2 HFFVRPanelImagePosition;

	[SerializeField]
	private Vector2 HFFVRImageSize;

	[SerializeField]
	private Vector2 HFFVRImagePosition;

	private bool isChinese
	{
		[CompilerGenerated]
		get
		{
			return LocalizationManager.CurrentLanguage == "Chinese Taiwan" || LocalizationManager.CurrentLanguage == "Chinese Simplified";
		}
	}

	private List<GetWorkshopHighlights.Highlight> highlightsToUse
	{
		[CompilerGenerated]
		get
		{
			return (GetWorkshopHighlights.Instance == null) ? null : ((!isChinese) ? GetWorkshopHighlights.EnglishHighlights : GetWorkshopHighlights.ChineseHighlights);
		}
	}

	private void OnEnable()
	{
		if (GetWorkshopHighlights.Instance.IsHFFVROnly)
		{
			panelImage.rectTransform.anchoredPosition = HFFVRPanelImagePosition;
			panelImage.rectTransform.sizeDelta = HFFVRPanelImageSize;
			levelImage.rectTransform.anchoredPosition = HFFVRImagePosition;
			levelImage.rectTransform.sizeDelta = HFFVRImageSize;
		}
		StartCoroutine(SetNextHighlight());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator SetNextHighlight(float waitTime = 0f)
	{
		for (float waitTimer = waitTime; waitTimer > 0f; waitTimer -= Time.deltaTime)
		{
			while (EventSystem.current.currentSelectedGameObject == base.gameObject)
			{
				yield return null;
			}
			yield return null;
		}
		if (highlightsToUse != null && highlightsToUse.Count != 0)
		{
			if (++currentHighlight >= highlightsToUse.Count)
			{
				currentHighlight = 0;
			}
			SetHighlight(highlightsToUse[currentHighlight]);
		}
	}

	public void SetHighlight(GetWorkshopHighlights.Highlight highlight)
	{
		panelImage.enabled = true;
		levelImage.enabled = true;
		titleText.enabled = !GetWorkshopHighlights.Instance.IsHFFVROnly;
		button.enabled = true;
		levelImage.sprite = highlight.sprite;
		levelImage.preserveAspect = true;
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(delegate
		{
			SteamFriends.ActivateGameOverlayToWebPage(highlight.link);
		});
		StartCoroutine(SetNextHighlight(rotationSpeed));
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		MenuSystem.instance.activeMenu.lastFocusedElement = startButton.gameObject;
		EventSystem.current.SetSelectedGameObject(startButton.gameObject);
	}
}
