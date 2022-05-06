using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public enum ControlMethod { MouseAndKeyboard, MouseOnly, KeyboardOnly, PS4 };
    public ControlMethod controlMethod;
    public int controllerPort;

    private enum JoystickAxis { LeftStickHorizontal, LeftStickVertical, RightStickHorizontal, RightStickVertical };

    PlayerMovement movementComponent;
    PlayerShoot shootComponent;

    float vertical;     // Used for up/down movement
    float horizontal;   // Used for left/right movement

    void Start()
    {
        movementComponent = GetComponent<PlayerMovement>();
        shootComponent = GetComponent<PlayerShoot>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement axis
        if (controlMethod != ControlMethod.PS4)
        {
            SetMovementAxisDigital();
        }
        else
        {
            SetMovementAxisAnalog();
        }
        movementComponent.SetDirection(horizontal, vertical);

        // Boost
        if (GetBoostKey())
        {
            //movementComponent.Boost();
        }

        // Aim
        shootComponent.Aim(GetAim());

        // Shoot
        if (GetShootKey())
        {
            shootComponent.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Tab) && controlMethod == ControlMethod.MouseAndKeyboard)
        {
            GameManager.instance.ToggleMouseLock();
        }
    }

    void SetMovementAxisDigital()
    {
        KeyCode left = KeyCode.None;
        KeyCode right = KeyCode.None;
        KeyCode up = KeyCode.None;
        KeyCode down = KeyCode.None;

        // Set keybinds
        switch (controlMethod)
        {
            case ControlMethod.MouseAndKeyboard:
            case ControlMethod.KeyboardOnly:
                left = KeyCode.A;
                right = KeyCode.D;
                up = KeyCode.W;
                down = KeyCode.S;
                break;
            case ControlMethod.MouseOnly:
                left = KeyCode.O;
                right = KeyCode.P;
                up = KeyCode.Mouse1;
                break;
        }
        
        horizontal = Mathf.MoveTowards(horizontal, GetDigitalAxisDirection(right, left), Time.deltaTime * 9);
        vertical = Mathf.MoveTowards(vertical, GetDigitalAxisDirection(up, down), Time.deltaTime * 9);

        horizontal = Mathf.Clamp(horizontal, -1, 1);
        vertical = Mathf.Clamp(vertical, -1, 1);
    }

    int GetDigitalAxisDirection(bool positive, bool negative)
    {
        if (positive && !negative)
        {
            return 1;
        }
        else if (negative && !positive)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
    int GetDigitalAxisDirection(KeyCode positive, KeyCode negative)
    {
        return GetDigitalAxisDirection(Input.GetKey(positive), Input.GetKey(negative));
    }

    void SetMovementAxisAnalog()
    {
        Vector2 axis = Vector2.zero;

        switch (controlMethod)
        {
            case ControlMethod.PS4:
                axis.x = GetJoystickAxis(JoystickAxis.LeftStickHorizontal);
                axis.y = GetJoystickAxis(JoystickAxis.LeftStickVertical);
                break;
        }

        horizontal = axis.x;
        vertical = axis.y;

        horizontal = Mathf.Clamp(horizontal, -1, 1);
        vertical = Mathf.Clamp(vertical, -1, 1);
    }

    bool GetBoostKey()
    {
        KeyCode boostKey = KeyCode.None;

        switch (controlMethod)
        {
            case ControlMethod.MouseAndKeyboard:
            case ControlMethod.KeyboardOnly:
                boostKey = KeyCode.LeftShift;
                break;
            case ControlMethod.MouseOnly:
                boostKey = KeyCode.Mouse2;
                break;
            case ControlMethod.PS4:
                boostKey = GetJoystickButton(KeyCode.JoystickButton0);
                break;
        }

        return Input.GetKey(boostKey);
    }

    bool GetShootKey()
    {
        KeyCode shootKey = KeyCode.None;
        KeyCode altShootKey = KeyCode.None;

        switch (controlMethod)
        {
            case ControlMethod.MouseAndKeyboard:
            case ControlMethod.MouseOnly:
                shootKey = KeyCode.Mouse0;
                break;
            case ControlMethod.KeyboardOnly:
                shootKey = KeyCode.Space;
                break;
            case ControlMethod.PS4:
                shootKey = GetJoystickButton(KeyCode.JoystickButton1);
                altShootKey = GetJoystickButton(KeyCode.JoystickButton7);
                break;
        }

        return Input.GetKey(shootKey) || Input.GetKey(altShootKey);
    }

    Vector3 LookMouseDirection()
    {
        Quaternion result = Quaternion.identity;

        // Get the mouse position
        Ray mouseWorldRay = shootComponent.cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseWorldRay, out RaycastHit hit))
        {
            // If the mouse hits anything, look towards the mouse
            Vector3 direction = hit.point - transform.position;
            result = Quaternion.LookRotation(direction);
        }

        Vector3 eulers = result.eulerAngles;

        return eulers;
    }

    Vector3 GetAim()
    {
        Vector3 direction = Vector2.zero;

        switch (controlMethod)
        {
            case ControlMethod.MouseAndKeyboard:
            case ControlMethod.MouseOnly:
                direction.x = -Input.GetAxis("Mouse Y");
                direction.y = Input.GetAxis("Mouse X");
                break;
            case ControlMethod.KeyboardOnly:
                direction.x = GetDigitalAxisDirection(Input.GetKey(KeyCode.None), Input.GetKey(KeyCode.UpArrow));
                direction.y = GetDigitalAxisDirection(Input.GetKey(KeyCode.RightArrow), Input.GetKey(KeyCode.LeftArrow));

                //direction *= 45;
                break;
            case ControlMethod.PS4:
                direction.x = -GetJoystickAxis(JoystickAxis.RightStickVertical);
                direction.y = -GetJoystickAxis(JoystickAxis.RightStickHorizontal);

                direction /= 16;

                //direction.x = Mathf.Clamp(direction.x, -45, 0);
                break;
        }

        return direction;
    }

    KeyCode GetJoystickButton(KeyCode button)
    {
        if (controllerPort == 0)
        {
            return KeyCode.None;
        }

        int keyCodeID = (int)button;
        return (KeyCode)keyCodeID + (20 * controllerPort);
    }

    float GetJoystickAxis(JoystickAxis axis)
    {
        if (controllerPort == 0)
        {
            return 0;
        }

        string inputAxis = string.Empty;

        switch (axis)
        {
            case JoystickAxis.LeftStickHorizontal:
                inputAxis = "LeftStickHorizontal";
                break;
            case JoystickAxis.LeftStickVertical:
                inputAxis = "LeftStickVertical";
                break;
            case JoystickAxis.RightStickHorizontal:
                inputAxis = "RightStickHorizontal";
                break;
            case JoystickAxis.RightStickVertical:
                inputAxis = "RightStickVertical";
                break;
        }

        return Input.GetAxis(inputAxis + controllerPort);
    }
}
