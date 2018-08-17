using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour {

	public GameObject output_prefab;
	public Training input_map;
	public GameObject[,] map;
	public Transform player;

	public float closeness_to_border_when_load = 0.2f, closeness_to_border_when_unload = 0;

	public int width_output = 22, length_output = 19, gridsize_output = 20;


	private OverlapWFC output_settings;
	private float width, length, center_x, center_y;
	private int initial_seed, initial_seed_x, initial_seed_y;
	private bool should_create_diagonal_left, should_create_diagonal_right;



	// Use this for initialization
	void Start () {
		map = new GameObject[3, 3];
		UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
		initial_seed = UnityEngine.Random.Range (0,Int32.MaxValue);
		initial_seed_x = UnityEngine.Random.Range (0,Int32.MaxValue);
		initial_seed_y = UnityEngine.Random.Range (0,Int32.MaxValue);

		InstantiateChunk (0, 0, 1, 1);

		output_settings =  map[1,1].GetComponent<OverlapWFC> ();
		width = width_output * gridsize_output;
		length = length_output * gridsize_output;

		center_x = 0;
		center_y = 0;
	}
	
	// Update is called once per frame
	void Update () {

		should_create_diagonal_left = false;
		should_create_diagonal_right = false;


		if (player.position.x > center_x + width) {

			//Shift left (player is going right)
			map [0, 0] = map [0, 1];
			map [0, 1] = map [0, 2];
			map [0, 2] = null;
			map [1, 0] = map [1, 1];
			map [1, 1] = map [1, 2];
			map [1, 2] = null;
			map [2, 0] = map [2, 1];
			map [2, 1] = map [2, 2];
			map [2, 2] = null;

			center_x = center_x + width;

		} else if (player.position.x > center_x + width * (1 - closeness_to_border_when_load)) {

			if (map [1, 2] == null) {
				InstantiateChunk (center_x + width, center_y, 1, 2);
			}

			should_create_diagonal_right = true;
				
		} else if (player.position.x > center_x + width * closeness_to_border_when_unload) {

			//remove chunk if necessary (center_left)
			if (map [1, 0] != null) {
				Destroy (map [1, 0]);
				map [1, 0] = null;
			}

			//remove chunk if necessary (upper_left)
			if (map [0, 0] != null) {
				Destroy (map [0, 0]);
				map [0, 0] = null;
			}

			//remove chunk if necessary (lower_left)
			if (map [2, 0] != null) {
				Destroy (map [2, 0]);
				map [2, 0] = null;
			}

		} else if (player.position.x > center_x + width * closeness_to_border_when_load) {

			//remove chunk if necessary
			if (map [1, 2] != null) {
				Destroy (map [1, 2]);
				map [1, 0] = null;
			}

			//remove chunk if necessary (upper_right)
			if (map [0, 2] != null) {
				Destroy (map [0, 2]);
				map [0, 0] = null;
			}

			//remove chunk if necessary (lower_right)
			if (map [2, 2] != null) {
				Destroy (map [2, 2]);
				map [2, 2] = null;
			}
			
		} else if (player.position.x > center_x) {

			should_create_diagonal_left = true;

			if (map [1, 0] == null) {
				InstantiateChunk (center_x - width, center_y, 1, 0);
			}
				
		} else {
			//Shift right (player is going left)
			map [0, 2] = map [0, 1];
			map [0, 1] = map [0, 0];
			map [0, 0] = null;
			map [1, 2] = map [1, 1];
			map [1, 1] = map [1, 0];
			map [1, 0] = null;
			map [2, 2] = map [2, 1];
			map [2, 1] = map [2, 0];
			map [2, 0] = null;

			center_x = center_x - width;

		}

		if (player.position.y > center_y + length) {

			//Shift down (player is going up)
			map [2, 0] = map [1, 0];
			map [1, 0] = map [0, 0];
			map [0, 0] = null;
			map [2, 1] = map [1, 1];
			map [1, 1] = map [0, 1];
			map [0, 1] = null;
			map [2, 2] = map [1, 2];
			map [1, 2] = map [0, 2];
			map [0, 2] = null;

			center_y = center_y + length;
			

		} else if (player.position.y > center_y + length * (1 - closeness_to_border_when_load)) {
			
			if (map [0, 1] == null) {
				InstantiateChunk (center_x, center_y + length, 0, 1);
			}

			if (should_create_diagonal_left && map [0, 0] == null) {
				InstantiateChunk (center_x - width, center_y + length, 0, 0);
			}

			if (should_create_diagonal_right && map [0, 2] == null) {
				InstantiateChunk (center_x + width, center_y + length, 0, 2);
			}

		} else if (player.position.y > center_y + length * closeness_to_border_when_unload) {

			//remove chunk if necessary (lower_center)
			if (map [2, 1] != null) {
				Destroy (map [2,1]);
				map [2,1] = null;
			}

			//remove chunk if necessary (lower_left)
			if (map [2, 0] != null) {
				Destroy (map [2,0]);
				map [2, 0] = null;
			}

			//remove chunk if necessary (lower_right)
			if (map [2, 2] != null) {
				Destroy (map [2, 2]);
				map [2, 2] = null;
			}


		} else if (player.position.y > center_y + length * closeness_to_border_when_load) {

			//remove chunk if necessary (upper_center)
			if (map [0, 1] != null) {
				Destroy (map [0,1]);
				map [0,1] = null;
			}

			//remove chunk if necessary (upper_left)
			if (map [0, 0] != null) {
				Destroy (map [0,0]);
				map [0,0] = null;
			}

			//remove chunk if necessary (upper_right)
			if (map [0, 2] != null) {
				Destroy (map [0,2]);
				map [0,2] = null;
			}

		} else if (player.position.y > center_y) {



			if (map [2, 1] == null) {
				InstantiateChunk (center_x, center_y - length, 2, 1);
			}

			if (should_create_diagonal_left && map [2, 0] == null) {
				InstantiateChunk (center_x - width, center_y - length, 2, 0);
			}

			if (should_create_diagonal_right && map [2, 2] == null) {
				InstantiateChunk (center_x + width, center_y - length, 2, 2);
			}

		} else {
			
			//Shift up (player is going down)
			map [0, 0] = map [1, 0];
			map [1, 0] = map [2, 0];
			map [2, 0] = null;
			map [0, 1] = map [1, 1];
			map [1, 1] = map [2, 1];
			map [2, 1] = null;
			map [0, 2] = map [1, 2];
			map [1, 2] = map [2, 2];
			map [2, 2] = null;


			center_y = center_y - length;

		}
	}


	GameObject InstantiateChunk(float pos_x, float pos_y, int i, int j) {
		map [i,j] = Instantiate (output_prefab) as GameObject;
		output_settings = map[i,j].GetComponent<OverlapWFC> ();

		output_settings.training = input_map;
		output_settings.seed = random_from_xy (pos_x, pos_y);
		output_settings.width = width_output;
		output_settings.depth = length_output;
		output_settings.gridsize = gridsize_output;
		output_settings.Generate ();

		Transform t = map [i, j].transform;
		t.position = new Vector3 (pos_x, pos_y, 0f);


		return map [i, j];
	}


	int random_from_xy(float x, float y) {
		int random_x, random_y;
		UnityEngine.Random.seed = initial_seed_x + (int)x;
		random_x = UnityEngine.Random.Range (0,Int32.MaxValue);

		UnityEngine.Random.seed = initial_seed_y + (int)y;
		random_y = UnityEngine.Random.Range (0,Int32.MaxValue);

		UnityEngine.Random.seed = initial_seed + random_x + random_y;

		return UnityEngine.Random.Range (0,Int32.MaxValue);
	}
}
