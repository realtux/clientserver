using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Network : MonoBehaviour {

    public static Network self { get; set; }

    public static Thread inbound_t;
    public static UdpClient client;
    public static List<IPEndPoint> connections;

    public GameObject ball;
    BallControl ball_control;

    void Start() {
        ball_control = ball.GetComponent<BallControl>();

        connections = new List<IPEndPoint>();

        client = new UdpClient(1234);

        inbound_t = new Thread(new ThreadStart(Inbound));
        inbound_t.Start();

        self = this;
    }

    void OnDestroy() {
        inbound_t.Abort();
    }

    void Inbound() {
        while (true) {
            IPEndPoint any;

            try {
                any = new IPEndPoint(IPAddress.Any, 1234);

                byte[] raw = client.Receive(ref any);
                string[] data = Encoding.ASCII.GetString(raw).Split(':');

                if (!connections.Any(c => c.Equals(any))) {
                    connections.Add(any);
                }

                ball_control.control_left = int.Parse(data[0]) == 1;
                ball_control.control_right = int.Parse(data[1]) == 1;
            } catch (SocketException) {
                // ignore, it's udp nobody cares
            } catch (Exception e) {
                Debug.Log("general problem receiving data");
                Debug.LogError(e);
            }
        }
    }

    public void Broadcast(string data) {
        byte[] bytes = Encoding.ASCII.GetBytes(data);

        foreach (var connection in connections) {
            try {
                client.Send(bytes, bytes.Length, connection);
            } catch (Exception) {
                Debug.Log("failed to broadcast?");
            }
        }
    }

}
