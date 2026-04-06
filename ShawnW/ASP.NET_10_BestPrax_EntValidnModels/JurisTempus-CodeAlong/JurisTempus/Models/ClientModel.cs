namespace JurisTempus.Models
{
	public class ClientModel
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public string? Phone { get; set; }
		public string? Contact { get; set; }

		public string? City { get; set; }

		public int CaseCount { get; set; }
	}
}



