namespace DiyetisyenRandevuSistemi.Models
{
	public class HekimCalismaPlani
	{
		public int Id { get; set; }

		public int DiyetisyenId { get; set; }  // foreign key
		public DayOfWeek Gun { get; set; }
		public TimeSpan Baslangic { get; set; }
		public TimeSpan Bitis { get; set; }
		public int SlotDakika { get; set; }

		// Navigation
		public Diyetisyen Diyetisyen { get; set; }
	}

}
