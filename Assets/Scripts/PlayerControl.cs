using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public enum ControlMethod { MouseAndKeyboard, MouseOnly, KeyboardOnly };
    public ControlMethod controlMethod;

    PlayerMovement movementComponent;
    PlayerShoot shootComponent;

    float vertical;     // Used for up/down movement
    float horizontal;   // Used for left/right movement
    bool shoot;     // Used for shooting
    bool boost;     // Used for boosting

    void Start()
    {
        movementComponent = GetComponent<PlayerMovement>();
        shootComponent = GetComponent<PlayerShoot>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement axis
        SetMovementAxisDigital();
        movementComponent.SetDirection(horizontal, vertical);

        // Boost
        if (GetBoostKey())
        {
            movementComponent.Boost();
        }

        // Aim
        bool relativeAim = true;
        if (controlMethod == ControlMethod.MouseAndKeyboard || controlMethod == ControlMethod.MouseOnly)
        {
            relativeAim = false;
        }

        shootComponent.Aim(Quaternion.Euler(GetAim()), relativeAim);

        // Shoot
        if (GetShootKey())
        {
            shootComponent.Shoot();
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
        }

        return Input.GetKey(boostKey);
    }

    bool GetShootKey()
    {
        KeyCode shootKey = KeyCode.None;

        switch (controlMethod)
        {
            case ControlMethod.MouseAndKeyboard:
            case ControlMethod.MouseOnly:
                shootKey = KeyCode.Mouse0;
                break;
            case ControlMethod.KeyboardOnly:
                shootKey = KeyCode.Space;
                break;
        }

        return Input.GetKey(shootKey);
    }

    Quaternion LookMouseDirection()
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

        return result;
    }

    Vector3 GetAim()
    {
        Vector3 direction = Vector2.zero;

        switch (controlMethod)
        {
            case ControlMethod.MouseAndKeyboard:
            case ControlMethod.MouseOnly:
                return LookMouseDirection().eulerAngles;
            case ControlMethod.KeyboardOnly:
                direction.x = GetDigitalAxisDirection(Input.GetKey(KeyCode.None), Input.GetKey(KeyCode.UpArrow));
                direction.y = GetDigitalAxisDirection(Input.GetKey(KeyCode.RightArrow), Input.GetKey(KeyCode.LeftArrow));

                direction *= 45;
                break;
        }

        return direction;
    }
}
