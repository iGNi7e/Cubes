using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    Vector3 velocity;

    Rigidbody myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity) //принимаем вектор в направлении которого будет совершено движение
    {
        velocity = _velocity;
    }

    public void LookAt(Vector3 _point) //поворот плеера в нужную сторону
    {
        Vector3 correctPoint = new Vector3(_point.x,transform.position.y,_point.z); //корректировка поворота, чтобы плеер не наклонялся
        transform.LookAt(correctPoint);
    }

    public void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
    }
}
