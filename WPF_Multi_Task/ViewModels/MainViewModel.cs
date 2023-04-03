using MvvmHelpers;
using MvvmHelpers.Commands;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_Multi_Task.Model;

namespace WPF_Multi_Task.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        #region Properties
        private SettingData _SettingData;

        public SettingData SettingData
        {
            get => _SettingData;
            set => SetProperty(ref _SettingData, value);
        }

        public ObservableCollection<ProfileViewModel> Profiles { get; } = new();
        #endregion

        #region CMD
        //public ICommand PauseProfile_CMD { get; set; }
        //public ICommand ResumeProfile_CMD { get; set; }
        //public ICommand StartProfile_CMD { get; set; }
        //public ICommand DeleteProfile_CMD { get; set; }
        //public ICommand CreateProfile_CMD { get; set; }
        //public ICommand StopProfile_CMD { get; set; }
        public ICommand AddData_CMD { get; }
        public ICommand StartAll_CMD { get; }
        public ICommand StopAll_CMD { get; }
        public ICommand DeleteAll_CMD { get; }
        #endregion

        public MainViewModel()
        {
            FirstLoad();

            AddData_CMD = new AsyncCommand(AddData);
            StartAll_CMD = new Command(StartAll, canExecute: () => Profiles != null);
            StopAll_CMD = new Command(StopAll, () => Profiles != null);
            DeleteAll_CMD = new AsyncCommand(DeleteAllAsync, _ => Profiles != null);
        }

        #region Method
        void FirstLoad()
        {
            LoadSavedData();
        }

        //void LoadCommand()
        //{
        //    //StopProfile_CMD = new Command<ProfileDetail>((p) => { return p != null && Profiles != null && Profiles.Contains(p); }, (p) => { StopProfile(p); });
        //    //PauseProfile_CMD = new RelayCommand<ProfileDetail>((p) => { return p != null && Profiles != null && Profiles.Contains(p); }, (p) => { PauseProfile(p); });
        //    //ResumeProfile_CMD = new RelayCommand<ProfileDetail>((p) => { return p != null && Profiles != null && Profiles.Contains(p); }, (p) => { ResumeProfile(p); });
        //    //StartProfile_CMD = new RelayCommand<ProfileDetail>((p) => { return p != null && Profiles != null && Profiles.Contains(p); }, (p) => { StartProfile(p); });
        //    //DeleteProfile_CMD = new RelayCommand<ProfileDetail>((p) => { return p != null && Profiles != null && Profiles.Contains(p); }, async (p) => { StopProfile(p); await DeleteProfile(p); });
        //    //CreateProfile_CMD = new AsyncCommand(async () => await CreateProfileAsync(), canExecute: _ => true);
        //    AddData_CMD = new Command(AddData, canExecute: () => true);
        //    StartAll_CMD = new Command(StartAll, canExecute: () => Profiles != null);
        //    StopAll_CMD = new Command(StopAll, () => Profiles != null);
        //    DeleteAll_CMD = new Command(DeleteAll, () => Profiles != null);
        //}

        async Task AddData()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                for (int i = 0; i < SettingData.TotalData; i++)
                {
                    var profile = new ProfileViewModel()
                    {
                        Container = Profiles,
                        ProfileDetail = new() { Name = i.ToString(), Status = "Created" }
                    };

                    Profiles.Add(profile);
                }

                Profiles.UpdateProfileIndex();
            });
            //this.StartTask(async () =>
            //{
            //    for (int i = 0; i < SettingData.TotalData; i++)
            //    {
            //        await CreateProfileAsync(i + "");
            //    }
            //}, null/*, null*/);
        }

        async Task CreateProfileAsync(string profileName = null)
        {
            var profile = new ProfileViewModel()
            {
                Container = Profiles,
                ProfileDetail = new() { Name = profileName, Status = "Created" }
            };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Profiles.Add(profile);
                Profiles.UpdateProfileIndex();
            });
        }

        void StopAll()
        {
            var result = Parallel.ForEach(Profiles, p =>
            {
                p.StopCommand.Execute(null);
            });

            //this.StartTask(() =>
            //{
            //    foreach (var p in Profiles)
            //    {
            //        p.StopCommand.Execute(null);
            //    }
            //}, null/*, null*/);
        }

        void StartAll()
        {
            var result = Parallel.ForEach(Profiles, p =>
            {
                p.StartCommand.Execute(null);
            });
            //this.StartTask(() =>
            //{
            //    foreach (var p in Profiles)
            //    {
            //        p.StartCommand.Execute(null);
            //    }
            //}, null/*, null*/);
        }

        async Task DeleteAllAsync()
        {
            StopAll();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                while (Profiles.Count > 0)
                {
                    Profiles[0].ProfileDetail.Status = $"Removing";
                    Profiles[0].Dispose();
                    Profiles.RemoveAt(0);
                }
                Profiles.UpdateProfileIndex();
            });

            //var result = Parallel.ForEach(Profiles, p =>
            //{
            //    Profiles.Remove(p);
            //});
            //this.StartTask(async () =>
            //{
            //    while (Profiles.Count > 0)
            //    {
            //        Profiles[0].StopCommand.Execute(null);
            //        Profiles[0].ProfileDetail.Status = $"Deleting Profile";

            //        await Application.Current.Dispatcher.InvokeAsync(() =>
            //        {
            //            Profiles[0].ProfileDetail.Status = $"Removing";
            //            Profiles.RemoveAt(0);
            //            Profiles.UpdateProfileIndex();
            //        });
            //    }
            //}, null/*, null*/);
        }

        void LoadSavedData()
        {
            try
            {
                var text = File.ReadAllText("Saved.txt");
                SettingData = JsonConvert.DeserializeObject<SettingData>(text);

            }
            catch
            {

            }

            if (SettingData == null)
            {
                SettingData = new SettingData();
                SettingData.TotalData = 100;
            }
        }

        public void SaveData()
        {
            try
            {
                File.WriteAllText("Saved.txt", JsonConvert.SerializeObject(SettingData));
            }
            catch { }
        }
        #endregion
    }
}