using System;
using System.ComponentModel;

namespace SmartTVRemoteControl.Models.Settings
{
	public class SettingsModel : INotifyPropertyChanged
	{
		
		public static SettingsModel Defaults
		{
			get
			{
				return SettingsModel.GetDefaultValues();
			}
		}
        
		public SettingsModel()
		{
		}

		private static SettingsModel GetDefaultValues()
		{
			SettingsModel settingsModel = new SettingsModel()
			{
			};
			return settingsModel;
		}

		public static SettingsModel Load()
		{
			SettingsModel settingsModel = new SettingsModel()
			{
			};
			return settingsModel;
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
			if (propertyChangedEventHandler != null)
			{
				propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public void Save(SettingsModel settings)
		{
			SmartTVRemoteControl.Properties.Settings.Default.Save();
			if (SettingsModel.SettingsSaved != null)
			{
				SettingsModel.SettingsSaved(this, null);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public static event EventHandler SettingsSaved;
	}
}