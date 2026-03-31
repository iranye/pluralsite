namespace FactoryMethod;

public abstract class DiscountService
{
    public abstract int DiscountPercentage { get; }

    public override string ToString() => GetType().Name;
}

public class CountryDiscountService : DiscountService
{
    private readonly string _countryIdentifier;

    public CountryDiscountService(string countryIdentifier)
    {
        _countryIdentifier = countryIdentifier;
    }

    public override int DiscountPercentage
    {
        get
        {
            switch (_countryIdentifier)
            {
                // if you're from Belgium, you get a better discount :)
                case "BE":
                    return 20;
                default:
                    return 10;
            }
        }
    }
}

public class CodeDiscountService : DiscountService
{
    private readonly Guid _code;

    public CodeDiscountService(Guid code)
    {
        _code = code;
    }

    public override int DiscountPercentage
    {
        // each code returns the same fixed percentage, but a code is only 
        // valid once - include a check to so whether the code's been used before
        // ... 
        get => 15;
    }
}
