using System;

namespace KnightDesk.Presentation.WPF.Models
{
    public class User : BaseModel
    {
        private int _id;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _ipAddress = string.Empty;
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

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value, nameof(Password));
        }

        public string IPAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value, nameof(IPAddress));
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
