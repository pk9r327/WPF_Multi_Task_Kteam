using MvvmHelpers;

namespace WPF_Multi_Task.Model
{
    public partial class ProfileDetail : ObservableObject
    {
        private int index;

        public int Index
        {
            get => index;
            set => SetProperty(ref index, value);
        }

        private string name;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string status;

        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }
    }
}
