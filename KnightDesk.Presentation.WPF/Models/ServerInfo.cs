using System;
using System.ComponentModel;

namespace KnightDesk.Presentation.WPF.Models
{
    public class ServerInfo : INotifyPropertyChanged
    {
        private int _id;
        private int _serverNo;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private int _accountCount;
        private DateTime? _createdAt;
        private DateTime? _updatedAt;
        private bool _isDeleted;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public int ServerNo
        {
            get => _serverNo;
            set
            {
                _serverNo = value;
                OnPropertyChanged(nameof(ServerNo));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public DateTime? CreatedAt
        {
            get => _createdAt;
            set
            {
                _createdAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }

        public DateTime? UpdatedAt
        {
            get => _updatedAt;
            set
            {
                _updatedAt = value;
                OnPropertyChanged(nameof(UpdatedAt));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public int AccountCount
        {
            get => _accountCount;
            set
            {
                _accountCount = value;
                OnPropertyChanged(nameof(AccountCount));
            }
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set
            {
                _isDeleted = value;
                OnPropertyChanged(nameof(IsDeleted));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
