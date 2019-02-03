using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    private LineRenderer lr;
    float timer = 0;
    int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponentInChildren<LineRenderer>();
    }

    public IEnumerator addPoint()
    {
        timer = 0;
        while (true)
        {
            lr.SetPosition(index, this.transform.GetChild(0).transform.position);

            timer += .02f;
            index++;
            yield return new WaitForSeconds(.02f);
        }
    }
}
