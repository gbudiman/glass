﻿using UnityEngine;
using System.Collections;

public class RSGDestructor : MonoBehaviour {
  Breakshot breakshot;
	// Use this for initialization
	void Start () {
    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void Trigger() {
    breakshot = GameObject.FindObjectOfType<Breakshot>();
    breakshot.Trigger();
  }

  void Destroy() {
    Destroy(gameObject);
  }
}