using System;

namespace GothmogToolkit.Tools.Core.OperationResults
{
	public struct OperationData
	{
		public OperationData(OperationResult result, Exception exception = null, string message = null)
		{
			Result = result;
			Exception = exception;
			Message = message;
		}
		public OperationResult Result { get; }
		public Exception Exception { get; }
		public string Message { get; }
	}
}
