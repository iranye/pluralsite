namespace Flyweight;

/// Flyweight
public interface ICharacter
{
    void Draw(string fontFamily, int fontSize);
}

/// Concrete Flyweight
public class CharacterA : ICharacter
{
    private readonly char _actualCharacter = 'a';
    private string _fontFamily = string.Empty;
    private int _fontSize;

    public void Draw(string fontFamily, int fontSize)
    {
        _fontFamily = fontFamily;
        _fontSize = fontSize;
        Console.WriteLine($"Drawing {_actualCharacter}, {_fontFamily} {_fontSize}");
    }
}

/// Concrete Flyweight
public class CharacterB : ICharacter
{
    private readonly char _actualCharacter = 'b';
    private string _fontFamily = string.Empty;
    private int _fontSize;

    public void Draw(string fontFamily, int fontSize)
    {
        _fontFamily = fontFamily;
        _fontSize = fontSize;
        Console.WriteLine($"Drawing {_actualCharacter}, {_fontFamily} {_fontSize}");
    }
}

/// FlyweightFactory
public class CharacterFactory
{
    private readonly Dictionary<char, ICharacter> _characters = new ();

    public ICharacter? GetCharacter(char characterIdentifier)
    { 
        // Coes the character dictionary contain the one we need? 
        if (_characters.ContainsKey(characterIdentifier))
        {
            Console.WriteLine("Character reuse");
            return _characters[characterIdentifier];
        }

        // The character isn't in the dictionary. 
        // Create it, store it, return it.
        Console.WriteLine("Character construction");
        switch (characterIdentifier)
        {
            case 'a':
                _characters[characterIdentifier] = new CharacterA();
                return _characters[characterIdentifier];
            case 'b':
                _characters[characterIdentifier] = new CharacterB();
                return _characters[characterIdentifier];
            // and so on...               
        }

        return null;
    }

    public ICharacter CreateParagraph(List<ICharacter> characters, int location)
    {
        return new Paragraph(characters, location);
    }
}

/// Unshared Concrete Flyweight
public class Paragraph : ICharacter
{
    private readonly int _location;
    private readonly List<ICharacter> _characters = new();

    public Paragraph(List<ICharacter> characters, int location)
    {
        _characters = characters;
        _location = location;
    }

    public void Draw(string fontFamily, int fontSize)
    {
        Console.WriteLine($"Drawing in paragraph at location {_location}");
        foreach (var character in _characters)
        {
            character.Draw(fontFamily, fontSize);
        }  
    }
}
