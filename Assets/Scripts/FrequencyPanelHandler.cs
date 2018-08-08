using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

// Controls the response to user input on the "Modify Terrain Frequencies" UI Panel 
// Each script is attached to a single category on that panel, and increases or decreases 
// the frequency value of an associated terrain tile 

// It also controls the displayed frequency number on the panel 
public class FrequencyPanelHandler : MonoBehaviour {
	// The prefab this particular instance of this script is modifying 
	public Transform tilePrefab; 

	// The text display of the frequency value 
	public Transform frequencyText; 

	void Start() {
		// update the frequency value on start 
		frequencyText.GetComponent<Text>().text = tilePrefab.GetComponent<TileData>().tileFrequency.ToString(); 
	}

	public void changeTileFrequency(int val) {
		TileData td = tilePrefab.GetComponent<TileData>(); 
		if (td.tileFrequency > 0 || (td.tileFrequency == 0 && val > 0)) {   // don't decrease frequency below zero or above 999 
			if (td.tileFrequency < 999 || (td.tileFrequency == 999 && val < 0)) {
				td.tileFrequency += val; 
				frequencyText.GetComponent<Text>().text = td.tileFrequency.ToString(); 
			}
		}
	}
}
