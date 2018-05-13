using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    public Transform weaponHold; //Спавн оружия на префабе плеера
    public Gun startingGun; //оружие которое дается при старте
    Gun equippedGun; //оружие которое находится у плеера в данный момент

    private void Start()
    {
        if(startingGun != null) //на старте присутствует оружие
        {
            EquipGun(startingGun);
        }
    }

    public void EquipGun(Gun gunToEquip) //спавн оружия у плеера
    {
        if (equippedGun != null) //если у плеера уже есть какое-то оружие, то разрушить его
            Destroy(equippedGun.gameObject);
        equippedGun = Instantiate(gunToEquip,weaponHold.position,weaponHold.rotation) as Gun; //спавн оружия
        equippedGun.transform.parent = weaponHold; //назначение оружия чаилдом
    }

    public void Shoot() //Вызов метода Shoot() при нажатии выстрела
    {
        if (equippedGun != null)
            equippedGun.Shoot();
    }

}
