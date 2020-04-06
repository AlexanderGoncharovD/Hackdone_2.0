using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelController : MonoBehaviour
{

	public bool isMenu = true;
	public int lvl = 1;
	public int score;
	public float timerPass;

	void Start ( )
	{
		timerPass = 5.0f;
		DontDestroyOnLoad (this.gameObject);
	}


}
