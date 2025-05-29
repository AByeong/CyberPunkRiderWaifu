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
        
        Elite_Electricity,
        
        
        Elite_Female_Walk,
        Elite_Female_Step,
        Elite_Female_KingStamp,
        Elite_Female_Detect,
        Elite_Female_Hit,
        Elite_Female_Tornado,
        Elite_Female_Die,
        
        Elite_male_Walk,
        Elite_male_Step,
        Elite_male_KingStamp,
        Elite_male_Detect,
        Elite_male_Hit,
        Elite_male_Tornado,
        Elite_male_Die,
        
        
        
        
        
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
        foreach(SoundStruct soundComponent in Sounds)
        {
           
//노말 몬스터의 피격 사운드 설정
            if (soundComponent.state == SoundType.NormalEnemy_Hit)
            {
                NormalEnemySource.clip = soundComponent.clip;
                return;
            }
        }
        
        
        if(Source == null) { Source = GetComponent<AudioSource>(); }
        base.Awake();
    }
    
    
    public void Play(SoundType state)
    {
        if (state == SoundType.NormalEnemy_Hit)
        {
            if (!NormalEnemySource.isPlaying)
            {
                //노말 몬스터는 따로 사운드 출력
                NormalEnemySource.Play();
                return;
            }
        }
        
        foreach(SoundStruct soundComponent in Sounds)
        {
           

            if (soundComponent.state == state)
            {
                if (soundComponent.clip == null) return;
                Source.PlayOneShot(soundComponent.clip);
                return;
            }
        }

    }
    
}
