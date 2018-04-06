using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaunchArcRenderer : MonoBehaviour {
    public LineRenderer sightLine;

    public int segmentCount = 20;
    public float segmentScale = 1;

    private Collider _hitObject;
    public Collider hitObject { get { return _hitObject; } }

    void Awake()
    {
        sightLine = GetComponent<LineRenderer>();
    }

    public void simulatePath(Vector3 velocity)
    {
        Vector3[] segments = new Vector3[segmentCount];
        segments[0] = transform.position;
        Vector3 segVelocity = velocity;
        _hitObject = null;

        for (int i = 1; i < segmentCount; i++)
        {
            float segTime = (segVelocity.sqrMagnitude != 0) ? segmentScale / segVelocity.magnitude : 0;
            segVelocity = segVelocity + Physics.gravity * segTime;
            RaycastHit hit;
            if (Physics.Raycast(segments[i - 1], segVelocity, out hit, segmentScale))
            {
                _hitObject = hit.collider;
                segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;
                segVelocity = segVelocity - Physics.gravity * (segmentScale - hit.distance) / segVelocity.magnitude;

                for (int j = i; j < segmentCount; j++)
                    segments[j] = segments[i];
                break;
            }
            else
            {
                segments[i] = segments[i - 1] + segVelocity * segTime;
            }
        }

        sightLine.positionCount = segmentCount;
        for (int i = 0; i < segmentCount; i++)
            sightLine.SetPosition(i, segments[i]);
    }
}
