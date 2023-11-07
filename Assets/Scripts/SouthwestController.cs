using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SouthwestController : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    Vector3 defaultPosition;
    bool rise = false;
    private void Start() {
        defaultPosition = transform.position;
    }
    void Update()
    {
        if(transform.position.y -defaultPosition.y < 5 && rise)
        {
            transform.position += new Vector3(0.5f * Time.deltaTime, 2f * Time.deltaTime, 0.25f * Time.deltaTime);
            transform.Rotate(0, 0, 1 * Time.deltaTime, Space.Self);
        }
        else if(transform.position.y -defaultPosition.y > -5 && !rise)
        {
            transform.position -= new Vector3(0.5f * Time.deltaTime, 2f * Time.deltaTime, 0.25f * Time.deltaTime);
            transform.Rotate(0, 0, -1 * Time.deltaTime, Space.Self);
        }
        else { rise = !rise; }
    }
}
