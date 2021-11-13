using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCube : MonoBehaviour
{
    bool canMove = false;
    BoxCollider myCollider;
    GameObject myChildCube;
    Rigidbody myRb;
    Rigidbody myChildRB;

    float m_cubeSpeed;
    float m_cubeLenght;

    // Actions
    public delegate void CubeState();
    public event CubeState onCubeStopped;


    private void FixedUpdate()
    {
        if (canMove && myChildCube)
        {
            // Move Cube forward

            //transform.localPosition += transform.forward * m_cubeSpeed * Time.deltaTime;
            myRb.MovePosition(transform.position+transform.forward * m_cubeSpeed * Time.fixedDeltaTime);
            //myRb.position += transform.forward * m_cubeSpeed * Time.fixedDeltaTime;

        }
    }

    private void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
        myRb = GetComponent<Rigidbody>();
    }

    public void Set(SignedTile _clickedTile, bool _prepare)
    {
        if (_prepare)
        {
            m_cubeSpeed = 10f;
            m_cubeLenght = Random.Range(2,3);
        }      
        else
        {
            m_cubeSpeed = GameManager.instance.CubeSpeed;
            m_cubeLenght = GameManager.instance.CubeLenght;
        }   
            
        
        AddCube();

        // Set scale & localPosition
        transform.localPosition = _clickedTile.m_Target.position;

        // Set direction accordingly to target direction on signedTile
        transform.LookAt(transform.position + _clickedTile.m_Target.forward);
        
        canMove = true;
    }

    void AddCube()
    {
        myChildCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        myChildCube.transform.SetParent(transform);

        // Set Collider as trigger
        myChildCube.GetComponent<BoxCollider>().isTrigger = false;
        myChildCube.GetComponent<BoxCollider>().enabled = false;

        // Set Child's Rigidbody
        myChildRB = myChildCube.AddComponent<Rigidbody>();
        myChildRB.constraints = RigidbodyConstraints.FreezeAll;
        myChildRB.useGravity = false;
        myChildRB.isKinematic = false;

        // Add Tag & Layer
        myChildCube.tag = "Obstacle";
        myChildCube.layer = 7;

        // Set position and localscale
        myChildCube.transform.localPosition = myChildCube.transform.localPosition - new Vector3(0f,0f, m_cubeLenght / 2);
        
        myChildCube.transform.localScale = new Vector3(LevelController.instance.m_TileSize, LevelController.instance.m_TileSize, m_cubeLenght);
        
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("Obstacle") && other.gameObject != myChildCube)
        {
            StopCube(true);
        }
        if (other.gameObject.CompareTag("LevelBorders"))
        {
            Debug.Log("Level Borders");
            StopCube(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.CompareTag("LevelBase"))
        {
            StopCube(true);
        }
    }
    */

    void StopCube(bool _adjust = false)
    {
        canMove = false;
        myCollider.enabled = false;

        // Enable child's collider
        myChildCube.GetComponent<BoxCollider>().enabled = true;

        // Lock Rigidbody
        myRb.velocity = Vector3.zero;
        myRb.angularVelocity = Vector3.zero;
        
        myRb.constraints = RigidbodyConstraints.FreezeAll;

        // To Adjust position
        if (_adjust)
        {
            Vector3 newPos = new Vector3();
            newPos.x = Mathf.Round(transform.position.x * 10f) / 10f;
            newPos.y = transform.position.y;
            newPos.z = Mathf.Round(transform.position.z * 10f) / 10f;
            transform.localPosition = newPos;
        }

        onCubeStopped?.Invoke();
    }

}
