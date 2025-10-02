using System;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public GameObject prefab;
    public bool is_static;
    public EventHandler OnHitSomething;
    public ParticleSystem particleSystem;
}
