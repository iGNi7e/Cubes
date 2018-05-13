using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    NavMeshAgent pathFinder; //экземпляр навмеша для перемещения енеми
    Transform target; // трансформ плеера, для задания конечной точки перемещения

    protected override void Start () {
        base.Start();

        pathFinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(UpdatePath());
	}
	
	void Update () {
	}

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f; //задержка перед обновлением координаты цели

        while(target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x,0,target.position.z); //определение координаты
            if(!dead)
                pathFinder.SetDestination(targetPosition);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
