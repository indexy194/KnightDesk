using System.Collections.Generic;
using System.ComponentModel;

namespace KnightDesk.Presentation.WPF.Models
{

    public class AutoSettings : BaseModel
    {
        private bool _autoState;
        private bool _autoEvent;
        private bool _autoEquip;
        private string _eventName = string.Empty;
        private string _equipTypeName = string.Empty;
        private string _nameFocus = string.Empty;
        private MountType _mountType = MountType.Heo;
        private MountType _mountType2 = MountType.Heo;

        public bool AutoState
        {
            get => _autoState;
            set => SetProperty(ref _autoState, value, nameof(AutoState));
        }

        public bool AutoEvent
        {
            get => _autoEvent;
            set => SetProperty(ref _autoEvent, value, nameof(AutoEvent));
        }

        public bool AutoEquip
        {
            get => _autoEquip;
            set => SetProperty(ref _autoEquip, value, nameof(AutoEquip));
        }

        public string EventName
        {
            get => _eventName;
            set => SetProperty(ref _eventName, value, nameof(EventName));
        }

        public string EquipTypeName
        {
            get => _equipTypeName;
            set => SetProperty(ref _equipTypeName, value, nameof(EquipTypeName));
        }

        public string NameFocus
        {
            get => _nameFocus;
            set => SetProperty(ref _nameFocus, value, nameof(NameFocus));
        }

        public MountType MountType
        {
            get => _mountType;
            set => SetProperty(ref _mountType, value, nameof(MountType));
        }

        public MountType MountType2
        {
            get => _mountType2;
            set => SetProperty(ref _mountType2, value, nameof(MountType2));
        }

        // Static lists for ComboBox binding
        public static List<string> EventNames
        {
            get
            {
                return new List<string>
                {
                    "Tết Nguyên Đán",
                    "Sự Kiện 8/3",
                    "Hùng Vương",
                    "Hè",
                    "Vu Lan",
                    "Trung Thu",
                    "Halloween",
                    "Black Friday",
                    "Noel",
                    "Chiếm thành",
                    "Chiếm mỏ"
                };
            }
        }

        public static List<string> EquipTypeNames
        {
            get
            {
                return new List<string>
                {
                    "Sát Thủ",
                    "Chiến Binh",
                    "Pháp Sư",
                    "Xạ Thủ"
                };
            }
        }

        // Helper method to get mount type description
        public static string GetMountTypeDescription(MountType mountType)
        {
            var field = mountType.GetType().GetField(mountType.ToString());
            var attribute = (DescriptionAttribute)System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute != null ? attribute.Description : mountType.ToString();
        }

        // Get all mount types for ComboBox binding
        public static List<MountType> MountTypes
        {
            get
            {
                return new List<MountType>
                {
                    MountType.Heo,
                    MountType.HoaKiLan,
                    MountType.LanSuVu,
                    MountType.ChuotPoro,
                    MountType.PhuongHoangLua,
                    MountType.TuanLoc,
                    MountType.GhostRider,
                    MountType.VoiMaMut,
                    MountType.CaChep,
                    MountType.May,
                    MountType.ChuotPoroVv,
                };
            }
        }
    }

    // Helper class for Mount Type display
    public class MountTypeItem
    {
        public MountType Value { get; set; }
        public string Display { get; set; }

        public MountTypeItem(MountType value)
        {
            Value = value;
            Display = AutoSettings.GetMountTypeDescription(value);
        }
    }
    public enum MountType
    {
        [Description("Heo")]
        Heo = 222,

        [Description("Hỏa Kì Lân")]
        HoaKiLan = 294,

        [Description("Lân Sư Vũ")]
        LanSuVu = 246,

        [Description("Chuột Poro")]
        ChuotPoro = 251,

        [Description("Phượng Hoàng Lửa")]
        PhuongHoangLua = 299,

        [Description("Tuần Lộc")]
        TuanLoc = 124,

        [Description("Ghost Rider")]
        GhostRider = 279,

        [Description("Voi Ma Mút")]
        VoiMaMut = 269,

        [Description("Cá Chép")]
        CaChep = 318,

        [Description("Mây")]
        May = 275,

        [Description("Chuột Poro VV")]
        ChuotPoroVv = 268,
    }
}
