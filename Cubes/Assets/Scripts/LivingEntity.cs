using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable {

    public float startingHealth; //стартовое количество хп у объекта
    protected float health; //текущее количество хп у объекта
    protected bool dead;

    public event System.Action OnDeath; //событие которое вызывается при смерти

    protected virtual void Start()
    {
        health = startingHealth; //оперделение хп при старте игры
    }

    public void TakeHit(float damage, RaycastHit hit) //переопределение метода уменьшения хп при попадании
    {

        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    protected void Die() //вызов события смерти и разрушение объекта
    {
        dead = true;
        if (OnDeath != null)
            OnDeath();
        GameObject.Destroy(gameObject);
    }

}
