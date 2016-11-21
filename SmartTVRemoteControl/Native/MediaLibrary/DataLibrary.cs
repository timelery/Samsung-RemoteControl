using MediaLibrary.DataModels;
using Microsoft.WindowsAPICodePack.Shell;
using SmartView2.Core;
using SmartTVRemoteControl.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using TagLib;

namespace SmartTVRemoteControl.Native.MediaLibrary
{
	public class DataLibrary
	{
		private bool isDataLoaded;

		private CancellationTokenSource cancelToken;

		private IBaseDispatcher dispatcher;

		public Guid ID
		{
			get;
			set;
		}
        
		static DataLibrary()
		{
			
		}
        
        public DataLibrary(IBaseDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        private void OnDataLoaded()
		{
			EventHandler eventHandler = this.DataLoaded;
			if (eventHandler != null)
			{
				eventHandler(this, EventArgs.Empty);
			}
		}

		private void OnDataUpdated()
		{
			EventHandler eventHandler = this.DataUpdated;
			if (eventHandler != null)
			{
				eventHandler(this, EventArgs.Empty);
			}
		}
        
		public async Task SaveLibrary()
		{
			Settings.Default.ID = this.ID;
			Settings.Default.Save();
		}
        
		public event EventHandler DataLoaded;

		public event EventHandler DataUpdated;
	}
}