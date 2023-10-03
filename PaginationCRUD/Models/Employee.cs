using Sieve.Attributes;

namespace PaginationCRUD.Models
{
    public class Employee
    {
        [Sieve(CanSort = true)] public int Id { get; set; }
        [Sieve(CanFilter = true,CanSort =true)] public string Name { get; set; } = string.Empty;

        [Sieve(CanFilter = true,CanSort =true)] public string Designation { get; set; } = string.Empty;  
    }
}
