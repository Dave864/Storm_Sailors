using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    // State variables
    private bool isPositioning = false;

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
    private Vector2 curWizardPos;                   // The current position of the wizard
    private Vector2 desWizardPos;                   // The desired position of the wizard

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
        curWizardPos = new Vector2(wizard.transform.position.x, wizard.transform.position.z).normalized;
        desWizardPos = new Vector2(curWizardPos.x, curWizardPos.y);

        // Set initial position of player
        transform.position = new Vector3(startArea.transform.position.x, transform.position.y, startArea.transform.position.z);
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
            desWizardPos = new Vector2(posVect.x, posVect.y);
            isPositioning = true;
        }

        // Reposition the player components
        if (isPositioning)
        {
            StartCoroutine(Position(desWizardPos));
        }

        // Move the player entity based off of the heading of the ship
        transform.position += ship.GetComponent<TestShip>().CurHeading * baseSpeed * Time.deltaTime;
    }

    // Repositions the wizard
    IEnumerator Position(Vector2 endPos)
    {
        // Set up the start and end rotations
        float endAngle = railCenter.GetComponent<TestCloudRail>().PositionAngle(endPos);
        Quaternion strtRot = railCenter.transform.rotation;
        Quaternion endRot = Quaternion.Euler(new Vector3(0, endAngle, 0));

        for (float posTime = 0; posTime < rotRate; posTime += Time.deltaTime)
        {
            // Rotate the center of the rail to position the wizard
            if (railCenter != null)
            {
                railCenter.transform.rotation = Quaternion.Slerp(strtRot, endRot, posTime / rotRate);
            }
            yield return null;
        }

        // Update the position of the wizard
        if (wizard != null)
        {
            wizard.GetComponent<TestWizard>().SetPosition(endPos);
        }

        // update the current wizard position
        curWizardPos = endPos;

        // Reset timer and state flags
        isPositioning = false;
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
        // Go back to start if ship hit an obstacle
        if (other.tag == "Obstacle")
        {
            cloudManager.GetComponent<TestCloudManager>().DispelAll();
            transform.position = new Vector3(startArea.transform.position.x, transform.position.y, startArea.transform.position.z);
        }
    }
}
