using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    // Start area reference
    public GameObject startArea = null;

    // References to components of player
    private GameObject railCenter;
    private GameObject cloudManager;
    private GameObject wizard;
    private GameObject ship;

    // Movement parameters
    private readonly int defaultSpeed = 500;        // The default base speed of the ship
    private readonly float defaultRotRate = 0.1f;   // The default time it takes to change wizard position in sec
    private int baseSpeed;                          // The used base speed of the ship
    private float rotRate;                          // The used time it takes to change wizard position in sec

    // Use this for initialization
    void Start()
    {
        // Establish the references to the components
        railCenter = gameObject.transform.Find("Cloud Rail/Rail Center").gameObject;
        wizard = gameObject.transform.Find("Cloud Rail/Rail Center/Wizard Entity").gameObject;
        ship = gameObject.transform.Find("Ship").gameObject;
        cloudManager = gameObject.transform.Find("Cloud Rail/Cloud Manager").gameObject;

        // Initialize the movement parameters
        baseSpeed = (ship == null) ? defaultSpeed : ship.GetComponent<TestShip>().baseSpeed;
        rotRate = (railCenter == null) ? defaultRotRate : railCenter.GetComponent<TestCloudRail>().rotRate;

        // Set initial position of player
        transform.position = new Vector3(startArea.transform.position.x, transform.position.y, startArea.transform.position.z);
    }
	
	// Update is called once per frame
	void Update()
    {
        // Quit qame
        if (Input.GetButton("Cancel"))
        {
            Application.Quit();
        }

        // Move the player entity based off of the heading of the ship
        transform.position += ship.GetComponent<TestShip>().CurHeading * baseSpeed * Time.deltaTime;
    }
}
