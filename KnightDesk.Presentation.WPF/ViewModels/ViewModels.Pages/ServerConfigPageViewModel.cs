using KnightDesk.Presentation.WPF.DTOs;
using KnightDesk.Presentation.WPF.Models;
using KnightDesk.Presentation.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels.Pages
{
    public class ServerConfigPageViewModel : INotifyPropertyChanged
    {
        private readonly IServerInfoServices _serverInfoService;
        private ObservableCollection<ServerInfo> _servers; // auto refresh UI on changes
        private ObservableCollection<ServerInfo> _filteredServers;
        private ServerInfo _currentServer;
        private string _searchText = string.Empty;
        private bool _isEditMode = false;
        private bool _isLoading = false;

        public ServerConfigPageViewModel()
        {
            _serverInfoService = new ServerInfoServices();
            _servers = new ObservableCollection<ServerInfo>();
            _filteredServers = new ObservableCollection<ServerInfo>();
            CurrentServer = new ServerInfo();

            // Initialize commands - .NET 3.5 compatible
            AddNewServerCommand = new RelayCommand(ExecuteAddNew);
            EditServerCommand = new RelayCommand<ServerInfo>(ExecuteEdit);
            DeleteServerCommand = new RelayCommand<ServerInfo>(ExecuteDelete);
            SaveServerCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);

            LoadServers();
        }

        #region Properties

        public ObservableCollection<ServerInfo> Servers
        {
            get { return _servers; }
            set
            {
                _servers = value;
                OnPropertyChanged("Servers");
            }
        }

        public ObservableCollection<ServerInfo> FilteredServers
        {
            get { return _filteredServers; }
            set
            {
                _filteredServers = value;
                OnPropertyChanged("FilteredServers");
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public ServerInfo CurrentServer
        {
            get { return _currentServer; }
            set
            {
                _currentServer = value;
                OnPropertyChanged("CurrentServer");
                OnPropertyChanged("FormTitle");
                OnPropertyChanged("SaveButtonText");
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
                ExecuteSearch();
            }
        }

        public string FormTitle
        {
            get { return _isEditMode ? "Edit" : "Add"; }
        }

        public string SaveButtonText
        {
            get { return _isEditMode ? "Update" : "Save"; }
        }

        #endregion

        #region Commands

        public ICommand AddNewServerCommand { get; private set; }
        public ICommand EditServerCommand { get; private set; }
        public ICommand DeleteServerCommand { get; private set; }
        public ICommand SaveServerCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        #endregion

        #region Command Implementations

        private void ExecuteSearch()
        {
            if (!string.IsNullOrEmpty(_searchText))
            {
                // Use API search for better performance
                _serverInfoService.SearchServersAsync(_searchText, new Action<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>>(result =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (result.Code == (int)RESPONSE_CODE.OK && result.Data != null)
                        {
                            _servers.Clear();
                            _filteredServers.Clear();
                            foreach (var entry in result.Data)
                            {
                                var server = new ServerInfo
                                {
                                    Id = entry.Id,
                                    IndexServer = entry.IndexServer,
                                    Name = entry.Name,
                                    AccountCount = entry.Accounts.Count,
                                };
                                _servers.Add(server);
                                _filteredServers.Add(server);
                            }
                        }
                    }));
                }));
            }
            else
            {
                LoadServers(); // Reset to full list
            }
        }

        private void ExecuteAddNew()
        {
            _isEditMode = false;
            CurrentServer = new ServerInfo();
            OnPropertyChanged("FormTitle");
            OnPropertyChanged("SaveButtonText");
        }

        private void ExecuteEdit(ServerInfo server)
        {
            if (server != null)
            {
                _isEditMode = true;
                CurrentServer = new ServerInfo
                {
                    Id = server.Id,
                    IndexServer = server.IndexServer,
                    Name = server.Name,
                };

                OnPropertyChanged("FormTitle");
                OnPropertyChanged("SaveButtonText");
            }
        }

        private void ExecuteDelete(ServerInfo server)
        {
            if (server != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    string.Format("Are you sure you want to delete the server '{0}'?", server.Name),
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    IsLoading = true;

                    ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
                    {
                        _serverInfoService.DeleteServerAsync(server.Id, new Action<GeneralResponseDTO<string>>(deleteResult =>
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                IsLoading = false;

                                if (deleteResult.Code == (int)RESPONSE_CODE.OK)
                                {
                                    if (!string.IsNullOrEmpty(_searchText))
                                    {
                                        ExecuteSearch();
                                    }
                                    else
                                    {
                                        LoadServers();
                                    }
                                    MessageBox.Show("Server deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("Error deleting server: {0}", deleteResult.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }));
                        }));
                    }));
                }
            }
        }

        private void ExecuteSave()
        {
            if (CurrentServer == null || string.IsNullOrEmpty(CurrentServer.Name) || CurrentServer.IndexServer <= 0)
            {
                MessageBox.Show("Please fill all required fields!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsLoading = true;


            if (_isEditMode)
            {
                var updateDto = new UpdateServerInfoDTO
                {
                    Id = CurrentServer.Id,
                    IndexServer = CurrentServer.IndexServer,
                    Name = CurrentServer.Name,
                };
                _serverInfoService.UpdateServerAsync(updateDto, new Action<GeneralResponseDTO<ServerInfoDTO>>(result =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        IsLoading = false;

                        if (result.Code == (int)RESPONSE_CODE.OK)
                        {
                            if (!string.IsNullOrEmpty(_searchText))
                            {
                                ExecuteSearch();
                            }
                            else
                            {
                                LoadServers();
                            }
                            ExecuteCancel();
                            MessageBox.Show("Account updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show(string.Format("Error updating server: {0}", result.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }));
                }));
            }
            else
            {
                var createDto = new CreateServerInfoDTO
                {
                    IndexServer = CurrentServer.IndexServer,
                    Name = CurrentServer.Name,
                };
                CurrentServer.AccountCount = 0;

                _serverInfoService.CreateServerAsync(createDto, new Action<GeneralResponseDTO<ServerInfoDTO>>(result =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        IsLoading = false;

                        if (result.Code == (int)RESPONSE_CODE.Created || result.Code == (int)RESPONSE_CODE.OK)
                        {
                            if (!string.IsNullOrEmpty(_searchText))
                            {
                                ExecuteSearch();
                            }
                            else
                            {
                                LoadServers();
                            }
                            ExecuteCancel();
                            MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show(string.Format("Error creating server: {0}", result.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }));
                }));
            }
        }

        private bool CanExecuteSave()
        {
            return CurrentServer != null &&
                   !string.IsNullOrEmpty(CurrentServer.Name) &&
                   CurrentServer.IndexServer > 0 &&
                   !IsLoading;
        }

        private void ExecuteCancel()
        {
            _isEditMode = false;
            CurrentServer = new ServerInfo();
            OnPropertyChanged("FormTitle");
            OnPropertyChanged("SaveButtonText");
        }

        #endregion

        #region Private Methods

        private void LoadServers()
        {
            IsLoading = true;


            _serverInfoService.GetAllServersAsync(new Action<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>>(result =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    IsLoading = false;

                    if (result.Code == (int)RESPONSE_CODE.OK && result.Data != null)
                    {
                        _servers.Clear();
                        _filteredServers.Clear();
                        foreach (var entry in result.Data)
                        {
                            var server = new ServerInfo
                            {
                                Id = entry.Id,
                                IndexServer = entry.IndexServer,
                                Name = entry.Name,
                                AccountCount = entry.Accounts.Count,
                            };
                            _servers.Add(server);
                            _filteredServers.Add(server);
                        }
                        //FilteredServers = _servers;
                    }
                }));
            }));

        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}