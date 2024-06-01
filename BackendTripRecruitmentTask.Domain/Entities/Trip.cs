using BackendTripRecruitmentTask.Domain.Exceptions;

namespace BackendTripRecruitmentTask.Domain.Entities;

public class Trip
{
    public int ID { get; set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public int NumberOfSeats { get; private set; }

    public Country Country { get; private set; }
    public string CountryThreeLetterCode { get; }
    public IList<Registration> Registrations { get; private set; } = new List<Registration>();

    public static Trip Create(string name, string? description, DateTime startDate, int numberOfSeats, Country country)
    {
        ValidateInput(name, description, startDate, numberOfSeats, country);

        return new Trip
        {
            Name = name,
            Description = description,
            Country = country,
            StartDate = startDate,
            NumberOfSeats = numberOfSeats
        };
    }

    public void Update(string? name, string? description, DateTime? startDate, int? numberOfSeats, Country? country)
    {
        ValidateInput(name ?? Name,
            description ?? Description,
            startDate ?? StartDate,
            numberOfSeats ?? NumberOfSeats,
            country ?? Country);

        Name = name ?? Name;
        Description = description ?? Description;
        Country = country ?? Country;
        StartDate = startDate ?? StartDate;
        NumberOfSeats = numberOfSeats ?? NumberOfSeats;
    }

    private static void ValidateInput(string name, string? description, DateTime startDate, int numberOfSeats,
        Country country)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InputException(nameof(name), "Trip name cannot be null or empty.");

        if (name.Contains(Environment.NewLine) || name.Length > 50)
            throw new InputException(nameof(name), "Trip name must be a single line and a maximum of 50 characters.");

        if (description != null && description.Trim() == string.Empty)
            throw new InputException(nameof(description), "Trip description cannot be empty or whitespace string.");

        if (startDate >= DateTime.UtcNow)
            throw new InputException(nameof(startDate), "Trip start date must be in the future.");

        if (numberOfSeats is >= 1 and <= 100)
            throw new InputException(nameof(numberOfSeats),
                "Trip number of seats must be between 1 and 100 inclusive.");

        if (country == default)
            throw new InputException(nameof(country), "Trip destination country cannot be null.");
    }
}