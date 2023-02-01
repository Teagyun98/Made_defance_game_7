using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public GameObject Camera;
	public string mod;
	public GameObject ArrowBlue;
	public GameObject ArrowRed;
	public ParticleSystem MasicBlue;
	public ParticleSystem MasicRed;

	private void Awake()
	{
		instance = this;
	}

}
