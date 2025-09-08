using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using KnightDesk.Presentation.WPF.DTOs;

namespace KnightDesk.Presentation.WPF.Services
{
    /// <summary>
    /// Service for checking server connectivity (network-level connection, not API calls)
    /// </summary>
    public static class ServerConnectionServices
    {
        /// <summary>
        /// Check if server is reachable via network ping
        /// </summary>
        /// <param name="request">Server connection request DTO</param>
        /// <returns>Server connection response DTO</returns>
        public static ServerConnectionResponseDTO CheckServerConnection(ServerConnectionRequestDTO request)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                // Extract hostname/IP from BaseUrl
                var uri = new Uri(request.BaseUrl);
                var hostName = uri.Host;

                // Use Ping to check network connectivity
                using (var ping = new Ping())
                {
                    var pingOptions = new PingOptions(64, true);
                    var buffer = new byte[32]; // Standard ping buffer size
                    
                    var pingReply = ping.Send(hostName, request.TimeoutMs, buffer, pingOptions);
                    stopwatch.Stop();

                    if (pingReply.Status == IPStatus.Success)
                    {
                        return new ServerConnectionResponseDTO
                        {
                            IsReachable = true,
                            Message = string.Format("Server {0} is reachable via network", hostName),
                            ResponseTimeMs = (int)pingReply.RoundtripTime
                        };
                    }
                    else
                    {
                        return new ServerConnectionResponseDTO
                        {
                            IsReachable = false,
                            Message = string.Format("Server {0} is not reachable: {1}", hostName, pingReply.Status),
                            ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return new ServerConnectionResponseDTO
                {
                    IsReachable = false,
                    Message = string.Format("Network connection error: {0}", ex.Message),
                    ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds
                };
            }
        }

        /// <summary>
        /// Check server connectivity asynchronously using BackgroundWorker
        /// </summary>
        /// <param name="request">Server connection request DTO</param>
        /// <param name="callback">Callback with ServerConnectionResponseDTO result</param>
        public static void CheckServerAsync(ServerConnectionRequestDTO request, Action<ServerConnectionResponseDTO> callback)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                e.Result = CheckServerConnection(request);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                callback((ServerConnectionResponseDTO)e.Result);
                worker.Dispose();
            };
            worker.RunWorkerAsync();
        }
    }
}
