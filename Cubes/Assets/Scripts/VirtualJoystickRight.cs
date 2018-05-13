using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystickRight : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image backgroundImage; //префаб основного круга
    private Image joystickImage; //префаб движущуйся точки в круге
    private Vector3 inputVector; //вектор определяющий направление поворота
    GunController gunController; //экземпляр GunController

    private void Start()
    {
        backgroundImage = GetComponent<Image>();
        joystickImage = transform.GetChild(0).GetComponent<Image>();
        gunController = GameObject.FindGameObjectWithTag("Player").GetComponent<GunController>();
    }

    public virtual void OnDrag(PointerEventData ped) //метод определяющий поворот плеера
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundImage.rectTransform,
            ped.position,
            ped.pressEventCamera,
            out pos))
        {
            pos.x = (pos.x / backgroundImage.rectTransform.sizeDelta.x);
            pos.y = (pos.y / backgroundImage.rectTransform.sizeDelta.y);
            inputVector = new Vector3(pos.x * 2 + 1,0,pos.y * 2 - 1);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            //передвижение точки на джойстике
            joystickImage.rectTransform.anchoredPosition = new Vector3(
                inputVector.x * (backgroundImage.rectTransform.sizeDelta.x / 3),
                inputVector.z * (backgroundImage.rectTransform.sizeDelta.y / 3));
        }
    }

    public virtual void OnPointerDown(PointerEventData ped) //метод срабатывающий по нажанию в джойстике
    {
        OnDrag(ped);
        StartCoroutine(Shoot());
    }

    public virtual void OnPointerUp(PointerEventData ped) //метод срабатывающий по отжатию в джойстике
    {
        //inputVector = Vector3.zero;
        joystickImage.rectTransform.anchoredPosition = Vector3.zero;
    }

    //конечные определения векторов, при нажатии с джойстика или с клавиатуры
    public float Horizontal()
    {
        return inputVector.x;
    }

    public float Vertical()
    {
        return inputVector.z;
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.01f);
        gunController.Shoot();
    }
}
