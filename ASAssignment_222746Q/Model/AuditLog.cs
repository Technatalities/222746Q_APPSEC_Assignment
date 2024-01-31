namespace ASAssignment_222746Q.Model
{
	public class AuditLog
	{
		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public string UserId { get; set; }
		public string ActionsTaken { get; set; }
		public string Information { get; set; }
	}
}
