using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Enemy Details", menuName = "Scriptable Objects/SiwDev/All Enemy Details")]
public class AllEnemySO : ScriptableObject
{
    public List<EnemyDetailsSO> enemyList;
}
