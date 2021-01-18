using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigRotator : MonoBehaviour
{
    [SerializeField] private float m_DefaultxAngle, m_DefaultyAngle;
    private float m_HorizontalInputValue;
    private float m_VerticalInputValue;
    private float m_RotationSpeed = 5.0f;
    [SerializeField] private bool m_WorldSpace; //should the rotation be changed in world space, or relative to the object's parent?

    // Update is called once per frame
    void Update()
    {
        m_HorizontalInputValue = Input.GetAxis("CameraHorizontal");
        m_VerticalInputValue = Input.GetAxis("CameraVertical");
        Quaternion target = Quaternion.Euler(m_DefaultxAngle + m_VerticalInputValue * 45.0f, m_DefaultyAngle + m_HorizontalInputValue * 180.0f, 0);
        if (!m_WorldSpace) {
            target = transform.parent.rotation * target;
        }
    
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * m_RotationSpeed);
    }
}