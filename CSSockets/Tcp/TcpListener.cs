﻿using System;
using System.Net;
using System.Net.Sockets;

namespace CSSockets.Tcp
{
    public delegate void TcpListenerIncomingHandler(TcpSocket newConnection);
    sealed public class TcpListener
    {
        public Socket Base { get; private set; } = null;
        public bool Listening { get; private set; } = false;
        private EndPoint BindEndPoint { get; set; } = null;
        public int BacklogSize { get; set; } = 511;

        public event TcpListenerIncomingHandler OnConnection;

        public TcpListener() { }
        private void CreateBase()
        {
            if (Base != null) Base.Dispose();
            Base = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public void Bind(EndPoint endPoint)
        {
            if (Listening) throw new InvalidOperationException("TcpSocketListener can be bound only when it's not listening");
            BindEndPoint = endPoint;
        }
        public void Start()
        {
            if (Listening)
                throw new InvalidOperationException("TcpSocketListener is already listening");
            Listening = true;
            CreateBase();
            Base.Bind(BindEndPoint);
            Base.Listen(BacklogSize);
            ListenIteration();
        }
        public void Stop()
        {
            if (!Listening)
                throw new InvalidOperationException("TcpSocketListener isn't listening");
            Base.Close();
        }

        private void ListenIteration() => Base.BeginAccept(OnNewSocket, null);
        private void OnNewSocket(IAsyncResult ar)
        {
            if (!Listening) return;
            TcpSocket socket = new TcpSocket(Base.EndAccept(ar));
            OnConnection?.Invoke(socket);
            ListenIteration();
        }
    }
}
