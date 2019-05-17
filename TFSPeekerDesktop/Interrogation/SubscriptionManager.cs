using System;
using System.Collections.Generic;

namespace TFSPeeker.Interrogation
{
	public class SubscriptionManager<T> : IDisposable
	{
		private readonly IList<IObserver<T>> allSubscribers;
		private readonly IObserver<T> subscriber;

		public SubscriptionManager(IList<IObserver<T>> allSubscribers, IObserver<T> subscriber)
		{
			this.allSubscribers = allSubscribers;
			this.subscriber = subscriber;
		}

		public void Dispose()
		{
			if (subscriber != null && allSubscribers.Contains(subscriber)) {
				allSubscribers.Remove(subscriber);
			}	
		}
	}
}