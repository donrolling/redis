using Models.Transactions;

namespace Models
{
    /// <summary>
    /// This class id designed to represent basic Success/Failure with a message.
    /// It should be used to replace things like void actions. We almost always need a result for an action and this is the most basic.
    /// This class is inherited in places where more detail is required.
    /// </summary>
    public class OperationResult
    {
        private bool _success = true;
        private string _message = string.Empty;
        private Status _status = Status.Success;

        public bool Failure
        {
            get
            {
                return !Success;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        public Status Status
        {
            get
            {
                return _status;
            }
        }

        public bool Success
        {
            get { return _success; }
        }

        public OperationResult()
        {
        }

        public OperationResult(bool success, string message)
        {
            _success = success;
            if (!_success)
            {
                _status = Status.Failure;
            }
            _message = message;
        }

        public OperationResult(bool success, string message, Status status)
        {
            _success = success;
            _message = message;
            _status = status;
        }

        public static OperationResult Fail(string message, Status status = Status.Failure)
        {
            return new OperationResult(false, message, status);
        }

        public static OperationResult Fail(Status status = Status.Failure)
        {
            return new OperationResult(false, string.Empty, status);
        }

        public static OperationResult Ok(string message, Status status = Status.Success)
        {
            return new OperationResult(false, message, status);
        }

        public static OperationResult Ok(Status status = Status.Success)
        {
            return new OperationResult(false, string.Empty, status);
        }
    }
}