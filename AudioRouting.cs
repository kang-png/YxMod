using System;
using HumanAPI;
using UnityEngine;
using UnityEngine.Audio;

public class AudioRouting : MonoBehaviour
{
	public AudioMixerGroup footsteps;

	public AudioMixerGroup body;

	public AudioMixerGroup grab;

	public AudioMixerGroup dialogue;

	public AudioMixerGroup tutorials;

	public AudioMixerGroup music;

	public AudioMixerGroup ambience;

	public AudioMixerGroup effects;

	public AudioMixerGroup physics;

	public AudioMixerGroup ambienceFX;

	private static AudioRouting instance;

	private void Awake()
	{
		instance = this;
	}

	public static AudioMixerGroup GetChannel(AudioChannel channel)
	{
		return channel switch
		{
			AudioChannel.Footsteps => instance.footsteps, 
			AudioChannel.Body => instance.body, 
			AudioChannel.Grab => instance.grab, 
			AudioChannel.Dialogue => instance.dialogue, 
			AudioChannel.Tutorials => instance.tutorials, 
			AudioChannel.Music => instance.music, 
			AudioChannel.Ambience => instance.ambience, 
			AudioChannel.Effects => instance.effects, 
			AudioChannel.Physics => instance.physics, 
			AudioChannel.AmbienceFX => instance.ambienceFX, 
			_ => throw new InvalidOperationException(), 
		};
	}
}
