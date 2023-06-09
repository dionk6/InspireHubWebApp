using InspireHubWebApp.Models;

namespace InspireHubWebApp.DTOs
{
    public class DashboardDto
    {
        public int Trainings { get; set; }
        public int Students { get; set; }
        public decimal UnpaidAmount { get; set; }
        public int Invoices { get; set; }
        public List<Training> TrainingsList { get; set; }
    }
}
