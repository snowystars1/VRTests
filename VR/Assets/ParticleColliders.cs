using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColliders : MonoBehaviour
{
    ParticleSystem attack;
    public GameObject particleCollider;
    private Rigidbody pcRb;
    // Start is called before the first frame update
    void Start()
    {
        attack = GetComponent<ParticleSystem>();
        pcRb = particleCollider.GetComponent<Rigidbody>();
        StartCoroutine("Pulse");
    }

    // Update is called once per frame

    IEnumerator Pulse()
    {
        while (true)
        {
            particleCollider.transform.position = transform.position; //Move it
            pcRb.velocity = Vector3.zero;
            pcRb.AddForce(attack.main.startSpeedMultiplier * this.transform.up, ForceMode.VelocityChange);
            attack.Play();
            yield return new WaitForSeconds(3f);
        }
    }
}
