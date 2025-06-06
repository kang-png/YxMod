using UnityEngine;

namespace HumanAPI;

[AddComponentMenu("Human/Sound/Signal Trigger", 10)]
public class SignalSoundVolumeUp : Node
{
	public NodeInput input;

	[SerializeField]
	private string soundGameObjectName = string.Empty;

	[SerializeField]
	private float targetVolume;

	private bool knownState;

	private Sound2 sound2;

	protected void Awake()
	{
		priority = NodePriority.Update;
	}

	public override void Process()
	{
		base.Process();
		float value = input.value;
		bool flag = value >= 0.5f;
		if (flag == knownState)
		{
			return;
		}
		knownState = flag;
		if (!flag || SignalManager.skipTransitions)
		{
			return;
		}
		GameObject gameObject = GameObject.Find(soundGameObjectName);
		if (gameObject != null)
		{
			sound2 = gameObject.GetComponent<Sound2>();
			if (sound2 != null)
			{
				sound2.SetBaseVolume(0.35f);
			}
		}
	}
}
