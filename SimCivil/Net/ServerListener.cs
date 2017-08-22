﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SimCivil;
using SimCivil.Net.Packets;

namespace SimCivil.Net
{
    /// <summary>
    /// Control TcpListener and a Listener Thread
    /// </summary>
    public class ServerListener : IServerListener, ITicker
    {
        TcpListener listener;

        /// <summary>
        /// Server host
        /// </summary>
        public IPAddress Host { get; }

        /// <summary>
        /// The port this listener listen to
        /// </summary>
        public int Port { get;  set; }
        /// <summary>
        /// Packets received from ServerClients and waiting for handling
        /// </summary>
        public Queue<Packet> PacketReadQueue { get; set; }
        /// <summary>
        /// Packets waiting for sending
        /// </summary>
        public Queue<Packet> PacketSendQueue { get; set; }
        /// <summary>
        /// ServerClients which are communicating with other clients
        /// </summary>
        public Dictionary<EndPoint, ServerClient> Clients { get; private set; } = new Dictionary<EndPoint, ServerClient>();

        /// <summary>
        /// The event triggered when a new ServerClient created
        /// </summary>
        public event EventHandler<ServerClient> NewConnectionEvent;
        /// <summary>
        /// The event triggered when a connection closed
        /// </summary>
        public event EventHandler<ServerClient> LostConnectionEvent;

        /// <summary>
        /// Construct a serverlistener
        /// </summary>
        /// <param name="host">server host</param>
        /// <param name="port">port to start listener</param>
        public ServerListener(IPAddress host, int port)
        {
            Host = host;
            Port = port;
            PacketSendQueue = new Queue<Packet>();
            PacketReadQueue = new Queue<Packet>();
        }
        /// <summary>
        /// Construct a serverlistener
        /// </summary>
        /// <param name="port">port to start listener</param>
        public ServerListener(int port)
        {
            Host = IPAddress.Loopback;
            Port = port;
            PacketSendQueue = new Queue<Packet>();
            PacketReadQueue = new Queue<Packet>();
        }

        /// <summary>
        /// Start the listener
        /// </summary>
        public void Start()
        {
            Task.Run(new Action(ListeningHandle));
            Task.Run(new Action(SendWorker));
        }

        /// <summary>
        /// Stop the client thread and remove it from storage
        /// </summary>
        /// <param name="client">the client to be removed</param>
        /// <returns></returns>
        public bool StopAndRemoveClient(ServerClient client)
        {
            if (Clients.ContainsValue(client))
            {
                EndPoint endPoint = client.TcpClt.Client.RemoteEndPoint;
                if (Clients.Remove(endPoint))
                {
                    client.Stop();
                    LostConnectionEvent?.Invoke(this, client);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Enqueue the packet and wait for sending
        /// </summary>
        /// <param name="pkt">the packet to send</param>
        public void SendPacket(Packet pkt)
        {
            lock (PacketSendQueue)
            {
                PacketSendQueue.Enqueue(pkt);
            }
        }

        /// <summary>
        /// Enqueue the packet and wait for handling
        /// </summary>
        /// <param name="pkt">the packet to handle</param>
        internal void PushPacket(Packet pkt)
        {
            lock (PacketReadQueue)
            {
                PacketReadQueue.Enqueue(pkt);
            }
        }

        private void SendWorker()
        {
            while (true)
            {
                Packet pkt = null;
                lock (PacketSendQueue)
                {
                    if (PacketSendQueue.Count > 0)
                        pkt = PacketSendQueue.Dequeue();
                }
                pkt?.Send();
                // TODO: WaitFor here?
                Thread.Yield();
            }
        }

        /// <summary>
        /// Listen to connection requests and create threads to handle the requests
        /// </summary>
        private void ListeningHandle()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, Port);
                listener.Start();
                Console.WriteLine("Listener started");

                while (true)
                {
                    TcpClient currentClient = listener.AcceptTcpClient();
                    ServerClient serverClient = new ServerClient(this, currentClient);
                    Clients.Add(serverClient.TcpClt.Client.RemoteEndPoint, serverClient);
                    NewConnectionEvent?.Invoke(this, serverClient);
                    serverClient.Start();
                    Console.WriteLine("A Connection Established");
                }
            }
            catch(ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Cannot start TcpListener: " + e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine("Listener exception: " + e.Message);
            }
            finally
            {
                listener?.Stop();
            }
        }

        /// <summary>
        /// Stop all clients thread and remove them from storage
        /// </summary>
        public void StopAndRemoveAllClient()
        {
            var clients = Clients.Values;
            foreach (var c in clients)
            {
                StopAndRemoveClient(c);
                LostConnectionEvent?.Invoke(this, c);
            }
        }

        /// <summary>
        /// Handle all packets in ReadQueue.
        /// </summary>
        /// <param name="tickCount">Total tick.</param>
        public void Update(int tickCount)
        {
            while (true)
            {
                Packet pkt = null;
                // To reduce lock time
                lock (PacketReadQueue)
                {
                    if (PacketReadQueue.Count > 0)
                        pkt = PacketReadQueue.Dequeue();
                    else
                        break;
                }
                if (pkt != null)
                {
                    pkt.Handle();
                    if (pkt is ResponsePacket)
                    {
                        // TODO: check if pkt is in wait list and callback.
                    }
                }

                foreach(var client in Clients.Values)
                {
                    client.TimeOutCheck();
                }
            }
        }
    }
}