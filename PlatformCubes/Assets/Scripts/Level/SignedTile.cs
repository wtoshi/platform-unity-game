using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignedTile : MonoBehaviour
{
    public GameObject m_ParentObject;
    public Transform m_Target;
    [SerializeField] Transform m_rayPoint;

    Material m_material;

    bool canSendCube = true;
    public bool CanSendCube { get { return canSendCube; } }

    [SerializeField] float m_distanceToObstacle;

    public enum Direction
    {
        ToLeft, ToRight, ToUp, ToBottom
    }

    private void Awake()
    {
        m_material = GetComponent<Renderer>().material;
    }

    public void Set(Direction _direction)
    {
        float angle = 0;

        switch (_direction)
        {
            case Direction.ToLeft:
                angle = -90;
                break;
            case Direction.ToRight:
                angle = 90;
                break;
            case Direction.ToUp:
                angle = 0;
                break;
            case Direction.ToBottom:
                angle = 180;
                break;
        }

        m_ParentObject.transform.rotation = Quaternion.Euler(0, angle, 0);

        
    }

    public void SetState(bool _state)
    {
        canSendCube = _state;

        if (_state)
            m_material.color = Color.green;
        else
            m_material.color = Color.red;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction = m_Target.forward * 50;
        Gizmos.DrawRay(m_rayPoint.position, direction);
    }
#endif

    public List<bool> CheckForEnd(int[] _lenghtList)
    {
        List<bool> _replyList = new List<bool>();

        for (int i = 0; i < _lenghtList.Length; i++)
        {
            _replyList.Add(doCheckForEnd(_lenghtList[i]));
        }

        return _replyList;
    }

    bool doCheckForEnd(float _lenght)
    {
        RaycastHit hit;

        if (Physics.Raycast(m_rayPoint.position, m_Target.forward * 50f, out hit, 100f, LayerMask.GetMask("Obstacle")))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {

                m_distanceToObstacle = Vector3.Distance(m_Target.position, hit.point);

                if (m_distanceToObstacle + 0.1f < _lenght)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
        }

        return true;
    }

    void CheckState()     
    {
        RaycastHit hit;

        if (Physics.Raycast(m_rayPoint.position, m_Target.forward * 50f , out hit, 100f, LayerMask.GetMask("Obstacle")))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {

                m_distanceToObstacle = Vector3.Distance(m_Target.position, hit.point);

                if (m_distanceToObstacle +0.1f < GameManager.instance.CubeLenght)
                {
                    SetState(false);
                }
                else
                {
                    SetState(true);
                }

            }
            else
            {
                SetState(true);
            }
        }
        else
        {
            SetState(true);
        }
    }

    private void OnEnable()
    {
        GameManager.instance.onCubeLenghtChanged += CheckState;
        GameManager.instance.onGameStateStarted += CheckState;
        GameManager.instance.onCubeMoved += CheckState;
    }

    private void OnDisable()
    {
        GameManager.instance.onCubeLenghtChanged -= CheckState;
        GameManager.instance.onGameStateStarted -= CheckState;
        GameManager.instance.onCubeMoved -= CheckState;
    }
}
