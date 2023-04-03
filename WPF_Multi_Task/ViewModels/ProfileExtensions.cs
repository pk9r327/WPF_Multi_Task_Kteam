using System.Collections.Generic;

namespace WPF_Multi_Task.ViewModels
{
    public static class ProfileExtensions
    {
        public static void UpdateProfileIndex(this ICollection<ProfileViewModel> profileViewModels)
        {
            if (profileViewModels == null || profileViewModels.Count == 0)
            {
                return;
            }

            int i = 0;
            foreach (var p in profileViewModels)
            {
                p.ProfileDetail.Index = ++i;
            }
        }
    }
}
