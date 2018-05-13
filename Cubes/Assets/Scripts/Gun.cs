using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public Transform muzzle; //префаб ProjectileSpawn(место спавна пули)
    public Projectile projectile; //префаб пули
    public float msBetweenShots = 100f; //мс между спавном пуль
    public float muzzleVelocity = 35f; //скорость пули

    float nextShotTime; //переменная для определения возможности следующего выстрела

    public void Shoot()
    {
        if(Time.time > nextShotTime) //если условие следуюзего выстрела удовлетворяет, то производим выстрел
        {
            nextShotTime = Time.time + msBetweenShots/1000;
            Projectile newProjectile = Instantiate(projectile,muzzle.position,muzzle.rotation) as Projectile; //создание пули
            newProjectile.SetSpeed(muzzleVelocity); //установка скорости пули
        }

    }

}
