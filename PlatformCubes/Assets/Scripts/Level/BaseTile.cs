using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : MonoBehaviour
{
    [SerializeField]    Transform m_rayPoint;

    public bool CheckSlot()
    {
        RaycastHit hit;

        if (Physics.Raycast(m_rayPoint.position, m_rayPoint.forward,out hit,10f))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }

        return false;
    }

    
}
