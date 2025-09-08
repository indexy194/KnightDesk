using System;
using System.ComponentModel;

namespace KnightDesk.Presentation.WPF.Models
{
    public class ServerInfo : BaseModel
    {
        private int _id;
        private int _indexServer;
        private string _name = string.Empty;
        private int _accountCount;
        private DateTime? _createdAt;
        private DateTime? _updatedAt;
        private bool _isDeleted;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value, nameof(Id));
        }

        public int IndexServer
        {
            get => _indexServer;
            set => SetProperty(ref _indexServer, value, nameof(IndexServer));
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, nameof(Name));

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

        public int AccountCount
        {
            get => _accountCount;
            set => SetProperty(ref _accountCount, value, nameof(AccountCount));

        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set => SetProperty(ref _isDeleted, value, nameof(IsDeleted));

        }
    }
}
