﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : MonoBehaviour {

    public float moveSpeed = 5f; //скорость плеера

    Camera viewCamera; //экземпляр камеры
    PlayerController controller; //экземпляр контроллера
    public VirtualJoystick virtualJoystick; //экземпляр скрипта управления для телефона
    GunController gunController; //экземпляр GunController

    void Start () {
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
        gunController = GetComponent<GunController>();
    }
	
	void Update () {
        //Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        //Movement Input
        Vector3 moveInput = new Vector3(virtualJoystick.Horizontal(),0,virtualJoystick.Vertical()); //принимаем параметры для определения направления движения
        Vector3 moveVelocity = moveInput.normalized * moveSpeed; //преобразование в конечный вектор движения плеера
        controller.Move(moveVelocity);

        //Определение направления поворота плеера в нужную сторону
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up,Vector3.zero);
        float rayDistance;

        if(groundPlane.Raycast(ray,out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            controller.LookAt(point);
        }

        //Weapon Input
        if (Input.GetMouseButtonDown(0))
        {
            gunController.Shoot();
        }
	}
}
