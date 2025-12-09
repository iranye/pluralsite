using Prototype;

var manager = new Manager("Cindy");
var managerClone = (Manager)manager.Clone();
Console.WriteLine($"Manager was cloned: {managerClone.Name}");

var employee = new Employee("Kevin", manager);
var employeeClone = (Employee)employee.Clone(true);
Console.WriteLine($"Employee was cloned: {employeeClone.Name}, with manager {employeeClone.Manager.Name}");

var managerNewName = "Karen";
Console.WriteLine($"Renaming Manager from {manager.Name} to {managerNewName}");
manager.Name = managerNewName;

Console.WriteLine($"Previously cloned Employee: {employeeClone.Name}, with manager {employeeClone.Manager.Name}");
