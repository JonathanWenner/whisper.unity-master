using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    // Local IP Address (Use Serial to send this command: "set-server <IP>")
    public string IP;

    // Port for the server to listen and send messages to
    public int WandPort1, WandPort2;

    // Reference to the wands for callbacks
    public Wand Wand1, Wand2;

    // For debug purposes
    public bool EnableLogging;

    // Make sure the TCP server is a singleton
    public static TCPServer instance {  get; private set; }

    TcpListener WandServer1, WandServer2;
    TcpClient WandClient1, WandClient2;
    NetworkStream stream1, stream2;
    Thread thread1, thread2;


    private void Start() {
        // Make sure this is the only server running
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(this);
        
        GetLocalIPAddress();

        // spin up the threads housing the servers
        Log("Starting Server for Wand 1");
        thread1 = new Thread(new ThreadStart(ThreadServer1));
        thread1.Start();

        Log("Starting Server for Wand 2");
        thread2 = new Thread(new ThreadStart(ThreadServer2));
        thread2.Start();
    }

    private void GetLocalIPAddress() {
        // This is the name of this machine
        string hostName = Dns.GetHostName();
        instance.Log("Host Name: " + hostName);

        // Go through all the IP addresses assigned to this machine, and get the IPv4 one
        foreach (IPAddress ip in Dns.GetHostEntry(hostName).AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                IP = ip.ToString();
                instance.Log("Local IP: " + IP);
                break;
            }
        }
    }

    private void Update() {
        // placeholder for start procedure
        if (Input.GetKeyDown(KeyCode.Space)) {
            SendMessageToAll("start 3000");
        }
    }

    // parameterises bland method calls
    private void ThreadServer1() {
        ThreadServer(ref Wand1, ref WandServer1, ref WandClient1, ref stream1, WandPort1, "Wand 1");
    }
    private void ThreadServer2() {
        ThreadServer(ref Wand2, ref WandServer2, ref WandClient2, ref stream2, WandPort2, "Wand 2");
    }

    // this is the real meat of the show
    private void ThreadServer(ref Wand wand, ref TcpListener server, ref TcpClient client, ref NetworkStream stream, int port, string name) {
        try {
            // create server
            IPAddress local = IPAddress.Parse(IP);
            server = new TcpListener(local, port);
            server.Start();
            instance.Log("Starting Server for " + name + " at " + IP + ":" + port);

            // instantiate buffer
            byte[] buffer = new byte[1024];
            string data = null;

            // keeps listening for new connections if the old one drops out
            while (true) {
                instance.Log("Waiting for connection");
                client = server.AcceptTcpClient();
                instance.Log("Connected to " + name);

                data = null;
                stream = client.GetStream();

                int i;

                while (true) {
                    // if connection is broken, start listening for new ones
                    if (!client.Connected) break;

                    // otherwise, if the stream has data on it, store it in the buffer, then send it to the corresponding wand for callback
                    while ((i = stream.Read(buffer, 0, buffer.Length)) != 0) {
                        data = Encoding.UTF8.GetString(buffer, 0, i);

                        if (data.StartsWith('#')) Debug.Log("Debug message from " + name + ": " + data[1..]);
                        else wand.callback(data);
                    }
                }
            }
        }
        catch (SocketException e) {
            instance.Log("SocketException: " + e);
        }
        finally {
            server.Stop();
        }
    }

    public void SendMessageToAll(string message) {
        SendMessageToClient(stream1, message);
        SendMessageToClient(stream2, message);
    }

    public void SendMessageToWand1(string message)
    {
        if (!WandClient1.Connected) return;
        SendMessageToClient(stream1, message);
    }

    public void SendMessageToWand2(string message)
    {
        if (!WandClient2.Connected) return;
        SendMessageToClient(stream2, message);
    }

    public void SendMessageToClient(NetworkStream stream, string message) {
        byte[] msg = Encoding.UTF8.GetBytes(message);
        stream.Write(msg, 0, msg.Length);
        Log("Sent: " + message);
    }

    private void OnApplicationQuit() {
        thread1.Abort();
        thread2.Abort();
        stream1.Close();
        stream2.Close();
        WandClient1.Close();
        WandClient2.Close();
        WandServer1.Stop();
        WandServer2.Stop();
    }

    public void Log(string msg) {
        if (EnableLogging) Debug.Log(msg);
    }
}
