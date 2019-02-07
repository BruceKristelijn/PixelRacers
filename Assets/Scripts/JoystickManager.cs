using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickManager : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

    public VirtualJoystick virtualJoystick;
    public GameObject joystick;
    private Vector2 joystickLocation;

    public virtual void OnPointerDown(PointerEventData ped) {
        joystickLocation = new Vector2(ped.position.x - 150, ped.position.y - 150);
        joystick.transform.position = joystickLocation;
        joystick.SetActive(true);

    }

    public virtual void OnPointerUp(PointerEventData ped) {
        joystick.SetActive(false);
    }
}
