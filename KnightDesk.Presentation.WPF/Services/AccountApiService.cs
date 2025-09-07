using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;
using KnightDesk.Presentation.WPF.DTOs;

namespace KnightDesk.Presentation.WPF.Services
{
    public class AccountApiService : IAccountApiService
    {
        private readonly string _baseUrl;

        public AccountApiService()
        {
            _baseUrl = Properties.Settings.Default.BaseUrl;
        }

        public void GetAllAccountsAsync(Action<GeneralResponseDTO<IEnumerable<AccountDTO>>> callback)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/accounts", _baseUrl));
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            var result = serializer.Deserialize<GeneralResponseDTO<IEnumerable<AccountDTO>>>(responseContent);
                            callback(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    callback(new GeneralResponseDTO<IEnumerable<AccountDTO>>
                    {
                        Code = 500,
                        Message = "Error: " + ex.Message,
                        Data = new List<AccountDTO>()
                    });
                }
            }));
        }

        public void GetAccountByIdAsync(int id, Action<GeneralResponseDTO<AccountDTO>> callback)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/accounts/{1}", _baseUrl, id));
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            var result = serializer.Deserialize<GeneralResponseDTO<AccountDTO>>(responseContent);
                            callback(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    callback(new GeneralResponseDTO<AccountDTO>
                    {
                        Code = 500,
                        Message = "Error: " + ex.Message,
                        Data = null
                    });
                }
            }));
        }

        public void GetAccountsByUserIdAsync(int userId, Action<GeneralResponseDTO<IEnumerable<AccountDTO>>> callback)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/accounts/user/{1}", _baseUrl, userId));
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            var result = serializer.Deserialize<GeneralResponseDTO<IEnumerable<AccountDTO>>>(responseContent);
                            callback(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    callback(new GeneralResponseDTO<IEnumerable<AccountDTO>>
                    {
                        Code = 500,
                        Message = "Error: " + ex.Message,
                        Data = new List<AccountDTO>()
                    });
                }
            }));
        }

        public void SearchAccountsAsync(string searchText, Action<GeneralResponseDTO<IEnumerable<AccountDTO>>> callback)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var encodedSearchText = Uri.EscapeDataString(searchText ?? string.Empty);
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/accounts/search?searchText={1}", _baseUrl, encodedSearchText));
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            var result = serializer.Deserialize<GeneralResponseDTO<IEnumerable<AccountDTO>>>(responseContent);
                            callback(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    callback(new GeneralResponseDTO<IEnumerable<AccountDTO>>
                    {
                        Code = 500,
                        Message = "Error: " + ex.Message,
                        Data = new List<AccountDTO>()
                    });
                }
            }));
        }

        public void AddAccountAsync(CreateAccountDTO account, Action<GeneralResponseDTO<AccountDTO>> callback)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(account);

                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/accounts/add-account", _baseUrl));
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
                            var result = serializer.Deserialize<GeneralResponseDTO<AccountDTO>>(responseContent);
                            callback(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    callback(new GeneralResponseDTO<AccountDTO>
                    {
                        Code = 500,
                        Message = "Error: " + ex.Message,
                        Data = null
                    });
                }
            }));
        }

        public void UpdateAccountAsync(UpdateAccountDTO account, Action<GeneralResponseDTO<AccountDTO>> callback)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(account);

                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/accounts/update-account", _baseUrl));
                    request.Method = "PUT";
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
                            var result = serializer.Deserialize<GeneralResponseDTO<AccountDTO>>(responseContent);
                            callback(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    callback(new GeneralResponseDTO<AccountDTO>
                    {
                        Code = 500,
                        Message = "Error: " + ex.Message,
                        Data = null
                    });
                }
            }));
        }

        public void DeleteAccountAsync(int id, Action<GeneralResponseDTO<bool>> callback)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/accounts/{1}", _baseUrl, id));
                    request.Method = "DELETE";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            var result = serializer.Deserialize<GeneralResponseDTO<bool>>(responseContent);
                            callback(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    callback(new GeneralResponseDTO<bool>
                    {
                        Code = 500,
                        Message = "Error: " + ex.Message,
                        Data = false
                    });
                }
            }));
        }

        public void ToggleFavoriteAsync(int id, Action<GeneralResponseDTO<bool>> callback)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/accounts/{1}/toggle-favorite", _baseUrl, id));
                    request.Method = "PUT";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";
                    request.ContentLength = 0;

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            var result = serializer.Deserialize<GeneralResponseDTO<bool>>(responseContent);
                            callback(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    callback(new GeneralResponseDTO<bool>
                    {
                        Code = 500,
                        Message = "Error: " + ex.Message,
                        Data = false
                    });
                }
            }));
        }

        public void GetFavoriteAccountsAsync(Action<GeneralResponseDTO<IEnumerable<AccountDTO>>> callback)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/accounts/favorites", _baseUrl));
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            var result = serializer.Deserialize<GeneralResponseDTO<IEnumerable<AccountDTO>>>(responseContent);
                            callback(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    callback(new GeneralResponseDTO<IEnumerable<AccountDTO>>
                    {
                        Code = 500,
                        Message = "Error: " + ex.Message,
                        Data = new List<AccountDTO>()
                    });
                }
            }));
        }

        public void GetFavoriteAccountsByUserIdAsync(int userId, Action<GeneralResponseDTO<IEnumerable<AccountDTO>>> callback)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/accounts/favorites/user/{1}", _baseUrl, userId));
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            var result = serializer.Deserialize<GeneralResponseDTO<IEnumerable<AccountDTO>>>(responseContent);
                            callback(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    callback(new GeneralResponseDTO<IEnumerable<AccountDTO>>
                    {
                        Code = 500,
                        Message = "Error: " + ex.Message,
                        Data = new List<AccountDTO>()
                    });
                }
            }));
        }
    }
}
