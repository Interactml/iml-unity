﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script that will move an object with the mouse
/// </summary>
public class MouseMover : MonoBehaviour {

    private GameObject m_ObjToMove;
    private Vector3 m_CurrentMouseWorldPos;

	// Use this for initialization
	void Start () {
        m_ObjToMove = this.gameObject;

        // Init depth of movement
        m_CurrentMouseWorldPos.z = 1f;


    }
	
	// Update is called once per frame
	void Update () {
        // Mouse x,y moves obj in 2D
        Vector3 newMousePosWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10f);
        m_CurrentMouseWorldPos.x = newMousePosWorldPoint.x;
        m_CurrentMouseWorldPos.y = newMousePosWorldPoint.y;

        // Mouse wheel moves obj in Z axis
        m_CurrentMouseWorldPos.z += Input.mouseScrollDelta.y;

        // Apply directly to position
        m_ObjToMove.transform.position = m_CurrentMouseWorldPos;

    }
}