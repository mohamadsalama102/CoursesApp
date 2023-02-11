namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class Discount
{
    public bool UsePercentage { get; set; }
    public double Percentage { get; set; }
    public decimal Amount { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public bool IsActive { get; set; }
}