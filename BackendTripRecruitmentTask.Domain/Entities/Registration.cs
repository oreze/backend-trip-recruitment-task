using System.Net.Mail;
using BackendTripRecruitmentTask.Domain.Exceptions;

namespace BackendTripRecruitmentTask.Domain.Entities;

public class Registration
{
    private Registration()
    {
    }

    public int ID { get; set; }
    public string Email { get; private set; } = null!;
    public DateTime RegisteredAt { get; private set; }

    public int TripID { get; }
    public Trip Trip { get; private set; } = null!;

    public static Registration Create(string email, Trip trip)
    {
        ValidateInput(email, trip);

        return new Registration
        {
            Email = email,
            RegisteredAt = DateTime.UtcNow,
            Trip = trip
        };
    }

    private static void ValidateInput(string email, Trip trip)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new InputException(nameof(email), "Registration email cannot be null or empty.");

        if (!MailAddress.TryCreate(email, out var mailAddress))
            throw new InputException(nameof(email), "Registration email is not a valid email address.");

        if (trip == default)
            throw new InputException(nameof(trip), "Registration trip cannot be null.");
    }
}