using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour {

    float last_sync = 0;

    public bool control_left = false;
    public bool control_right = false;

    void Start() {

    }

    void Update() {
        if (control_left) {
            transform.position = new Vector3(
                transform.position.x - 0.01f,
                transform.position.y,
                transform.position.z
            );

            Broadcast();
        }

        if (control_right) {
            transform.position = new Vector3(
                transform.position.x + 0.01f,
                transform.position.y,
                transform.position.z
            );

            Broadcast();
        }
    }

    void Broadcast() {
        if (last_sync < Time.time) {
            last_sync = Time.time + 0.02f;

            Network.self.Broadcast(
                $"{transform.position.x}:" +
                $"{transform.position.y}:" +
                $"{transform.position.z}"
            );
        }
    }

}
