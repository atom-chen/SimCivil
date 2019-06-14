﻿// Copyright (c) 2017 TPDT
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
// SimCivil - SimCivil.Rpc - SimpleTimeoutDaemon.cs
// Create Date: 2019/05/08
// Update Date: 2019/05/19

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DotNetty.Transport.Channels;

using Microsoft.Extensions.Logging;

using static System.Diagnostics.Debug;

namespace SimCivil.Rpc.Timeout
{
    /// <summary>
    /// One clock hand algorithm timeout daemon
    /// </summary>
    public class DummyTimeoutDaemon : ITimeoutDaemon
    {
        private readonly int                              _waitTime;
        public event EventHandler<ClientTimeoutEventArgs> ClientTimeout;

        protected List<KeyValuePair<IChannel, bool>> ClientsToBeRemoved =
            new List<KeyValuePair<IChannel, bool>>();

        protected readonly RpcServer               Server;
        protected readonly ILogger                 Logger;
        protected          Task                    Daemon;
        protected          CancellationTokenSource CancelSrc;
        public             bool                    IsRunning { get; private set; }

        private readonly ConcurrentDictionary<IChannel, bool> _receiveCounts =
            new ConcurrentDictionary<IChannel, bool>();

        public DummyTimeoutDaemon(RpcServer server, ILogger logger, int waitTime)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Notify a Packet has received for a chennel
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="request">The RpcRequest.</param>
        /// <returns>Whether the notifed-state of channel changed to true</returns>
        public virtual bool NotifyPacketReceived(IChannel channel, RpcRequest request)
        {
            Logger.LogInformation($"Notify received: {channel.RemoteAddress}");
            return true;
        }

        public virtual void RegisterChannel(IChannel channel)
        {
            // Dummy
        }

        public virtual void UnregisterChannel(IChannel channel)
        {
            // Dummy
        }

        public virtual void Start()
        {
            // Dummy
        }

        protected virtual void DaemonRun()
        {
            // Dummy
        }

        public virtual void Stop()
        {
            // Dummy
        }

        public void Dispose()
        {
            // Dummy
        }
    }
}