using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour 
{
	bool paused = false;
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape) && paused == false) 
		{   
			paused = true;  
			Time.timeScale = 0.0f;
		}
		else if(Input.GetKeyDown(KeyCode.Escape) && paused == true) 
		{
			paused = false;
			Time.timeScale = 1.0f;
		}
	}
}
