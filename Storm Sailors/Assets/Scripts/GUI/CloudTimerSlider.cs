using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudTimerSlider : MonoBehaviour
{
    private GameObject wizardObject;
    float dipVal;

	// Use this for initialization
	void Start()
    {
        // Set up reference to wizard
        wizardObject = GameObject.Find("Wizard Object");
        if (wizardObject == null)
        {
            Debug.LogError("Wizard Object object not found", wizardObject);
        }

        // Intialize dip value
        GameObject cloudManager = GameObject.Find("Cloud Manager");
        if (cloudManager == null)
        {
            Debug.LogError("Cloud Manager object not found", cloudManager);
            dipVal = 0;
        }
        else
        {
            dipVal = cloudManager.GetComponent<CloudManager>().dipVal;
        }
	}
	
	// Update is called once per frame
	void Update()
    {
        // Position Cloud Timer UI at wizard transform
        Vector3 wizardPosition = wizardObject.transform.position;
        wizardPosition.y -= dipVal;
        transform.position = Camera.main.WorldToScreenPoint(wizardPosition);
	}
}
