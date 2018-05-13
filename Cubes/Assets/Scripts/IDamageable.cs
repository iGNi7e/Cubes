using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable { // интерфейс для упрощения получения урона

    void TakeHit(float damage,RaycastHit hit);

}
