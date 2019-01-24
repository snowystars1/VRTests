using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [HideInInspector]
    public Vector3 hoverPoint;

    public float hoverRadius;
    int hoverMask = (1 << 10);
    public int maxHoverNums;
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
                {
                    closestHoverObj.GetComponent<Renderer>().material.color = currentColor;
                    closestHoverObj = null;
                }
                yield return new WaitForSeconds(.1f);
                continue;
            }

            float closest = 0;
            closestHoverObj=null;
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
            if (closestHoverObj != null)
            {
                Renderer objRenderer = closestHoverObj.GetComponent<Renderer>();
                currentColor = objRenderer.material.color; //Store the current color, so we can reset the object color in later loop iterations.
                objRenderer.material.color = Color.yellow; //If they are hovering over an object that you can pick up, it will turn yellow.
            }

            yield return new WaitForSeconds(.1f);
        }
    }
}
