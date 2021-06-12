// Created by Three Byte Intermedia, Inc. | project: PCNetListener |
// Created: 2021 06 12
// by Olaaf Rossi

using System;
using PCNetListener.Core.Models;

namespace PCNetListener.Core.Services
{
    public interface IPcNetworkListener
    {
        event EventHandler<NetworkMessagesEventArgs> MessageHit;
        int GetAppSettingsDataUdpPort();
        void ListenLoop(object state);
        void Run();
    }
}