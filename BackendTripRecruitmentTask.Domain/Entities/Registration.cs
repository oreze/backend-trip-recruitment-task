namespace BackendTripRecruitmentTask.Domain.Entities;

public class Registration
{
    public int ID { get; set; }
    public string Email { get; private set; }
    public int TripID { get; private set; }
    public Trip Trip { get; private set; }
}