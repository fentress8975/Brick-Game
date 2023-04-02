using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlsLocal : MonoBehaviour
{
    [SerializeField] private PlayerClientBar m_Bar;

    private Direction m_Direction = Direction.None;

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        var oldDirection = m_Direction;

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                m_Direction = Direction.None;
            }
            else
            {
                m_Direction = Direction.Left;
            }
        }
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            m_Direction = Direction.Right;
        }
        else
        {
            m_Direction = Direction.None;
        }
        if (oldDirection != m_Direction)
        {
            m_Bar.ChangeDirectionLocal(m_Direction);
        }
    }
}
