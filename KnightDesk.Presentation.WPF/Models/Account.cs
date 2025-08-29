using System;
using System.ComponentModel;

namespace KnightDesk.Presentation.WPF.Models
{
    public class Account : INotifyPropertyChanged
    {
        private int _id;
        private string _username = string.Empty;
        private string _characterName = string.Empty;
        private string _password = string.Empty;
        private int _indexCharacter;
        private bool _isFavorite;
        private int _serverInfoId;
        private int _userId;
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

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string CharacterName
        {
            get => _characterName;
            set
            {
                _characterName = value;
                OnPropertyChanged(nameof(CharacterName));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public int IndexCharacter
        {
            get => _indexCharacter;
            set
            {
                _indexCharacter = value;
                OnPropertyChanged(nameof(IndexCharacter));
            }
        }

        public int ServerInfoId
        {
            get => _serverInfoId;
            set
            {
                _serverInfoId = value;
                OnPropertyChanged(nameof(ServerInfoId));
            }
        }

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                _isFavorite = value;
                OnPropertyChanged(nameof(IsFavorite));
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
