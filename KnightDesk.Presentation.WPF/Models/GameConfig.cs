using System.IO;

namespace KnightDesk.Presentation.WPF.Models
{
    public class GameConfig : BaseModel
    {
        private string _gamePath = string.Empty;

        public string GamePath
        {
            get => _gamePath;
            set => SetProperty(ref _gamePath, value, nameof(GamePath));
        }

        public bool IsValidGamePath
        {
            get => !string.IsNullOrEmpty(_gamePath) &&
                   Path.GetExtension(_gamePath).ToLower() == ".exe" &&
                   File.Exists(_gamePath);
        }
    }
}
