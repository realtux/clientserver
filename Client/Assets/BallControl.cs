using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour {

    float last_sync = 0;

    int control_left = 0;
    int control_right = 0;

    void Update() {
        control_left = Input.GetKey("a") ? 1 : 0;
        control_right = Input.GetKey("d") ? 1 : 0;

        if (last_sync < Time.time) {
            last_sync = Time.time + 0.02f;
            Network.self.Send($"{control_left}:{control_right}");
        }
    }

    public void UpdatePos(Vector3 position) {
        transform.position = position;
    }

}
