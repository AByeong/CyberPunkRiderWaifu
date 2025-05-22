using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SkillDataList", menuName = "Scriptable Objects/SkillDataList")]
public class SkillDataList : ScriptableObject
{
    public List<SkillData> SkillData;
}
