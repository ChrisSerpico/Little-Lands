using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Holds data for a tile, such as how frequently it should appear 
// Read by the map generator when creating a new map 
public class TileData : MonoBehaviour {
	// how often this tile should show up during generation. 
	// Note that this is a relative value. If all tiles have a value of 100, they'll all 
	// show up just as often as one another. But a tile with a value of 1 will show up 
	// 1/5th as often as a tile with a value of 5 
	public int tileFrequency = 10; 
}
