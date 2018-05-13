using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {

    public float moveSpeed = 5f; //скорость плеера

    Camera viewCamera; //экземпляр камеры
    PlayerController controller; //экземпляр контроллера
    public VirtualJoystickLeft virtualJoystick; //экземпляр скрипта управления для телефона
    public VirtualJoystickRight virtualJoystickRight; //экземпляр скрипта поворота персонажа для телефона
    GunController gunController; //экземпляр GunController

    protected override void Start () {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }
	
	void Update () {
        //Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        //Movement Input
        Vector3 moveInput = new Vector3(virtualJoystick.Horizontal(),0,virtualJoystick.Vertical()); //принимаем параметры для определения направления движения
        Vector3 moveVelocity = moveInput.normalized * moveSpeed; //преобразование в конечный вектор движения плеера
        controller.Move(moveVelocity);

        Vector3 rotateInput = new Vector3(virtualJoystickRight.Horizontal() + transform.position.x,0,virtualJoystickRight.Vertical() + transform.position.z); //принимаем параметры для определения направления поворота
        controller.LookAt(rotateInput);

        #region Mouse Inpit (Закомментировать для билда под телефон)

        //Определение направления поворота плеера в нужную сторону
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up,Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray,out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            controller.LookAt(point);
        }

        //Weapon Input
        if (Input.GetMouseButtonDown(0))
        {
            gunController.Shoot();
        }
        #endregion
    }
}
