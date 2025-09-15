using KnightDesk.Presentation.WPF.Constants;
using KnightDesk.Presentation.WPF.DTOs;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace KnightDesk.Presentation.WPF.Services
{
    public interface ILoginServices
    {
        void LoginAsync(LoginRequestDTO loginRequest, Action<GeneralResponseDTO<UserDTO>> callback);
    }
    public class LoginServices : ILoginServices
    {
        private readonly string _baseUrl;

        public LoginServices()
        {
            _baseUrl = BaseApiUri.BaseApiUrl;
        }

        public void LoginAsync(LoginRequestDTO loginRequest, Action<GeneralResponseDTO<UserDTO>> callback)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(loginRequest);

                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/users/login", _baseUrl));
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
                            var result = serializer.Deserialize<GeneralResponseDTO<UserDTO>>(responseContent);
                            e.Result = result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    e.Result = new GeneralResponseDTO<UserDTO>
                    {
                        Code = (int)RESPONSE_CODE.InternalServerError,
                        Message = "Error: " + ex.Message,
                        Data = null
                    };
                }
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                callback((GeneralResponseDTO<UserDTO>)e.Result);
                worker.Dispose();
            };
            worker.RunWorkerAsync();
        }
    }
}
