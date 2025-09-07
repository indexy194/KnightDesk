using System;
using System.Net;

namespace KnightDesk.Presentation.WPF.Services
{
    /// <summary>
    /// Service for checking server connectivity
    /// </summary>
    public static class ServerConnectionService
    {
        /// <summary>
        /// Check if server is reachable
        /// </summary>
        /// <param name="baseUrl">Server base URL</param>
        /// <param name="timeoutMs">Timeout in milliseconds (default 5000)</param>
        /// <returns>True if server is reachable, false otherwise</returns>
        public static bool IsServerReachable(string baseUrl, int timeoutMs = 5000)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/serverinfo", baseUrl));
                request.Method = "GET";
                request.Timeout = timeoutMs;
                request.ReadWriteTimeout = timeoutMs;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check server connectivity asynchronously using callback
        /// </summary>
        /// <param name="baseUrl">Server base URL</param>
        /// <param name="callback">Callback with result (true/false)</param>
        /// <param name="timeoutMs">Timeout in milliseconds</param>
        public static void CheckServerAsync(string baseUrl, Action<bool> callback, int timeoutMs = 5000)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(state =>
            {
                bool isReachable = IsServerReachable(baseUrl, timeoutMs);
                callback(isReachable);
            }));
        }
    }
}
