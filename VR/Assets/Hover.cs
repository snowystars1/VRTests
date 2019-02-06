using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    //[HideInInspector]
    public Vector3 hoverPoint;

    public float hoverRadius;
    public LayerMask hoverMask;
    public int maxHoverNums;
    [HideInInspector]
    public GameObject closestHoverObj = null;

    [SerializeField]
    public bool isHovering = false;//We'll have to get the instance of the class running on the controller cause there are two controllers.

    // Start is called before the first frame update
    void Start()
    {
        hoverPoint = transform.position;
        StartCoroutine("HoverCheck");
    }

    // Update is called once per frame
    void Update()
    {
        //hoverPoint = this.transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(hoverPoint, hoverRadius);
    }

    //We are going to write a coroutine which will check for objects within the hover range. The hover range is just a transform and a radius.
    //So we will check if there is an object with an ObjectInteraction class inside the range by using Vector3.Distance and comparing that with the hover radius.

    private IEnumerator HoverCheck()
    {
        Collider[] colliders = new Collider[maxHoverNums];
        closestHoverObj=null;
        Color currentColor = Color.clear;//just a null color value
        int numColliding = 0;
        while (true)
        {
            hoverPoint = transform.position;

            for(int i = 0; i < colliders.Length; i++)
            {
                colliders[i] = null;
            }

            numColliding = Physics.OverlapSphereNonAlloc(hoverPoint, hoverRadius, colliders, hoverMask);

            if(numColliding > maxHoverNums)
            {
                Debug.LogWarning("TOO MANY OBJECTS COLLIDING");
            }
            if (numColliding == 0)
            {
                isHovering = false;
                if (closestHoverObj != null)//This means we were just hovering over something, and now we stopped
                    closestHoverObj = null;
                yield return new WaitForSeconds(.1f);
                continue;
            }

            float closest = hoverRadius;
            closestHoverObj=null;
            for(int i = 0; i < colliders.Length; i++)
            {
                try
                {
                    ObjectInteraction objInt = colliders[i].gameObject.GetComponent<ObjectInteraction>();
                }catch(NullReferenceException)
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
