using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Gameplay.Component;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public bool isAlive;

    public EnemyData(Enemy enemy)
    {
        if (enemy.TryGetComponent(out HealthComponent healthComponent))
            isAlive = healthComponent.CurrentHealth > 0;
    }
}
