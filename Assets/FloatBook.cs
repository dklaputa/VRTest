using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatBook : MonoBehaviour
{
    private readonly Vector3 playerPosition = new Vector3(0, 5, 0);

    // Use this for initialization
    void Start()
    {
        GetComponent<Animator>().Play("FloatBook", 0, Random.value);
        transform.GetChild(0).rotation =
            Quaternion.LookRotation(playerPosition - transform.position);
    }

    // Update is called once per frame
    void Update()
    {
    }
}