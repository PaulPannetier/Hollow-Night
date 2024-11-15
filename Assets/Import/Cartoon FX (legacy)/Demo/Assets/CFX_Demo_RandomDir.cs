using UnityEngine;
using System.Collections;
using System;

// Cartoon FX  - (c) 2015 Jean Moreno

public class CFX_Demo_RandomDir : MonoBehaviour
{
	public Vector3 min = new Vector3(0,0,0);
	public Vector3 max = new Vector3(0,360,0);
	
	void Start ()
	{
		this.transform.eulerAngles = new Vector3(Random.Rand(min.x,max.x),Random.Rand(min.y,max.y),Random.Rand(min.z,max.z));
	}
	
}
