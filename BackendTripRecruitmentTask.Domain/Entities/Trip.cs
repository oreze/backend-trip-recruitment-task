namespace BackendTripRecruitmentTask.Domain.Entities;

public class Trip
{
    public string ID { get; set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string Country { get; private set; }
    public DateTime StartDate { get; private set; }
    public int NumberOfSeats { get; private set; }

    public IList<Registration> Registrations { get; private set; } = new List<Registration>();
}