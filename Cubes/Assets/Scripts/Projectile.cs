using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public LayerMask collisionMask; //слои которые видит пуля
    float speed = 10f; //Скорость пули
    float damage = 23; //урон пули

    float lifeTime = 3;
    float skinWidth = .1f; //устранения бага двойного урона в близи при приближении врага

    private void Start()
    {
        Destroy(gameObject,lifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position,.1f,collisionMask); //регистрация коллайдеров вблизи спавна пули
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0]);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

	void Update () {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance); //движение пули вперед
	}

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position,transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit,moveDistance + skinWidth,collisionMask,QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
            damageableObject.TakeHit(damage,hit);
        GameObject.Destroy(gameObject);
    }

    void OnHitObject(Collider c)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
            damageableObject.TakeDamage(damage);
        GameObject.Destroy(gameObject);
    }
}
