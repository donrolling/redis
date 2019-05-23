using Models.Transactions;

namespace Models {

	public class OperationResult<T> : OperationResult {
		public T Result { get; set; }

		public OperationResult() { }

		public OperationResult(OperationResult operationResult) : base (operationResult.Success, operationResult.Message, operationResult.Status) { }

		public OperationResult(OperationResult operationResult, T result) : this(operationResult) {
			this.Result = result;
		}

		public static OperationResult<T> Ok(T result, string message = "") {
			return new OperationResult<T>(OperationResult.Ok(message), result);
		}

		public new static OperationResult<T> Fail(string message, Status status = Status.Failure) {
			return new OperationResult<T>(OperationResult.Fail(message));
		}
	}
}