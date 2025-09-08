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
                var httpRequest = WebRequest.Create(request.BaseUrl + "/health");
                httpRequest.Method = "GET";
                httpRequest.Timeout = request.TimeoutMs;


                using (var response = (HttpWebResponse)httpRequest.GetResponse())
                {
                    stopwatch.Stop();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return new ServerConnectionResponseDTO
                        {
                            IsReachable = true,
                            Message = "Server is online",
                            ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds
                        };
                    }
                }
                return new ServerConnectionResponseDTO
                {
                    IsReachable = false,
                    Message = "Server returned non-200",
                    ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return new ServerConnectionResponseDTO
                {
                    IsReachable = false,
                    Message = $"Server not reachable: {ex.Message}",
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
