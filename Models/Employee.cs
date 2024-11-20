#pragma warning disable CS8618 // Disable the warning for nullable reference types

namespace GestionaireEmployes.Models
{
    public class Employee
{
    public int EmployeeId { get; set; }
    public string? FullName { get; set; } // Nullable
    public string? Position { get; set; } // Nullable
    public decimal Salary { get; set; }
    public Currency Currency { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsCurrentEmployee { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ContractType { get; set; } // Nullable
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}


    public enum Currency
    {
        EUR,
        USD,
        TND
    }
}