using UnityEngine;
using System.Collections;
using System;

// Cartoon FX  - (c) 2015 Jean Moreno

public class CFX_Demo_RandomDirectionTranslate : MonoBehaviour
{
	public float speed = 30.0f;
	public Vector3 baseDir = Vector3.zero;
	public Vector3 axis = Vector3.forward;
	public bool gravity;
	private Vector3 dir;
	
	void Start ()
	{
		dir = new Vector3(Random.Rand(0.0f,360.0f),Random.Rand(0.0f,360.0f),Random.Rand(0.0f,360.0f)).normalized;
		dir.Scale(axis);
		dir += baseDir;
	}
	
	void Update ()
	{
		this.transform.Translate(dir * speed * Time.deltaTime);
		
		if(gravity)
		{
			this.transform.Translate(Physics.gravity * Time.deltaTime);
		}
	}
}
