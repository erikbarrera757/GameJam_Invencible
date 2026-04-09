using UnityEngine;
using UnityEngine.InputSystem; // Importante para el nuevo sistema
using Unity.Cinemachine;

public class CameraMouseControl : MonoBehaviour
{
    private CinemachineInputAxisController axisController;

    void Start()
    {
        axisController = GetComponent<CinemachineInputAxisController>();
    }

    void Update()
    {
        // Detectamos el click derecho usando el nuevo sistema
        bool isRightClick = Mouse.current.rightButton.isPressed;

        // Encendemos o apagamos el controlador de ejes
        if (axisController != null)
        {
            axisController.enabled = isRightClick;
        }

        // Bloqueo del cursor
        if (isRightClick)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}