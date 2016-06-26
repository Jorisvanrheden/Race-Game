using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioController :MonoBehaviour {

	public AudioClip[] clips = new AudioClip[2]; 
	public AudioClip[] effects = new AudioClip[3]; 
	private AudioSource source;

	void Awake(){
		source = GetComponent<AudioSource> ();
	}

	public void playClip(int index){
		source.Stop ();
		source.PlayOneShot(clips[index]);
	}

	public void stopClip(){
		source.Stop ();
	}

	public void playEffect(int index){
		source.PlayOneShot(effects[index]);
	}
}
