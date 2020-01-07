using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Network : MonoBehaviour {

    public static Network self { get; set; }

    public static Thread inbound_t;
    public static UdpClient client;
    public static bool connected = false;
    public static IPEndPoint endpoint;

    public GameObject ball;
    BallControl ball_control;

    static public Queue<Action> queue = new Queue<Action>();

    void Start() {
        ball_control = ball.GetComponent<BallControl>();

        endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);

        client = new UdpClient();
        client.Connect(endpoint);
        connected = true;

        inbound_t = new Thread(new ThreadStart(Inbound));
        inbound_t.Start();

        self = this;
    }

    public void Update() {
        while (queue.Count > 0) {
            Action action = null;
            lock (queue) {
                if (queue.Count > 0)
                    action = queue.Dequeue();
            }
            action?.Invoke();
        }
    }

    public void Defer(Action action) {
        lock (queue) {
            queue.Enqueue(action);
        }
    }

    void OnDestroy() {
        inbound_t.Abort();
    }

    void Inbound() {
        while (true) {
            try {
                if (!connected) {
                    client = new UdpClient();
                    client.Connect(endpoint);
                    connected = true;
                }

                byte[] raw = client.Receive(ref endpoint);
                string[] data = Encoding.ASCII.GetString(raw).Split(':');
                
                Defer(() => {
                    ball_control.UpdatePos(new Vector3(
                        float.Parse(data[0]),
                        float.Parse(data[1]),
                        float.Parse(data[2])
                    ));
                });
            } catch (SocketException e) {
                connected = false;

                Debug.Log($"socket problem, waiting 500ms and reconnecting");

                Thread.Sleep(500);
            }
        }
    }

    public void Send(string data) {
        byte[] bytes = Encoding.ASCII.GetBytes(data);
        client.Send(bytes, bytes.Length);

        Debug.Log($"sending updates {data}");
    }

}
