using System.Collections.Generic;

namespace TFSPeeker.Interrogation
{
	public struct ViewResult
	{
		public string Name { get; set; }

		public IEnumerable<TestCaseDescription> Result { get; set; }
	}
}