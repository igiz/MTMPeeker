using System;
using TFSPeeker.Interrogation;

namespace TFSPeeker
{
	public interface IApplication: IObserver<DataSet>, IDisposable
	{
		void Run(IInterrogator<DataSet> interrogator);
	}
}