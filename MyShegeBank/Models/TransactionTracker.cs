﻿namespace ShegeBank.Models;
internal class TransactionTracker
{
    public long TransactionId { get; set; }
    public long UserBankAccountId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal TransactionAmount { get; set; }
    public string? TransactionType { get; set; }
    public string? Description { get; set; }
}