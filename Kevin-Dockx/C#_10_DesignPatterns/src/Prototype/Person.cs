using Newtonsoft.Json;

namespace Prototype;

public abstract class Person
{
    public abstract string Name { get; set; }

    public abstract Person Clone(bool deepClone);  
}


public class Employee : Person
{
    public Manager Manager { get; set; }
    public override string Name { get; set; }
     
    public Employee(string name, Manager manager)
    {
        Name = name;
        Manager = manager;
    }

    public override Person Clone(bool deepClone = false)
    {
        if (deepClone)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var objectAsJson = JsonConvert.SerializeObject(this, typeof(Employee), settings);
            var result = JsonConvert.DeserializeObject<Person>(objectAsJson, settings);
            return result is null ? (Person)MemberwiseClone() : result;
        }

        return (Person)MemberwiseClone();
    }
}


public class Manager : Person
{
    public override string Name { get; set; }

    public Manager(string name)
    {
        Name = name;
    }

    public override Person Clone(bool deepClone = false)
    {
        if (deepClone)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var objectAsJson = JsonConvert.SerializeObject(this, typeof(Manager), settings);
            var result = JsonConvert.DeserializeObject<Person>(objectAsJson, settings);
            return result is null ? (Person)MemberwiseClone() : result;
        }

        return (Person)MemberwiseClone();
    }
}
