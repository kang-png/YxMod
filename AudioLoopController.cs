using UnityEngine;

public class AudioLoopController : MonoBehaviour
{
	public enum LoopControl
	{
		OnStop_StopLoop,
		OnStop_PauseLoop
	}

	public AudioSource start;

	public AudioSource loop;

	public AudioSource end;

	public float startDelay = 1f;

	public void Play()
	{
		start.PlayDelayed(startDelay);
		loop.PlayDelayed(startDelay + start.clip.length);
	}

	public void Pause()
	{
		loop.Pause();
		end.Play();
	}

	public void Stop()
	{
		loop.Stop();
		end.Play();
	}

	public void StopImmediately()
	{
		start.Stop();
		loop.Stop();
		end.Stop();
	}
}
