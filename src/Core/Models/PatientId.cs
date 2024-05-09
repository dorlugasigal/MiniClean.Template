namespace Core.Models;

public record PatientId(string? Value)
{
    public static implicit operator PatientId(string id) => new(id);
}