using System;
using UnityEngine;
public enum EPlayerState
{
    Jump,
    Landing,
    Hit,
    Attack,
    Dash,
    Death,

    Skill1,
    Skill2,
    Skill3,
    Skill4,
    Skill5,
    Skill6,
    Skill7,
    Skill8

}

[Serializable]
public class SoundComponent
{
    public EPlayerState state;
    public AudioClip clip;
}

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{
    public SoundComponent[] SoundComponents;
    public AudioSource AudioSource;

    public void Reset()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public void Play(EPlayerState state)
    {
        foreach(SoundComponent soundComponent in SoundComponents)
        {
            if (soundComponent.state == state)
            {
                if (soundComponent.clip == null) return;
                AudioSource.clip = soundComponent.clip;
                Debug.Log($"[Sound] Playing clip: {soundComponent.clip.name} for state {state}");
                AudioSource.Play();
                return;
            }
        }


        Debug.LogWarning($"[Sound] No matching clip found for state: {state}");
    }
}
