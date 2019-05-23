namespace Models.Transactions {
	public enum Status {
		Success,
		Failure,
		ItemNotFound,
		Cancelled,
		Aborted,
		Expired
	}
}