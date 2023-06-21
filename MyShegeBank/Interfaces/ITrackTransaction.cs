namespace ShegeBank.Interfaces;

public interface ITrackTransaction
{
    Task InsertTransactionAsync();
    Task ViewTransactionAsync();
}