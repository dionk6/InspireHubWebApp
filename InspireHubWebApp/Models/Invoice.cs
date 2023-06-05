namespace InspireHubWebApp.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string InvoiceDate { get; set; }
        public int InvoiceNo { get; set; }
        public int Year { get; set; }
        public string? StudentAddress { get; set; }
        public string Description { get; set; }
        public string Month { get; set; }
        public string Price { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Student Student { get; set; }
    }
}
