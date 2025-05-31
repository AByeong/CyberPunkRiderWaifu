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
    
}

[Serializable]
public class SoundComponent
{
    public EPlayerState state;
    public AudioClip[] clip;
}

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{
    public SoundComponent[] SoundComponents;
    public AudioSource AudioSource;
    
    private int _previousSoundIndex;
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
                if (soundComponent.clip.Length <= 0) return;

                // 클립이 하나밖에 없으면 그냥 재생
                if (soundComponent.clip.Length == 1)
                {
                    AudioSource.clip = soundComponent.clip[0];
                    AudioSource.Play();
                    _previousSoundIndex = 0;
                    return;
                }

                // 이전 인덱스를 제외한 새 배열 생성
                AudioClip[] newClips = new AudioClip[soundComponent.clip.Length - 1];
                int[] newIndices = new int[soundComponent.clip.Length - 1];

                for (int i = 0, j = 0; i < soundComponent.clip.Length; i++)
                {
                    if (i == _previousSoundIndex) continue;
                    newClips[j] = soundComponent.clip[i];
                    newIndices[j] = i;
                    j++;
                }

                int randomIndexInNew = UnityEngine.Random.Range(0, newClips.Length);
                AudioSource.clip = newClips[randomIndexInNew];
                AudioSource.Play();

                _previousSoundIndex = newIndices[randomIndexInNew];
                return;
            }

        }


        Debug.LogWarning($"[Sound] No matching clip found for state: {state}");
    }

    
    public void PlaySound(int soundState)
    {
        // EPlayerState 값 참고:
        // Attack = 3
        Play((EPlayerState)soundState);
    }
}
