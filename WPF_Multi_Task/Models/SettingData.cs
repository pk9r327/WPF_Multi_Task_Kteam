using MvvmHelpers;

namespace WPF_Multi_Task.Model
{
    public partial class SettingData : ObservableObject
    {
        private int totalData;

        public int TotalData
        {
            get => totalData;
            set => SetProperty(ref totalData, value);
        }
    }
}
