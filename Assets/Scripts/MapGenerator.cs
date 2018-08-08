using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class MapGenerator : MonoBehaviour {
	// DEFINITIONS 
	// How large the map should be in number of tiles (map is currently always square) 
	public int mapSize = 150; 

	// The number of seed tiles placed into the map array when generation starts 
	// These tiles will then spread outwards as far as possible 
	public int seedTiles = 150; 

	// GLOBAL VARIABLES 
	// An array holding the types of terrain tiles used 
	public Transform[] tileTypes; 
	// Holds a list of integers where each int corresponds to a tileType. The proportion of 
	// ints is equivalent to the frequency of each tile type. This means that drawing an int
	// randomly from this list is the same as taking a tile type randomly with the correct frequency
	private List<int> tileOptions; 
	
	// Use this for initialization
	void Start () {
		// init UI elements 
		mapSizeText.GetComponent<Text>().text = mapSize.ToString(); 
		seedTilesText.GetComponent<Text>().text = seedTiles.ToString(); 
		updateTileOptions(); 
		regenerate(); 
	}
	
	// Update is called once per frame
	void Update () {
		// if for some reason seed tiles gets too large ever, reset it to an okay value 
		if (seedTiles > mapSize * mapSize) {
			seedTiles = mapSize * mapSize; 
			seedTilesText.GetComponent<Text>().text = seedTiles.ToString(); 
		}
	}

	// a public function that generates and then instantiates a new map 
	public void regenerate() {
		resetMap(); 

		// This is a 1d array that is representative of two dimensions, which holds a set of 
		// integers. each integer corresponds to a tile type. 
		int[] mapArray = new int[mapSize * mapSize]; 
		for (int i = 0; i < mapArray.Length; i++) {
			mapArray[i] = -1; 
		}

		// fill the map array with numbers 
		// This now instantiates tile objects as they're created, which is a little slower but 
		// allows the user to watch the generation happen. 
		// TODO: add a user-controlled toggle somewhere so they can decide whether they want fast 
		// or pretty generation 
		IEnumerator coroutine = createMap(mapArray, true); 
		StartCoroutine(coroutine); 

		// Now, go through the map and create game objects corresponding to each integer 
		//instantiateMap(mapArray);
	}

	// Builds a list of ints corresponding to tile types that is used in the createMap function
	// This is public so the user can change frequencies and then update tile options themselves 
	// TODO: there's probably a faster way of doing this, google it or something  
	public void updateTileOptions() {
		// reset the list, since tile frequencies might have changed 
		tileOptions = new List<int>(); 
		
		for (int i = 0; i < tileTypes.Length; i++) { 
			for (int j = 0; j < tileTypes[i].GetComponent<TileData>().tileFrequency; j++) {
				// add the int corresponding to a given tile type to the frequency list a number 
				// of times equal to that tile's frequency 
				tileOptions.Add(i); 
			}
		}
	}

	// Takes an array, seeds it with a number of tiles of different types, and then converts 
	// surrounding tiles to adjacent types until the map is completed. 
	// TODO: the instantiate bool isn't really necessary as this is now a coroutine. 
	// Not instantiating should be moved to a similar but function that isn't a couroutine 
	IEnumerator createMap(int[] map, bool instantiate = false) {
		// Random number generator 
		System.Random rand = new System.Random(); 

		// holds the indices that are gonna be updated in the next loop, so that we 
		// aren't iterating and modifying the map array at the same time 
		List<int> toUpdate = new List<int>(); 

		// "Seed" the mapArray with some starting values, which will then spread outwards 
		for (int i = 0; i < seedTiles; i++) {
			int nextIndex = rand.Next(mapSize * mapSize); 
			map[nextIndex] = tileOptions[rand.Next(tileOptions.Count)]; 
			// Add these seed indices to the update list, so they're updated immediately in the 
			// expansion loop 
			toUpdate.Add(nextIndex); 

			// if instantiate is true, instantiate these tiles immediately
			if (instantiate) {
				Instantiate(tileTypes[map[nextIndex]], new Vector3(nextIndex % mapSize, nextIndex / mapSize, 0), Quaternion.identity, this.transform); 
			}
		}

		
		// Each loop, every number that is not -1 will try to expand into surrounding cells
		// If none expand, the loop ends. 
		// TODO: use some functions to make this less indented lol 

		// this boolean tracks whether any surrounding cells were updated 
		bool indexUpdated; 

		while (toUpdate.Count > 0) {
			List<int> newUpdate = new List<int>(toUpdate); 
			for (int i = 0; i < toUpdate.Count; i++) {

				indexUpdated = false; 

				// update surrounding cells, then add them to list 
				// if instantiate is true, create the new tiles as they are added to the map array 
				// (see instantiateMap function for an explanation of the method call) 
				if (toUpdate[i] >= mapSize) {
					// cell above 
					indexUpdated = updateCell(map, newUpdate, toUpdate[i] - mapSize, toUpdate[i], instantiate) || indexUpdated; 
				}
				if (toUpdate[i] % mapSize != (mapSize - 1)) {
					// cell to the right 
					indexUpdated = updateCell(map, newUpdate, toUpdate[i] + 1, toUpdate[i], instantiate) || indexUpdated; 
				}
				if (toUpdate[i] < map.Length - mapSize) {
					// cell below 
					indexUpdated = updateCell(map, newUpdate, toUpdate[i] + mapSize, toUpdate[i], instantiate) || indexUpdated; 
				}
				if (toUpdate[i] % mapSize != 0) {
					// cell to the left 
					indexUpdated = updateCell(map, newUpdate, toUpdate[i] - 1, toUpdate[i], instantiate) || indexUpdated; 
				}

				// if this index didn't update anything, we can remove it 
				if (!indexUpdated) { 
					newUpdate.Remove(toUpdate[i]); 
				}
			}

			toUpdate = new List<int>(newUpdate); 

			yield return null; 
		}
	}

	// attempts to update a cell in a given map array to be the value of the original cell
	// returns true on success
	// It will also instantiate a new map tile if instantiate is set to true (only on successes) 
	private bool updateCell(int[] map, List<int> updateList, int newCell, int originalCell, bool instantiate = false) {
		if (map[newCell] == -1) {
			map[newCell] = map[originalCell];  
			updateList.Add(newCell);
			if (instantiate) {
				Instantiate(tileTypes[map[newCell]], new Vector3(newCell % mapSize, newCell / mapSize, 0), Quaternion.identity, this.transform); 
			}
			return true; 
		}
		else {
			return false; 
		}
	}

	// Takes an array, clears all children of this game object, and then uses the array to 
	// generate a "map" of gameobjects, where each game object is a tile 
	private void instantiateMap(int[] map) {
		resetMap(); 

		for (int i = 0; i < map.Length; i++) {
			// For each entry of mapArray, gets a prefab from tileTypes (corresponding to the integer value of the entry) 
			// and creates a new instance of it 
			// Each new instance is created at a point corresponding to the x, y position of the mapArray entry 
			// The created instance is additionally made a child of this object 
			Instantiate(tileTypes[map[i]], new Vector3(i % mapSize, i / mapSize, 0), Quaternion.identity, this.transform); 
		}
	}

	// Deletes all the currently instantiated map tiles 
	private void resetMap() {
		// Clear the current children (to wipe the map clean) 
		foreach(Transform child in this.transform) {
			GameObject.Destroy(child.gameObject); 
		}
	}

	// UI FUNCTIONS 
	// These functions modify variables in this file, and are controlled 
	// via the "Map Size Options" panel 
	// UI VARIABLES
	// stuff that needs to be updated in the UI when these functions are used 
	public Transform mapSizeText;
	public Transform seedTilesText; 

	public void changeMapSize(int val) {
		if (mapSize > 0 || (mapSize == 0 && val > 0)) { // don't decrease map size below zero 
			if (mapSize < 9999 || (mapSize == 9999 && val < 0)) { // don't increase map size above 9999 (which would already be insane lol)
				mapSize += val; 
				mapSizeText.GetComponent<Text>().text = mapSize.ToString(); 
			}
		}
	}

	public void changeSeedTiles(int val) {
		if (seedTiles > 0 || (seedTiles == 0 && val > 0)) { // don't decrease # of seed tiles below zero 
			if (seedTiles < mapSize * mapSize || (seedTiles == mapSize * mapSize && val < 0)) { // don't increase # of seed tiles above number of tiles in map 
				seedTiles += val; 
				seedTilesText.GetComponent<Text>().text = seedTiles.ToString(); 
			}
		}
	}
}
