namespace ButceYonetimi.Entities
{
    public enum BudgetRequestStatus
    {
        Draft = 0,
        PendingManagerApproval = 1,
        PendingFinanceApproval = 2,
        Approved = 3,
        Revision = 4,
        Rejected = 5,
        Cancelled = 6,
    }
}