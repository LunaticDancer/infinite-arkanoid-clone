using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

	[SerializeField] AudioSource source;
    [SerializeField] AudioClip[] powerUpSounds;
    [SerializeField] AudioClip[] destroySounds;

	private void Awake()
	{
		Instance = this;
	}

	public void PlayRandomDestroySound()
	{
		source.PlayOneShot(destroySounds[Random.Range(0, destroySounds.Length)]);
	}

	public void PlayPowerUpSound()
	{
		source.PlayOneShot(powerUpSounds[Random.Range(0, powerUpSounds.Length)]);
	}
}
