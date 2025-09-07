using KnightDesk.Presentation.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;

namespace KnightDesk.Presentation.WPF.Services
{
    public class ServerInfoService : IServerInfoService
    {
        private readonly string _baseUrl;

        public ServerInfoService()
        {
            _baseUrl = Properties.Settings.Default.BaseUrl;
        }

        public void GetAllServersAsync(Action<List<ServerInfo>> callback)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/serverinfo", _baseUrl));
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            var servers = serializer.Deserialize<List<ServerInfo>>(responseContent);
                            callback(servers ?? new List<ServerInfo>());
                        }
                    }
                }
                catch (Exception)
                {
                    // Return empty list if error occurs
                    callback(new List<ServerInfo>());
                }
            }));
        }
    }
}
