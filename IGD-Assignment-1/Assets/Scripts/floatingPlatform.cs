using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingPlatform : MonoBehaviour
{

    private float sinPos = 0f;

    // Update is called once per frame
    void Update()
    {
        sinPos += 1 * Time.deltaTime;
        transform.Rotate(0, Time.deltaTime * 200, 0, Space.Self);  //Causes player to SPIN
        transform.position = this.transform.position + new Vector3(Mathf.Sin(sinPos) / 5, 0, Mathf.Cos(sinPos) / 5);
    }


}
