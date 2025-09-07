using KnightDesk.Presentation.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels.Pages
{
    public class ServerConfigPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ServerInfo> _servers;
        private ObservableCollection<ServerInfo> _filteredServers;
        private ServerInfo _currentServer;
        private string _searchText = string.Empty;
        private bool _isEditMode = false;

        public ServerConfigPageViewModel()
        {
            Servers = new ObservableCollection<ServerInfo>();
            FilteredServers = new ObservableCollection<ServerInfo>();
            CurrentServer = new ServerInfo();

            // Initialize commands - .NET 3.5 compatible
            SearchCommand = new RelayCommand(new Action(ExecuteSearch));
            AddNewServerCommand = new RelayCommand(new Action(ExecuteAddNew));
            EditServerCommand = new RelayCommand<ServerInfo>(new Action<ServerInfo>(ExecuteEdit));
            DeleteServerCommand = new RelayCommand<ServerInfo>(new Action<ServerInfo>(ExecuteDelete));
            SaveServerCommand = new RelayCommand(new Action(ExecuteSave), new Func<bool>(CanExecuteSave));
            CancelCommand = new RelayCommand(new Action(ExecuteCancel));

            LoadData();
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
                FilterServers();
            }
        }

        public string FormTitle 
        { 
            get { return _isEditMode ? "Edit Server" : "Add New Server"; } 
        }

        public string SaveButtonText 
        { 
            get { return _isEditMode ? "Update" : "Save"; } 
        }

        #endregion

        #region Commands

        public ICommand SearchCommand { get; private set; }
        public ICommand AddNewServerCommand { get; private set; }
        public ICommand EditServerCommand { get; private set; }
        public ICommand DeleteServerCommand { get; private set; }
        public ICommand SaveServerCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        #endregion

        #region Command Implementations

        private void ExecuteSearch()
        {
            FilterServers();
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
                    Name = server.Name,
                    ServerNo = server.ServerNo,
                    Description = server.Description,
                    CreatedAt = server.CreatedAt,
                    UpdatedAt = server.UpdatedAt,
                    IsDeleted = server.IsDeleted,
                    AccountCount = server.AccountCount
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
                    try
                    {
                        Servers.Remove(server);
                        FilterServers();
                        MessageBox.Show("Server deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Error deleting server: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ExecuteSave()
        {
            try
            {
                if (_isEditMode)
                {
                    ServerInfo existingServer = Servers.FirstOrDefault(delegate(ServerInfo s) { return s.Id == CurrentServer.Id; });
                    if (existingServer != null)
                    {
                        int index = Servers.IndexOf(existingServer);
                        CurrentServer.UpdatedAt = DateTime.Now;
                        Servers[index] = CurrentServer;
                    }
                    MessageBox.Show("Server updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    CurrentServer.Id = Servers.Count > 0 ? Servers.Max(delegate(ServerInfo s) { return s.Id; }) + 1 : 1;
                    CurrentServer.CreatedAt = DateTime.Now;
                    CurrentServer.UpdatedAt = DateTime.Now;
                    CurrentServer.AccountCount = 0;
                    Servers.Add(CurrentServer);
                    MessageBox.Show("Server created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                FilterServers();
                ExecuteCancel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error saving server: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteSave()
        {
            return CurrentServer != null && 
                   !string.IsNullOrEmpty(CurrentServer.Name) && 
                   CurrentServer.ServerNo > 0;
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

        private void LoadData()
        {
            try
            {
                // Mock server data - replace with actual API call when available
                Servers.Clear();
                Servers.Add(new ServerInfo { Id = 1, Name = "Game Server 1", ServerNo = 1, Description = "Main game server for beginners", AccountCount = 5, CreatedAt = DateTime.Now.AddDays(-30) });
                Servers.Add(new ServerInfo { Id = 2, Name = "Game Server 2", ServerNo = 2, Description = "Advanced game server", AccountCount = 3, CreatedAt = DateTime.Now.AddDays(-20) });
                Servers.Add(new ServerInfo { Id = 3, Name = "Game Server 3", ServerNo = 3, Description = "PvP focused server", AccountCount = 8, CreatedAt = DateTime.Now.AddDays(-10) });

                FilterServers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error loading data: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterServers()
        {
            FilteredServers.Clear();
            
            if (string.IsNullOrEmpty(SearchText))
            {
                foreach (ServerInfo server in Servers)
                {
                    FilteredServers.Add(server);
                }
            }
            else
            {
                foreach (ServerInfo server in Servers)
                {
                    if (server.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        server.ServerNo.ToString().IndexOf(SearchText) >= 0)
                    {
                        FilteredServers.Add(server);
                    }
                }
            }
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