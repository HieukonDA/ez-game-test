using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyLevelConfig", menuName = "ScriptableObjects/EnemyLevelConfig", order = 1)]
public class EnemyLevelConfig : ScriptableObject
{
    public int levelNumber;
    public float maxHealth;
    public int attackDamage;
    public float movementSpeed;
    public float attackCooldown;
    public int enemyCount;
}