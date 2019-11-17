﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public Transform origin;
    public Transform target;

    [SerializeField]
    private float startingBlinkInterval = 1;
    [SerializeField]
    private float endingBlinkInterval = 0.05f;
    [SerializeField]
    private float blinkIterations = 10;
    [SerializeField]
    private float minTimePerIteration = 0.1f;

    [SerializeField]
    private float beamDelayTime = 0.25f;

    [SerializeField]
    private float beamGrowTime = 1;
    /// Duration after it has finished growing
    [SerializeField]
    private float beamDuration = 1;
    [SerializeField]
    private float beamFadeTime = 0.5f;

    private LineRenderer lr;
    private SpriteRenderer sr;

    private Transform beam;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        sr = GetComponentInChildren<SpriteRenderer>();
        beam = transform.GetChild(0);

        lr.SetPosition(0, origin.position);
        lr.SetPosition(1, target.position);
        StartCoroutine(BlinkRoutine());
    }

    void Update()
    {
        lr.SetPosition(0, origin.position);
        lr.SetPosition(1, target.position);
    }

    private IEnumerator BlinkRoutine()
    {
        float deltaTime = (startingBlinkInterval - endingBlinkInterval) / blinkIterations;
        for (int i = 0; i <= blinkIterations; ++i)
        {
            float waitTime = startingBlinkInterval - deltaTime * i;
            float startTime = Time.time;
            do
            {
                lr.enabled = true;
                yield return new WaitForSeconds(waitTime);

                lr.enabled = false;
                yield return new WaitForSeconds(waitTime);
            }
            while (Time.time - startTime < minTimePerIteration);
        }

        StartCoroutine(BeamRoutine());
    }

    private IEnumerator BeamRoutine()
    {
        transform.position = origin.position;
        Vector2 direction = (Vector2)(target.position - transform.position);
        beam.right = direction;

        yield return new WaitForSeconds(beamDelayTime);

        // Beam growth
        {
            float startTime = Time.time;
            do
            {
                yield return null;
                beam.localScale = new Vector3(beam.localScale.x,
                    Mathf.Lerp(0, 0.3f, (Time.time - startTime) / beamGrowTime),
                    beam.localScale.z);
                Debug.Log((Time.time - startTime) / beamGrowTime);
            }
            while (Time.time - startTime < beamGrowTime);
        }

        // Beam life
        yield return new WaitForSeconds(beamDuration);

        //TODO: Disable collider

        // Beam death
        {
            float startTime = Time.time;
            do
            {
                yield return null;
                sr.color = Util.ModifyAlpha(sr.color, Mathf.Lerp(1, 0, (Time.time - startTime) / beamFadeTime));
            }
            while (Time.time - startTime < beamFadeTime);
        }
        Destroy(gameObject);
    }
}
