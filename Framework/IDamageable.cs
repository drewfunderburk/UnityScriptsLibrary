using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implemented by any class that needs to be able to take damage. All damaging classes should target this interface
/// </summary>
public interface IDamageable
{
    public void TakeDamage(float damage);
}