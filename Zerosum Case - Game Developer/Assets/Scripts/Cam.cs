using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    [SerializeField] private Transform followed;
    [SerializeField] private float dist;
    [SerializeField] PlayerSettings settings;


    void Start()
    {
        dist = transform.position.z - followed.transform.position.z;
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(followed.position.x, transform.position.y, followed.position.z + dist);
    }
}