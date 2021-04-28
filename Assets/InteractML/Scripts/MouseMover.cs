using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple script that will move an object with the mouse
/// </summary>
public class MouseMover : MonoBehaviour {

    private GameObject m_ObjToMove;
    private Vector3 m_CurrentMouseWorldPos;

    public bool MoveAxisZ;
    public bool UseCurrentZOnStart;

	// Use this for initialization
	void Start () {
        m_ObjToMove = this.gameObject;

        if (!UseCurrentZOnStart)
        {
            // Init depth of movement
            m_CurrentMouseWorldPos.z = -1f;
        }
        else
        {
            m_CurrentMouseWorldPos.z = this.transform.position.z;
        }

    }
	
	// Update is called once per frame
	void Update () {
        // Mouse x,y moves obj in 2D
        Vector2 mousePos2 = Mouse.current.position.ReadValue();
        Vector3 mousePos = new Vector3(mousePos2.x, mousePos2.y, 10);
        Vector3 newMousePosWorldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        newMousePosWorldPoint.z = 1;
        
        m_CurrentMouseWorldPos.x = newMousePosWorldPoint.x;
        m_CurrentMouseWorldPos.y = newMousePosWorldPoint.y;

        if (MoveAxisZ)
        {
            // Mouse wheel moves obj in Z axis
            m_CurrentMouseWorldPos.z += Input.mouseScrollDelta.y;
        }

        // Apply directly to position
        m_ObjToMove.transform.position = m_CurrentMouseWorldPos;

    }
}
