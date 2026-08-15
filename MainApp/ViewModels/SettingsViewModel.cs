using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MainApp.ViewModels
{
    public partial class SettingsViewModel : ObservableValidator
    {
        public SettingsViewModel()
        {
            SaveLocation = Settings.Default.SaveLocation;
            ValidateAllProperties();
        }

        [Required]
        [ObservableProperty]
        public partial string SaveLocation { get; set; }
    }
}
