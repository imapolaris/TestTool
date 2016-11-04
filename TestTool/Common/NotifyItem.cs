using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Data.Common
{
    [Serializable]
	public class NotifyItem : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void onPropertyChanged([CallerMemberName] string name = "")
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		protected void updateProper<T>(ref T properValue, T newValue, [CallerMemberName] string properName = "")
		{
			if (object.Equals(properValue, newValue))
				return;

			properValue = newValue;
			onPropertyChanged(properName);
		}
	}
}
