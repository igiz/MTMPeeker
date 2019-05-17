using System;

namespace TFSPeeker.Interrogation
{
	public interface IInterrogator<out TDataSet>
	{
		#region Properties

		IObservable<TDataSet> DataProvider { get; }

		#endregion

		#region Methods

		void Start();

		void Stop();

		void Refresh();

		void SwitchContext(ApplicationContext newContext);

		#endregion
	}
}