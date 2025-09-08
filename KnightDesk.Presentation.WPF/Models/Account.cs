using System;

namespace KnightDesk.Presentation.WPF.Models
{
    public class Account : BaseModel
    {
        private int _id;
        private string _username = string.Empty;
        private string _characterName = string.Empty;
        private string _password = string.Empty;
        private int _indexCharacter;
        private bool _isFavorite;
        private int _serverInfoId;
        private int _indexServer;
        private string _serverName = string.Empty;
        private int _userId;
        private DateTime? _createdAt;
        private DateTime? _updatedAt;
        private bool _isDeleted;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value, nameof(Id));
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value, nameof(Username));
        }

        public string CharacterName
        {
            get => _characterName;
            set => SetProperty(ref _characterName, value, nameof(CharacterName));
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value, nameof(Password));
        }

        public int IndexCharacter
        {
            get => _indexCharacter;
            set => SetProperty(ref _indexCharacter, value, nameof(IndexCharacter));
        }

        public int ServerInfoId
        {
            get => _serverInfoId;
            set => SetProperty(ref _serverInfoId, value, nameof(ServerInfoId));
        }
        public int IndexServer
        {
            get => _indexServer;
            set => SetProperty(ref _indexServer, value, nameof(IndexServer));
        }
        public string ServerName
        {
            get => _serverName;
            set => SetProperty(ref _serverName, value, nameof(ServerName));
        }

        public int UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value, nameof(UserId));
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value, nameof(IsFavorite));
        }

        public DateTime? CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value, nameof(CreatedAt));
        }

        public DateTime? UpdatedAt
        {
            get => _updatedAt;
            set => SetProperty(ref _updatedAt, value, nameof(UpdatedAt));
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set => SetProperty(ref _isDeleted, value, nameof(IsDeleted));
        }
    }
}
