using BackendTripRecruitmentTask.Domain.Exceptions;

namespace BackendTripRecruitmentTask.Domain.Entities;

public class Country
{
    public string ThreeLetterCode { get; private set; }
    public string Name { get; private set; }

    public static Country Create(string threeLetterCode, string name)
    {
        ValidateInput(threeLetterCode, name);

        return new Country()
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

        if (name.Length <= 20)
            throw new InputException(nameof(name), "Country name has max length of 20 characters.");
    }

}