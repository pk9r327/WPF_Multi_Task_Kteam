using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Multi_Task.ViewModel;

namespace WPF_Multi_Task.Model
{
    public class ProfileDetail : BaseViewModel
    {
        private int _Index;
        public int Index { get => _Index; set { _Index = value; OnPropertyChanged(); } }

        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }

        private string _Status;
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }
    }
}
