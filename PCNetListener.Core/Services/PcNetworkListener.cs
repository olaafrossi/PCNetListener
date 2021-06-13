// Created by Three Byte Intermedia, Inc. | project: PCNetListener |
// Created: 2021 06 12
// by Olaaf Rossi

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PCNetListener.Core.Models;
using Serilog.Core;
using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace PCNetListener.Core.Services
{
    public class PcNetworkListener : IPcNetworkListener
    {
        private readonly ILogger<PcNetworkListener> _log;
        private readonly IConfiguration _config;

        /// <summary>
        ///     Constructor for the PC listener, injects dependencies
        /// </summary>
        /// <param name="log"></param>
        /// <param name="config"></param>
        public PcNetworkListener(ILogger<PcNetworkListener> log, IConfiguration config)
        {
            _log = log;
            _config = config;
            
            _log.LogInformation("The PC Network Listener Service has started from the constructor with log and config args", typeof(ILogEventEnricher));
            Run();
        }

        public string Test()
        {
            return "hello from the pc singleton";
        }

        public event EventHandler<NetworkMessagesEventArgs> MessageHit;

        public int GetAppSettingsDataUdpPort()
        {
            int udpPort = _config.GetValue<int>("UdpPort");
            int output = 0;

            if (udpPort >= 65535)
            {
                _log.LogWarning("UDP port setting is greater than 65535 (this is illegal). Setting to 16009 | your illegal port # was { udpPort }", udpPort);
                output = 16009;
                return output;
            }

            try
            {
                if (udpPort is 16009)
                {
                    output = udpPort;
                    _log.LogWarning("UDP Port is configured for {udpPort}", udpPort);
                }
                else if (udpPort is 0)
                {
                    _log.LogWarning("UDP port setting is 0 (this is illegal). Setting to 16009 | your illegal port # was { udpPort }", udpPort);
                    output = 16009;
                }
                else if (udpPort >= 1)
                {
                    _log.LogWarning("Parsed a valid (non-standard) UDP listener port from the appsettings.jsonfile | {udpPort}", udpPort);
                    output = udpPort;
                }
            }
            catch
            {
                _log.LogWarning("Failed to parse a valid UDP listener port from the appsettings.jsonfile Setting to 16009");
                output = 16009;
            }

            MessageHit?.Invoke(
                this,
                new NetworkMessagesEventArgs
                {
                    IncomingMessage = "No Message",
                    OutgoingMessage = "No Message",
                    RemoteIP = "Not Set",
                    RemotePort = "0",
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    UDPPort = output
                });

            return output;
        }

        public void ListenLoop(object state)
        {
            int portNumber = GetAppSettingsDataUdpPort();
            var udpClient = OpenUdpListener(portNumber);
            var udpEndPoint = UdpEndPoint(IPAddress.Any, 0);

            bool listening = true;

            byte[] dataBytes;

            while (listening)
            {
                dataBytes = udpClient.Receive(ref udpEndPoint);
                _log.LogWarning("Last Remote: {udpEndPoint.Address} on Port: {udpEndPoint.Port}", udpEndPoint.Address, udpEndPoint.Port);

                string stringIn = Encoding.ASCII.GetString(dataBytes); // Incoming commands must be received as a single packet.
                stringIn = stringIn.ToUpperInvariant(); // format the string to upper case for matching

                //Parse messages separated by cr
                int delimPos = stringIn.IndexOf("\r");
                while (delimPos >= 0)
                {
                    string message = stringIn.Substring(0, delimPos + 1).Trim();
                    stringIn = stringIn.Remove(0, delimPos + 1); //remove the message
                    delimPos = stringIn.IndexOf("\r");

                    _log.LogWarning("Incoming Message: {message}", message);

                    if (message is "EXIT")
                    {
                        listening = false;
                        _log.LogWarning("Stopping the listen loop by your command");
                        MessageHit?.Invoke(
                            this,
                            new NetworkMessagesEventArgs
                            {
                                IncomingMessage = message,
                                UDPPort = portNumber,
                                OutgoingMessage = string.Empty,
                                RemoteIP = udpEndPoint.Address.ToString(),
                                RemotePort = udpEndPoint.Port.ToString(),
                                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                            });
                    }
                    else if (message is "PING")
                    {
                        string responseString = "PONG\r";
                        byte[] sendBytes = Encoding.ASCII.GetBytes(responseString);
                        udpClient.Send(sendBytes, sendBytes.Length, udpEndPoint);
                        _log.LogWarning("Sent: {responseString}", responseString);
                        MessageHit?.Invoke(
                            this,
                            new NetworkMessagesEventArgs
                            {
                                IncomingMessage = message,
                                UDPPort = portNumber,
                                OutgoingMessage = "PONG",
                                RemoteIP = udpEndPoint.Address.ToString(),
                                RemotePort = udpEndPoint.Port.ToString(),
                                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                            });
                    }
                    else if (message is "REBOOT" || message is "RESTART")
                    {
                        _log.LogWarning("Rebooting PC- in 5 seconds");
                        MessageHit?.Invoke(
                            this,
                            new NetworkMessagesEventArgs
                            {
                                IncomingMessage = message,
                                UDPPort = portNumber,
                                OutgoingMessage = string.Empty,
                                RemoteIP = udpEndPoint.Address.ToString(),
                                RemotePort = udpEndPoint.Port.ToString(),
                                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                            });
                        Thread.Sleep(5000);
                        Process.Start("shutdown", "/r /f /t 3 /c \"Reboot Triggered\" /d p:0:0");
                    }
                    else if (message is "SHUTDOWN")
                    {
                        _log.LogWarning("Shutting Down PC- in 5 seconds");
                        MessageHit?.Invoke(
                            this,
                            new NetworkMessagesEventArgs
                            {
                                IncomingMessage = message,
                                UDPPort = portNumber,
                                OutgoingMessage = string.Empty,
                                RemoteIP = udpEndPoint.Address.ToString(),
                                RemotePort = udpEndPoint.Port.ToString(),
                                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                            });
                        Thread.Sleep(5000);
                        Process.Start("shutdown", "/s /f /t 3 /c \"Shutdown Triggered\" /d p:0:0");
                    }
                    else if (message is "SLEEP")
                    {
                        _log.LogWarning("Sleeping PC- in 5 seconds");
                        MessageHit?.Invoke(
                            this,
                            new NetworkMessagesEventArgs
                            {
                                IncomingMessage = message,
                                UDPPort = portNumber,
                                OutgoingMessage = string.Empty,
                                RemoteIP = udpEndPoint.Address.ToString(),
                                RemotePort = udpEndPoint.Port.ToString(),
                                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                            });
                        Thread.Sleep(5000);
                        Process.Start("rundll32.exe", "powrprof.dll,SetSuspendState 0,1,0");
                    }
                }
            }
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem(ListenLoop);
            _log.LogWarning("The PC Network Listener Server Run Loop has started");
        }

        private UdpClient OpenUdpListener(int port)
        {
            UdpClient udpSender = new UdpClient(port);
            return udpSender;
        }

        private IPEndPoint UdpEndPoint(IPAddress address, int port)
        {
            IPEndPoint udpEndPoint = new IPEndPoint(address, port);
            return udpEndPoint;
        }
    }
}