namespace BuilderPattern;

/// <summary>
/// The 'Director' class
/// </summary>
public class Director
{
    // Builder uses a complex series of steps
    public void Construct(Builder builder)
    {
        builder.BuildPartA();
        builder.BuildPartB();
    }
}

public abstract class Builder
{
    public abstract void BuildPartA();
    public abstract void BuildPartB();
    public abstract Product GetResult();
}

public class ConcreteBuilder1 : Builder
{
    private Product product = new Product();

    public override void BuildPartA()
    {
        product.Add("PartA");
    }

    public override void BuildPartB()
    {
        product.Add("PartB");
    }

    public override Product GetResult()
    {
        return product;
    }
}

public class ConcreteBuilder2 : Builder
{
    private Product product = new Product();

    public override void BuildPartA()
    {
        product.Add("PartX");
    }

    public override void BuildPartB()
    {
        product.Add("PartY");
    }

    public override Product GetResult()
    {
        return product;
    }
}

public class Product
{
    private List<string> parts = new List<string>();

    public void Add(string part)
    {
        parts.Add(part);
    }

    public void Show()
    {
        Console.WriteLine("\nProduct Parts -------");
        foreach (string part in parts)
            Console.WriteLine(part);
    }
}
