using KnightDesk.Presentation.WPF.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace KnightDesk.Presentation.WPF.Services
{
    public interface IServerInfoServices
    {
        void GetAllServersAsync(Action<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> callback);
        void GetServerByIdAsync(int id, Action<GeneralResponseDTO<ServerInfoDTO>> callback);
        void CreateServerAsync(CreateServerInfoDTO serverInfo, Action<GeneralResponseDTO<ServerInfoDTO>> callback);
        void UpdateServerAsync(UpdateServerInfoDTO serverInfo, Action<GeneralResponseDTO<ServerInfoDTO>> callback);
        void DeleteServerAsync(int id, Action<GeneralResponseDTO<string>> callback);
        void SearchServersAsync(string searchText, Action<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> callback);
    }
    public class ServerInfoServices : IServerInfoServices
    {
        private readonly string _baseUrl;

        public ServerInfoServices()
        {
            _baseUrl = Properties.Settings.Default.BaseUrl;
        }

        public void GetAllServersAsync(Action<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> callback)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
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
                            var result = serializer.Deserialize<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>>(responseContent);
                            e.Result = result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorResult = new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                    {
                        Code = (int)RESPONSE_CODE.InternalServerError,
                        Message = ex.Message,
                        Data = new List<ServerInfoDTO>()
                    };
                    e.Result = errorResult;
                }
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                callback((GeneralResponseDTO<IEnumerable<ServerInfoDTO>>)e.Result);
                worker.Dispose();
            };
            worker.RunWorkerAsync();
        }

        public void GetServerByIdAsync(int id, Action<GeneralResponseDTO<ServerInfoDTO>> callback)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/serverinfo/{1}", _baseUrl, id));
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            var server = serializer.Deserialize<ServerInfoDTO>(responseContent);
                            
                            var result = new GeneralResponseDTO<ServerInfoDTO>
                            {
                                Code = (int)response.StatusCode,
                                Message = "Success",
                                Data = server
                            };
                            e.Result = result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorResult = new GeneralResponseDTO<ServerInfoDTO>
                    {
                        Code = 500,
                        Message = ex.Message,
                        Data = null
                    };
                    e.Result = errorResult;
                }
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                callback((GeneralResponseDTO<ServerInfoDTO>)e.Result);
                worker.Dispose();
            };
            worker.RunWorkerAsync();
        }

        public void CreateServerAsync(CreateServerInfoDTO serverInfo, Action<GeneralResponseDTO<ServerInfoDTO>> callback)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/serverinfo/add-server", _baseUrl));
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    var serializer = new JavaScriptSerializer();
                    var jsonData = serializer.Serialize(serverInfo);
                    var data = Encoding.UTF8.GetBytes(jsonData);

                    request.ContentLength = data.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var createdServer = serializer.Deserialize<ServerInfoDTO>(responseContent);
                            
                            var result = new GeneralResponseDTO<ServerInfoDTO>
                            {
                                Code = (int)response.StatusCode,
                                Message = "Server created successfully",
                                Data = createdServer
                            };
                            e.Result = result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorResult = new GeneralResponseDTO<ServerInfoDTO>
                    {
                        Code = 500,
                        Message = ex.Message,
                        Data = null
                    };
                    e.Result = errorResult;
                }
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                callback((GeneralResponseDTO<ServerInfoDTO>)e.Result);
                worker.Dispose();
            };
            worker.RunWorkerAsync();
        }

        public void UpdateServerAsync(UpdateServerInfoDTO serverInfo, Action<GeneralResponseDTO<ServerInfoDTO>> callback)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/serverinfo/update-server", _baseUrl));
                    request.Method = "PUT";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    var serializer = new JavaScriptSerializer();
                    var jsonData = serializer.Serialize(serverInfo);
                    var data = Encoding.UTF8.GetBytes(jsonData);

                    request.ContentLength = data.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var updatedServer = serializer.Deserialize<ServerInfoDTO>(responseContent);
                            
                            var result = new GeneralResponseDTO<ServerInfoDTO>
                            {
                                Code = (int)response.StatusCode,
                                Message = "Server updated successfully",
                                Data = updatedServer
                            };
                            e.Result = result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorResult = new GeneralResponseDTO<ServerInfoDTO>
                    {
                        Code = 500,
                        Message = ex.Message,
                        Data = null
                    };
                    e.Result = errorResult;
                }
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                callback((GeneralResponseDTO<ServerInfoDTO>)e.Result);
                worker.Dispose();
            };
            worker.RunWorkerAsync();
        }

        public void DeleteServerAsync(int id, Action<GeneralResponseDTO<string>> callback)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/serverinfo/{1}", _baseUrl, id));
                    request.Method = "DELETE";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            
                            var result = new GeneralResponseDTO<string>
                            {
                                Code = (int)response.StatusCode,
                                Message = "Server deleted successfully",
                                Data = responseContent
                            };
                            e.Result = result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorResult = new GeneralResponseDTO<string>
                    {
                        Code = 500,
                        Message = ex.Message,
                        Data = null
                    };
                    e.Result = errorResult;
                }
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                callback((GeneralResponseDTO<string>)e.Result);
                worker.Dispose();
            };
            worker.RunWorkerAsync();
        }

        public void SearchServersAsync(string searchText, Action<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> callback)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                try
                {
                    var encodedSearchText = Uri.EscapeDataString(searchText ?? string.Empty);
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/serverinfo/search?searchText={1}", _baseUrl, encodedSearchText));
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            var result = serializer.Deserialize<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>>(responseContent);
                            e.Result = result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorResult = new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                    {
                        Code = 500,
                        Message = ex.Message,
                        Data = new List<ServerInfoDTO>()
                    };
                    e.Result = errorResult;
                }
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                callback((GeneralResponseDTO<IEnumerable<ServerInfoDTO>>)e.Result);
                worker.Dispose();
            };
            worker.RunWorkerAsync();
        }
    }
}
