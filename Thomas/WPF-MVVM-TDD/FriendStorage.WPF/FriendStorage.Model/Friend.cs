namespace FriendStorage.Model;

public class Friend
{
    public int Id { get; set; }

    public string FirstName { get; set; } = String.Empty;

    public string LastName { get; set; } = String.Empty;

    public DateTime? Birthday { get; set; }

    public bool IsDeveloper { get; set; }

    public override string ToString()
    {
        var devStr = IsDeveloper ? "DEVELOPER" : "NON-DEVELOPER";
        return $"{FirstName} {LastName} {devStr}";
    }

}
