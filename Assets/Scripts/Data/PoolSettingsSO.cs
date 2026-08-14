using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolSettings", menuName = "Pool/Settings")]

public class PoolSettingsSO : ScriptableObject
{
    public List<PoolSetting> poolSettings;
}

[Serializable]
public class PoolSetting
{
    public MonoBehaviour prefab;
    public int quantity;
}