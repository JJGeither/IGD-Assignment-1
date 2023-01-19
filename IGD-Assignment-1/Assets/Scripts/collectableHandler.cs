using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectableHandler : MonoBehaviour
{

    private float sinPos = 0f;
    public GameObject playerObject;

    // Update is called once per frame
    void Update()
    {
        sinPos += 1 * Time.deltaTime;
        transform.Rotate(0, Time.deltaTime * 20, 0, Space.Self);  //Causes player to SPIN
        transform.position = this.transform.position + new Vector3(0, Mathf.Sin(sinPos) / 200, 0);
    }


}
