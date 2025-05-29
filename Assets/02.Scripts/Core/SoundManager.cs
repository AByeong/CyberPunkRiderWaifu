using System;
using UnityEngine;


public enum SoundType
{
        UI_Open,
        UI_Close,
        UI_Click,
        
        NormalEnemy_Attack,
        NormalEnemy_Walk,
        NormalEnemy_Hit,
        NormalEnemy_Die,
        
        Elite_Female_Walk,
        Elite_Female_Step,
        Elite_Female_KingStamp,
        Elite_Female_Detect,
        Elite_Female_Hit,
        Elite_Female_Tornado,
        Elite_Female_Die,
        
        
        
        
        
}


[Serializable]
public class SoundStruct
{
    public SoundType state;
    public AudioClip clip;
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    
    public SoundStruct[] Sounds;
    public AudioSource Source;
    public AudioSource NormalEnemySource;

    private void Awake()
    {
        
        if(Source == null) { Source = GetComponent<AudioSource>(); }
        base.Awake();
    }
    
    
    public void Play(SoundType state)
    {
        foreach(SoundStruct soundComponent in Sounds)
        {
            if (soundComponent.state == SoundType.NormalEnemy_Hit)
            {
                if (!NormalEnemySource.isPlaying)
                {
                    NormalEnemySource.clip = soundComponent.clip;
                    NormalEnemySource.Play();
                }
            }
            else if (soundComponent.state == state)
            {
                if (soundComponent.clip == null) return;
                //Source.clip = soundComponent.clip; 
                Source.PlayOneShot(soundComponent.clip);
                return;
            }
        }

    }
    
}
