using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchVector : MonoBehaviour
{
    // State variables to manage input events
    private bool shiftingToGaleMode = false;

    // Reference to game objects
    private GameObject wizardObject;

	// Use this for initialization
	void Awake()
    {
        wizardObject = GameObject.Find("Wizard Object");
        if (!wizardObject)
        {
            Debug.LogError("Wizard object not found", wizardObject);
        }
	}
	
	// Update is called once per frame
	void Update()
    {
        if (wizardObject.GetComponent<Wizard>().CurMode == Wizard.Mode.STORM)
        {
            // Display the launch vector sprite if not shifting back to gale mode
            if (!shiftingToGaleMode && wizardObject.GetComponent<StormMode>().CurAction == StormMode.Action.DEFAULT)
            {
                shiftingToGaleMode = Input.GetButtonDown("Change Mode");
            }
            GetComponent<SpriteRenderer>().enabled = !shiftingToGaleMode;

            // Position the launch vector sprite
            Vector3 spritePosition = wizardObject.transform.position;
            spritePosition.y = transform.position.y;
            spritePosition += wizardObject.transform.forward.normalized * 4;
            transform.position = spritePosition;

            // Rotate the launch vector sprite to look in the direction the wizard is facing
            transform.rotation = Quaternion.LookRotation(Vector3.up, wizardObject.transform.forward);
        }
        else
        {
            shiftingToGaleMode = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
