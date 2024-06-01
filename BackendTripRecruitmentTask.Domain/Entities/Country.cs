using System.Globalization;
using BackendTripRecruitmentTask.Domain.Exceptions;

namespace BackendTripRecruitmentTask.Domain.Entities;

public class Country
{
    public string ThreeLetterCode { get; private set; }
    public string Name { get; private set; }

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

            var name = TrimTo20Characters(region.EnglishName);
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

        if (string.IsNullOrWhiteSpace(name))
            throw new InputException(nameof(name), "Country name cannot be null or empty.");

        if (name.Length > 20)
            throw new InputException(nameof(name), "Country name has max length of 20 characters.");
    }

    private static string TrimTo20Characters(string input)
    {
        var result = input;
        if (input.Length > 20) result = result.Substring(0, 20);

        return result;
    }
}