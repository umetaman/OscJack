// OSC Jack - Open Sound Control plugin for Unity
// https://github.com/keijiro/OscJack

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace OscJack
{
    public sealed class OscClient : IDisposable
    {
        #region Object life cycle

        public OscClient(string destination, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            if (destination == "255.255.255.255")
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            var dest = new IPEndPoint(IPAddress.Parse(destination), port);
            _socket.Connect(dest);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Packet sender methods

        public void Send(string address)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",");
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, int data)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",i");
            _encoder.Append(data);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, int element1, int element2)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",ii");
            _encoder.Append(element1);
            _encoder.Append(element2);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, int element1, int element2, int element3)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",iii");
            _encoder.Append(element1);
            _encoder.Append(element2);
            _encoder.Append(element3);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, float data)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",f");
            _encoder.Append(data);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, float element1, float element2)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",ff");
            _encoder.Append(element1);
            _encoder.Append(element2);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, float element1, float element2, float element3)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",fff");
            _encoder.Append(element1);
            _encoder.Append(element2);
            _encoder.Append(element3);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, float element1, float element2, float element3, float element4)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",ffff");
            _encoder.Append(element1);
            _encoder.Append(element2);
            _encoder.Append(element3);
            _encoder.Append(element4);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, string data)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",s");
            _encoder.Append(data);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, byte[] blob)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",b");
            _encoder.Append(blob);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, params object[] data)
        {
            _encoder.Clear();
            _encoder.Append(address);

            char[] tagTypes = new char[data.Length + 1];
            tagTypes[0] = ',';

            for (int i = 0; i < data.Length; i++)
            {
                var element = data[i];
                int index = i + 1;
                if(element is int)
                {
                    tagTypes[index] = 'i';
                }
                else if (element is float)
                {
                    tagTypes[index] = 'f';
                }
                else if (element is string)
                {
                    tagTypes[index] = 's';
                }
                else if (element is byte[])
                {
                    tagTypes[index] = 'b';
                }
                else
                {
                    throw new ArgumentException("Unsupported data type.");
                }
            }
            _encoder.Append(new string(tagTypes));

            for (int i = 0; i < data.Length; i++)
            {
                var element = data[i];
                UnityEngine.Debug.Log(element);
                if(element is bool)
                {
                    _encoder.Append((bool)element ? 1 : 0);
                }
                else if (element is int)
                {
                    _encoder.Append((int)element);
                }
                else if (element is float)
                {
                    _encoder.Append((float)element);
                }
                else if (element is string)
                {
                    _encoder.Append((string)element);
                }
                else if (element is byte[])
                {
                    _encoder.Append((byte[])element);
                }
            }

            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        #endregion

        #region IDispose implementation

        bool _disposed;

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }

                _encoder = null;
            }
        }

        ~OscClient()
        {
            Dispose(false);
        }

        #endregion

        #region Private variables

        OscPacketEncoder _encoder = new OscPacketEncoder();
        Socket _socket;

        #endregion
    }
}
