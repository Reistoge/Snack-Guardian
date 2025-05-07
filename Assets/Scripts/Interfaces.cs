using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void takeDamage(float damage);
}
public interface IAttackable
{
    void attack(float damage);
}

public interface IBreakable
{
    void breakObject();
}
public interface IHealable{
    void heal(float healAmount);

}

 