using System.Globalization;
using BackendTripRecruitmentTask.Domain.Exceptions;

namespace BackendTripRecruitmentTask.Domain.Entities;

public class Country
{
    public string ThreeLetterCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;

    private Country() {}

    public static IEnumerable<Country> GetAllCountries()
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        var countries = new Dictionary<string, string>();

        foreach (var culture in cultures)
        {
            // not sure if it contains all countries, it would be a better idea to just use external nuget/api
            // taking care of that
            var region = new RegionInfo(culture.Name);

            if (string.IsNullOrWhiteSpace(region.ThreeLetterISORegionName) ||
                string.IsNullOrWhiteSpace(region.EnglishName))
                continue;

            var name = TrimToXCharacters(region.EnglishName, Constants.MaximumCountryNameLength);
            countries.TryAdd(region.ThreeLetterISORegionName, name);
        }

        return countries.Select(kvp => Create(kvp.Key, kvp.Value));
    }

    public static Country Create(string threeLetterCode, string name)
    {
        ValidateInput(threeLetterCode, name);

        return new Country
        {
            ThreeLetterCode = threeLetterCode,
            Name = name
        };
    }

    private static void ValidateInput(string threeLetterCode, string name)
    {
        if (string.IsNullOrWhiteSpace(threeLetterCode))
            throw new InputException(nameof(threeLetterCode), "Country code cannot be null or empty.");

        if (threeLetterCode.Length != 3)
            throw new InputException(nameof(threeLetterCode), "Country code must be 3 characters long.");

        if (string.IsNullOrWhiteSpace(name))
            throw new InputException(nameof(name), "Country name cannot be null or empty.");

        if (name.Length > Constants.MaximumCountryNameLength)
            throw new InputException(nameof(name),
                $"Country name has max length of {Constants.MaximumCountryNameLength} characters.");
    }

    private static string TrimToXCharacters(string input, int maxCharNumber)
    {
        var result = input;
        if (input.Length > maxCharNumber) result = result.Substring(0, maxCharNumber);

        return result;
    }
}