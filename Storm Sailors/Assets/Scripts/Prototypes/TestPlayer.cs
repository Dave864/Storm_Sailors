using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    // References to components of player
    private GameObject railCenter;
    private GameObject wizard;
    private GameObject ship;

    // Movement parameters
    private readonly int defaultSpeed = 500;        // The default speed of the ship
    private readonly float defaultRotRate = 0.1f;   // The default time in sec of a change in position
    private int baseSpeed;
    private float rotRate;
    private Vector2 curPosVect;                     // The current position of the wizard
    private Vector2 desPosVect;                     // The desired position of the wizard
    private Vector2 strtPosVect;                    // The starting position of a move
    private Vector2 posVectVel;                     // The velocity of a changing position

    // Keeps track of time spent repositioning
    private float posTime = 0;

    // Use this for initialization
    void Start()
    {
        // Establish the references to the components
        railCenter = gameObject.transform.Find("Cloud Rail/Rail Center").gameObject;
        wizard = gameObject.transform.Find("Cloud Rail/Rail Center/Wizard").gameObject;
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
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        Vector2 posVect = new Vector2(Mathf.Round(hInput), Mathf.Round(vInput)).normalized;

        // Set up the start and end positions
        if (posVect != new Vector2(0, 0))
        {
            desPosVect = posVect;
            strtPosVect = curPosVect;
        }

        // Move the player components
        if (desPosVect != curPosVect)
        {
            // Reposition Wizard
            if (wizard != null)
            {

            }
            if (railCenter != null)
            {

            }

            // Rotate Ship
            if (ship != null)
            {

            }

            // Update curPosVect
            //curPosVect = Vector2.SmoothDamp(curPosVect, desPosVect, ref posVectVel, rotRate, Mathf.Infinity, Time.deltaTime);

            // Move Player
        }
    }
}
