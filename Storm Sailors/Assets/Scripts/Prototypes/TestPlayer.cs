﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    // State variables for keeping track of what is happening
    private bool isPositioning = false;

    // References to components of player
    private GameObject railCenter;
    private GameObject wizard;
    private GameObject ship;

    // Movement parameters
    private readonly int defaultSpeed = 500;        // The default base speed of the ship
    private readonly float defaultRotRate = 0.1f;   // The default time it takes to change position in sec
    private int baseSpeed;                          // The used base speed of the ship
    private float rotRate;                          // The used time it takes to change position in sec
    private Vector2 curPosVect;                     // The current position of the wizard
    private Vector2 desPosVect;                     // The desired position of the wizard
    private Vector2 strtPosVect;                    // The starting position of a move
    private Vector2 posVectVel;                     // The velocity of a changing position

    // Use this for initialization
    void Start()
    {
        // Establish the references to the components
        railCenter = gameObject.transform.Find("Cloud Rail/Rail Center").gameObject;
        wizard = gameObject.transform.Find("Cloud Rail/Rail Center/Wizard Entity").gameObject;
        ship = gameObject.transform.Find("Ship").gameObject;

        // Initialize the movement parameters
        baseSpeed = (ship == null) ? defaultSpeed : ship.GetComponent<TestShip>().baseSpeed;
        rotRate = (railCenter == null) ? defaultRotRate : railCenter.GetComponent<TestCloudRail>().rotRate;
        curPosVect = new Vector2(wizard.transform.position.x, wizard.transform.position.z).normalized;
        desPosVect = new Vector2(curPosVect.x, curPosVect.y);
        strtPosVect = new Vector2(curPosVect.x, curPosVect.y);
        posVectVel = new Vector2(0, 0);
    }
	
	// Update is called once per frame
	void Update()
    {
        // Construct position vector from input
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        Vector2 posVect = new Vector2(Mathf.Round(hInput), Mathf.Round(vInput));

        // Set up the start and end positions if not already repositioning
        if (!isPositioning && posVect != new Vector2(0, 0))
        {
            desPosVect = new Vector2(posVect.x, posVect.y);
            strtPosVect = curPosVect;
            isPositioning = true;
        }

        // Reposition the player components
        if (isPositioning)
        {
            StartCoroutine(Position(strtPosVect, desPosVect));
        }
    }

    IEnumerator Position(Vector2 strtPos, Vector2 endPos)
    {
        // Set up the start and end rotations
        float strtAngle = railCenter.GetComponent<TestCloudRail>().PositionAngle(strtPos);
        float endAngle = railCenter.GetComponent<TestCloudRail>().PositionAngle(endPos);
        Quaternion strtRot = railCenter.transform.rotation;
        Quaternion endRot = Quaternion.Euler(new Vector3(0, endAngle, 0));

        for (float posTime = 0; posTime < rotRate; posTime += Time.deltaTime)
        {
            // Rotate the Wizard to face the center of the rail
            if (wizard != null)
            {

            }
            // Rotate the center of the rail to position the wizard
            if (railCenter != null)
            {
                railCenter.transform.rotation = Quaternion.Slerp(strtRot, endRot, posTime / rotRate);
            }
            yield return null;
        }
        // update the current position
        curPosVect = endPos;

        // Reset timer and state flags
        isPositioning = false;
        yield return null;
    }
}
