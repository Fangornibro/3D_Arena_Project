using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    [Header("Initialisation")]
    [SerializeField] private Player player;
    [SerializeField] private Joystick joystick;
    [SerializeField] private float sensX, sensY;
    private float xRotation, yRotation, mouseX, mouseY;

    void Update()
    {
        transform.position = player.head.position;

        mouseX = joystick.Horizontal * Time.deltaTime * sensX;

        mouseY = joystick.Vertical * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -60f, 60f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        player.head.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
