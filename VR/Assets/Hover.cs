using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public Vector3 hoverPoint;
    public float hoverRadius;
    int hoverMask = (1 << 10);
    public int maxHoverNums;

    [SerializeField]
    public static bool isHovering = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("HoverCheck");
    }

    // Update is called once per frame
    void Update()
    {
        Gizmos.DrawSphere(hoverPoint, hoverRadius);
    }

    //We are going to write a coroutine which will check for objects within the hover range. The hover range is just a transform and a radius.
    //So we will check if there is an object with an ObjectInteraction class inside the range by using Vector3.Distance and comparing that with the hover radius.

    private IEnumerator HoverCheck()
    {
        Collider[] colliders = new Collider[maxHoverNums];
        int numColliding = 0;
        while (true)
        {
            for(int i = 0; i < colliders.Length; i++)
            {
                colliders[i] = null;
            }

            numColliding = Physics.OverlapSphereNonAlloc(hoverPoint, hoverRadius, colliders, hoverMask);
            if(numColliding > maxHoverNums)
            {
                Debug.LogWarning("TOO MANY OBJECTS COLLIDING");
            }
            float closest = 0;
            GameObject closestHoverObj;
            for(int i = 0; i < colliders.Length; i++)
            {
                ObjectInteraction objInt = colliders[i].GetComponent<ObjectInteraction>();
                if (objInt == null)//This object has no objectInteraction script.
                {
                    continue;
                }
                float currentDist = Vector3.Distance(colliders[i].ClosestPoint(hoverPoint), hoverPoint);
                if (currentDist < closest)
                {//If the distance between the closest point on the collider and the hover point less than the current minimum, make this the new min.
                    closest = currentDist;
                    closestHoverObj = colliders[i].gameObject;                
                }
            }
            //Do something to indicate that the hand is currently hovering over an object that can be picked up.
            isHovering = true;

            yield return new WaitForSeconds(.1f);
        }
    }
}
