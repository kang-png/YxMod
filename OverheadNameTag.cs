using Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class OverheadNameTag : MonoBehaviour
{
	public Text textMesh;

	public float maxScale = 0.3f;

	public float minScale = 0.15f;

	public float maxScaleDistance = 10f;

	public float minOffsetFromHead = 0.5f;

	public float maxOffsetFromHead = 5f;

	public float rotateSpeed = 1f;

	public float FadeInDuration = 0.1f;

	public float FadeOutDuration = 0.2f;

	public float minWidth = 337f;

	public GameObject Child;

	public SpriteRenderer SpeakerSprite;

	public Sprite TalkingSprite;

	public Sprite NotTalkingSprite;

	public Sprite MutedSprite;

	public float waitTimeOnForceShow = 5f;

	private NetPlayer player;

	private Image childBackground;

	private static Camera mainCamera;

	private float TransitionTimer;

	private bool TransitionInProgress;

	private bool isTalking;

	public float MinimumBgWidth;

	private bool forceShow;

	private float currentWaitTime;

	private float SpeakerSize;

	private float sizeAddition = 0.075f;

	private float initialBGAlpha;

	private float getChildWidth => textMesh.preferredWidth * textMesh.rectTransform.localScale.x + SpeakerSize + sizeAddition;

	private void Start()
	{
		player = GetComponentInParent<NetPlayer>();
		childBackground = Child.GetComponent<Image>();
		if ((bool)childBackground)
		{
			initialBGAlpha = childBackground.color.a;
			childBackground.color = new Color(1f, 1f, 1f, 0f);
		}
		if (player == null)
		{
			Child.SetActive(value: false);
		}
		else
		{
			player.overHeadNameTag = this;
		}
		SpeakerSprite.sprite = NotTalkingSprite;
		Child.SetActive(value: false);
		SpeakerSprite.enabled = false;
	}

	private void OnEnable()
	{
		textMesh.text = "WW";
		MinimumBgWidth = getChildWidth;
		textMesh.text = string.Empty;
		AdjustTagWidth();
	}

	private void FadeTransition(bool FadeIn)
	{
		if (!Child || !childBackground || !SpeakerSprite || !textMesh)
		{
			return;
		}
		if (FadeIn)
		{
			if (!Child.activeSelf)
			{
				Child.SetActive(value: true);
			}
			if (TransitionTimer < 1f)
			{
				TransitionTimer += Time.unscaledDeltaTime / FadeInDuration;
				TransitionTimer = Mathf.Clamp01(TransitionTimer);
				childBackground.color = new Color(childBackground.color.r, childBackground.color.g, childBackground.color.b, TransitionTimer * initialBGAlpha);
				SpeakerSprite.color = new Color(SpeakerSprite.color.r, SpeakerSprite.color.g, SpeakerSprite.color.b, TransitionTimer);
				textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, TransitionTimer);
			}
			else if (TransitionInProgress)
			{
				TransitionInProgress = false;
			}
		}
		else if (TransitionTimer > 0f)
		{
			TransitionTimer -= Time.unscaledDeltaTime / FadeOutDuration;
			TransitionTimer = Mathf.Clamp01(TransitionTimer);
			childBackground.color = new Color(childBackground.color.r, childBackground.color.g, childBackground.color.b, TransitionTimer * initialBGAlpha);
			SpeakerSprite.color = new Color(SpeakerSprite.color.r, SpeakerSprite.color.g, SpeakerSprite.color.b, TransitionTimer);
			textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, TransitionTimer);
		}
		else if (Child.activeSelf)
		{
			Child.SetActive(value: false);
			TransitionInProgress = false;
		}
	}

	private void Update()
	{
	}

	public void UpdateNameTag(ChatUser user)
	{
		textMesh.text = user.GamerTag;
		SpeakerSprite.enabled = false;
		AdjustTagWidth();
	}

	private void AdjustTagWidth()
	{
		if (!string.IsNullOrEmpty(textMesh.text) && textMesh.text.Length > 16)
		{
			textMesh.text = textMesh.text.Substring(0, 16) + "…";
		}
		RectTransform component = Child.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2((!(getChildWidth < MinimumBgWidth)) ? getChildWidth : MinimumBgWidth, component.rect.height);
	}
}
