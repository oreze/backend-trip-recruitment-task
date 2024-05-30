using System.Net.Mail;
using BackendTripRecruitmentTask.Domain.Exceptions;

namespace BackendTripRecruitmentTask.Domain.Entities;

public class Registration
{
    public int ID { get; set; }
    public string Email { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    
    public int TripID { get; private set; }
    public Trip Trip { get; private set; }

    public static Registration Create(string email, DateTime registeredAt, Trip trip)
    {
        ValidateInput(email, registeredAt, trip);

        return new()
        {
            Email = email,
            RegisteredAt = registeredAt,
            Trip = trip
        };
    }
    
    private static void ValidateInput(string email, DateTime registeredAt, Trip trip)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new InputException(nameof(email), "Registration email cannot be null or empty.");

        if (!MailAddress.TryCreate(email, out MailAddress? mailAddress))
            throw new InputException(nameof(email), "Registration email is not a valid email address.");

        if (registeredAt >= DateTime.UtcNow)
            throw new InputException(nameof(registeredAt), "Registration date cannot be in the future.");
        
        if (trip == default)
            throw new InputException(nameof(trip), "Registration trip cannot be null.");
    }
}