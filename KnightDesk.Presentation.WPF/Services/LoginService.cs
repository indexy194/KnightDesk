using KnightDesk.Presentation.WPF.DTOs;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;

namespace KnightDesk.Presentation.WPF.Services
{
    public class LoginService : ILoginService
    {
        private readonly string _baseUrl;

        public LoginService()
        {
            _baseUrl = Properties.Settings.Default.BaseUrl;
        }

        public void LoginAsync(string username, string password, Action<GeneralResponseDTO<LoginResponseDTO>> callback)
        {
            // Use ThreadPool for background operation in .NET 3.5
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var loginRequest = new
                    {
                        Username = username,
                        Password = password
                    };

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(loginRequest);

                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/users/login", _baseUrl));
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(json);
                    }

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var apiResponse = serializer.Deserialize<GeneralResponseDTO<UserDTO>>(responseContent);

                                if (apiResponse != null && apiResponse.Code == 200)
                                {
                                    callback(new GeneralResponseDTO<LoginResponseDTO>
                                    {
                                        Message = "Đăng nhập thành công!",
                                        Data = new LoginResponseDTO
                                        {
                                            IsSuccess = true,
                                            Id = apiResponse.Data.Id,
                                            Username = apiResponse.Data.Username,
                                            IPAddress = apiResponse.Data.IPAddress,
                                        }
                                    });
                                }
                                else
                                {
                                    callback(new GeneralResponseDTO<LoginResponseDTO>
                                    {
                                        Message = apiResponse != null ? apiResponse.Message : "Đăng nhập thất bại!",
                                        Data = new LoginResponseDTO
                                        {
                                            IsSuccess = false
                                        }
                                    });
                                }
                            }
                            else
                            {
                                callback(new GeneralResponseDTO<LoginResponseDTO>
                                {
                                    Message = "Lỗi kết nối tới server!",
                                    Data = new LoginResponseDTO
                                    {
                                        IsSuccess = false
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    callback(new GeneralResponseDTO<LoginResponseDTO>
                    {
                        Message = string.Format("Error: {0}", ex.Message),
                        Data = new LoginResponseDTO
                        {
                            IsSuccess = false
                        }
                    });
                }
            }));
        }
    }
}
