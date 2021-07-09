using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;

    public bool flying;
    public bool shaking;
    public bool poked;
    public bool straining;
    public bool poking;

    GameController gameController;
    public bool powerX;

    TubeRenderer tubeRenderer;
    Vector3[] points;

    public GameObject hitEffect;
    public ParticleSystem fireEffect;
    Vector3 offset;

    Vector3 hitPoint = Vector3.zero;

    void Start()
    {
        tubeRenderer = FindObjectOfType<TubeRenderer>();
        gameController = FindObjectOfType<GameController>();
        rb = GetComponent<Rigidbody>();
        offset = new Vector3(0, 0, 0.01f);
        points = new Vector3[3];
        Poke();
    }

    void Update()
    {
        if (flying && rb.velocity.y < 0f)
        {
            fireEffect.Stop();
        }
        points[0] = transform.position;
        points[1] = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z - 0.75f);
        points[2] = hitPoint;
        DrawCurvedLine();
    }

    public void Shake(Transform shakeObject,float shakePower)
    {
        shakeObject.position = shakeObject.position + Random.insideUnitSphere / 100 * (shakePower / 100);
    }

    public void Poke()
    {
        rb.velocity /= 10;
        rb.angularVelocity /= 10;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 5f))
        {
            poking = true;
            hitPoint = hit.point;
            if (hit.transform.CompareTag("Normal"))
            {
                Stay(hit.point);
            }
            if (hit.transform.CompareTag("Target"))
            {
                Stay(hit.point);
                if (Mathf.Abs(hit.point.y - hit.transform.position.y) < 0.25f)
                {
                    powerX = true;
                    transform.GetComponent<TrailRenderer>().enabled = false;
                }
            }
            if (hit.transform.CompareTag("Metal"))
            {
                Invoke("ClearLine", 0.05f);
            }
            if (hit.transform.CompareTag("Red"))
            {
                Invoke("ClearLine", 0.05f);
                gameController.GameOver();
            }
            if (hit.transform.CompareTag("Empty"))
            {
                Invoke("ClearLine", 0.05f);
            }
        }
        else
        {
            Invoke("ClearLine", 0.05f);
        }
    }

    void DrawCurvedLine()
    {
        if (straining)
        {
            tubeRenderer.SetPositions(LineSmoother.SmoothLine(points, 0.02f));
        }
        else if (poked || poking)
        {
            tubeRenderer.SetPositions(points);
            tubeRenderer.enabled = true;
        }
    }

    void ClearLine()
    {
        poking = false;
        tubeRenderer.enabled = false;
    }
    public void Jump(float power)
    {
        if (powerX)
        {
            fireEffect.Play();
        }
        poked = false;
        flying = true;
        ClearLine();
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * power * 5);
        rb.AddTorque(Vector3.right * power * 200);
    }

    void Stay(Vector3 hitPoint)
    {
        transform.GetComponent<TrailRenderer>().enabled = true;
        powerX = false;
        fireEffect.Stop();
        poked = true;
        flying = false;
        Instantiate(hitEffect, hitPoint - offset, Quaternion.identity);
        rb.isKinematic = true;
        transform.rotation = Quaternion.identity; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            gameController.GameOver();
        }
    }
    
}
