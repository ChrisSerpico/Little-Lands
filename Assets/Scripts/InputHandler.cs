using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
	// UI contorl panel that can be toggled with a keypress
	public Transform controlPanelObject; 
	// UI terrain control panel that can be toggled with a keypress 
	public Transform terrainPanelObject; 
	// UI map size control panel that can be toggled with a keypress
	public Transform mapSizePanelObject; 

	// Holds the main camera object, so that it can be manipulated by user input 
	public Transform cameraObject; 
	// Higher sensitivities make scrolling faster 
	public float scrollSensitivity= 1f; 
	// Camera is moved by dragging, but only while the left mouse button is held down, 
	// so this variable tracks whether the button is held (and thus we are in the dragging state) 
	private bool dragging = false; 
	// Higher sensitivites make dragging faster 
	public float dragSensitivity = 1f; 
	
	// Game object that holds all of the map data (array of map tiles) 
	public Transform mapObject; 
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// toggle activation of control panel 
		if (Input.GetKeyDown(KeyCode.Q)) {
			if (controlPanelObject.gameObject.activeSelf) {
				// panel is active, so deactivate it 
				controlPanelObject.gameObject.SetActive(false); 
			}
			else {
				// panel is not active, so activate it 
				controlPanelObject.gameObject.SetActive(true); 
			}
		}
		// toggle activation of terrain control panel 
		if (Input.GetKeyDown(KeyCode.F)) {
			if (terrainPanelObject.gameObject.activeSelf) {
				terrainPanelObject.gameObject.SetActive(false);
			}
			else {
				terrainPanelObject.gameObject.SetActive(true); 
			}
		}
		// toggle activation of map size control panel 
		if (Input.GetKeyDown(KeyCode.O)) {
			if (mapSizePanelObject.gameObject.activeSelf) {
				mapSizePanelObject.gameObject.SetActive(false); 
			}
			else {
				mapSizePanelObject.gameObject.SetActive(true); 
			}
		}

		// Zoom camera in and out 
		if (Input.GetAxis("Mouse ScrollWheel") != 0.0) {
			cameraObject.GetComponent<Camera>().orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity; 
		}

		// Move camera around 
		if (Input.GetMouseButtonDown(0)) {
			// mouse button is pressed, begin dragging screen around 
			dragging = true; 
		}
		else if (Input.GetMouseButtonUp(0)) {
			// mouse button is released, stop dragging screen around 
			dragging = false; 
		}
		if (dragging) {
			// finally, if we're supposed to be dragging the screen around remember to actually move it ;) 
			cameraObject.transform.Translate(-Input.GetAxis("Mouse X") * dragSensitivity * cameraObject.GetComponent<Camera>().orthographicSize, 
											-Input.GetAxis("Mouse Y") * dragSensitivity * cameraObject.GetComponent<Camera>().orthographicSize, 0f); 
		}

		// If the player presses G, the map object should generate a new map 
		if (Input.GetKeyDown(KeyCode.G)) {
			mapObject.GetComponent<MapGenerator>().regenerate(); 
		}
	}
}
