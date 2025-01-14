using OscJack;
using OscJack.Annotation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[OscPackable]
public partial class PacketData
{
    [OscElementOrder(0)]
    public int x;
    [OscElementOrder(1)]
    public int y;
    [OscElementOrder(3)]
    public int z;
    [OscElementOrder(2)]
    public string message;
    [OscElementOrder(4)]
    public float value;
    [OscElementOrder(5)]
    public byte[] bytes;

    public PacketData()
    {
    }
}

public class PacketTest : MonoBehaviour
{
    private OscClient client;
    private OscServer server;
    private int count = 0;

    private void Start()
    {
        this.client = new OscClient("127.0.0.1", 12345);

        this.server = new OscServer(12345);
        this.server.MessageDispatcher.AddCallback("/test", OnPacketReceived);
    }

    private void Update()
    {
        if (this.client != null)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                var packet = new PacketData
                {
                    x = count,
                    y = count + 1,
                    z = count + 2,
                    message = "Hello",
                    value = 3.14f,
                    bytes = new byte[] { 0x00, 0x01, 0x02 }
                };
                this.client.Send("/test", packet.ToObjects());
                count++;
            }
        }
    }

    private void OnPacketReceived(string address, OscDataHandle handle)
    {
        var packet = new PacketData(handle);
        Debug.Log(handle.GetElementAsInt(0));
        Debug.Log($"{packet.x} {packet.y}, {packet.z}, {packet.message}, {packet.value}, {packet.bytes.SequenceEqual(new byte[] { 0x00, 0x01, 0x02 })}");
    }
}