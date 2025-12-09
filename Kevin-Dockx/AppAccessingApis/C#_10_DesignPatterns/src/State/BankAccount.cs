namespace State;

/// Context
public class BankAccount
{
    public BankAccountState BankAccountState { get; set; }
    public decimal Balance { get { return BankAccountState.Balance; } }

    public BankAccount()
    {
        BankAccountState = new RegularState(200, this);
    }

    /// Request a deposit
    public void Deposit(decimal amount)
    {
        // let the current state handle the deposit
        BankAccountState.Deposit(amount);
    }

    /// Request a withdrawal
    public void Withdraw(decimal amount)
    {
        // let the current state handle the withdrawel
        BankAccountState.Withdraw(amount);
    }
}

/// State
public abstract class BankAccountState
{
    public BankAccount BankAccount { get; protected set; } = null!;
    public decimal Balance { get; protected set; }

    public abstract void Deposit(decimal amount);
    public abstract void Withdraw(decimal amount);
}

/// ConcreteState
public class OverdrawnState : BankAccountState
{
    public OverdrawnState(decimal balance, BankAccount bankAccount)
    {
        Balance = balance;
        BankAccount = bankAccount;
    }

    /// Handle a deposit
    public override void Deposit(decimal amount)
    {
        Console.WriteLine($"In {GetType()}, depositing {amount}");
        Balance += amount;
        if (Balance >= 0)
        {
            // change state to regular
            BankAccount.BankAccountState = new RegularState(Balance, BankAccount);
        } 
    }

    /// Handle withdrawing
    public override void Withdraw(decimal amount)
    {
        // cannot withdraw
        Console.WriteLine($"In {GetType()}, cannot withdraw, balance {Balance}");
    }
}

/// ConcreteState
public class RegularState : BankAccountState
{
    public RegularState(decimal balance, BankAccount bankAccount)
    {
        Balance = balance;
        BankAccount = bankAccount;
    }

    /// Handle a deposit
    public override void Deposit(decimal amount)
    {
        Console.WriteLine($"In {GetType()}, depositing {amount}");
        Balance += amount;
        if (Balance >= 1000)
        {
            // change state to gold
            BankAccount.BankAccountState = new GoldState(Balance, BankAccount);
        }
    }

    /// Handle withdrawing
    public override void Withdraw(decimal amount)
    {
        Console.WriteLine($"In {GetType()}, withdrawing {amount} from {Balance}");
        // change state to overdrawn when withdrawing results in less than zero
        Balance -= amount;
        if (Balance < 0)
        {
            // change state to overdrawn
            BankAccount.BankAccountState = new OverdrawnState(Balance, BankAccount);
        }
    }
}

/// ConcreteState
public class GoldState : BankAccountState
{
    public GoldState(decimal balance, BankAccount bankAccount)
    {
        Balance = balance;
        BankAccount = bankAccount;
    }

    /// Handle a deposit
    public override void Deposit(decimal amount)
    {
        Console.WriteLine($"In {GetType()}, depositing {amount} + 10% bonus: {amount/10}");
        Balance += amount + (amount/10);
    }

    /// Handle withdrawing
    public override void Withdraw(decimal amount)
    {
        Console.WriteLine($"In {GetType()}, withdrawing {amount} from {Balance}");
        // change state to overdrawn when withdrawing results in less than zero
        Balance -= amount;
        if (Balance < 1000 && Balance >= 0)
        {
            // change state to regular
            BankAccount.BankAccountState = new RegularState(Balance, BankAccount);
        }
        else if (Balance < 0)
        {
            // change state to overdrawn
            BankAccount.BankAccountState = new OverdrawnState(Balance, BankAccount);
        }
    }
}