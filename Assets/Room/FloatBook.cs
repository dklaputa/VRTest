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
                     Quaternion.LookRotation(transform.position - playerPosition);
        transform.GetChild(1).rotation = Quaternion.LookRotation(transform.position - playerPosition);
    }

    public void ShowIntroduction()
    {
        if (UI.IsShow) return;
        transform.GetChild(1).GetComponent<UI>().Show();
    }
}