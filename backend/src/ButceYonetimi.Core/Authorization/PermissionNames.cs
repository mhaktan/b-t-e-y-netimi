namespace ButceYonetimi.Authorization
{
    public static class PermissionNames
    {
        public const string Pages = "Pages";

        // BudgetPeriod
        public const string BudgetPeriod_Read = "BudgetPeriod.Read";
        public const string BudgetPeriod_Create = "BudgetPeriod.Create";
        public const string BudgetPeriod_Update = "BudgetPeriod.Update";
        public const string BudgetPeriod_Delete = "BudgetPeriod.Delete";

        // Department
        public const string Department_Read = "Department.Read";
        public const string Department_Create = "Department.Create";
        public const string Department_Update = "Department.Update";
        public const string Department_Delete = "Department.Delete";

        // BudgetCategory
        public const string BudgetCategory_Read = "BudgetCategory.Read";
        public const string BudgetCategory_Create = "BudgetCategory.Create";
        public const string BudgetCategory_Update = "BudgetCategory.Update";
        public const string BudgetCategory_Delete = "BudgetCategory.Delete";

        // ExchangeRate
        public const string ExchangeRate_Read = "ExchangeRate.Read";
        public const string ExchangeRate_Create = "ExchangeRate.Create";
        public const string ExchangeRate_Update = "ExchangeRate.Update";
        public const string ExchangeRate_Delete = "ExchangeRate.Delete";

        // BudgetRequest
        public const string BudgetRequest_Read = "BudgetRequest.Read";
        public const string BudgetRequest_Create = "BudgetRequest.Create";
        public const string BudgetRequest_Update = "BudgetRequest.Update";
        public const string BudgetRequest_Delete = "BudgetRequest.Delete";
        public const string BudgetRequest_ChangeStatus = "BudgetRequest.ChangeStatus";

        // BudgetRequestItem
        public const string BudgetRequestItem_Read = "BudgetRequestItem.Read";
        public const string BudgetRequestItem_Create = "BudgetRequestItem.Create";
        public const string BudgetRequestItem_Update = "BudgetRequestItem.Update";
        public const string BudgetRequestItem_Delete = "BudgetRequestItem.Delete";

        // Budget
        public const string Budget_Read = "Budget.Read";
        public const string Budget_Create = "Budget.Create";
        public const string Budget_Update = "Budget.Update";
        public const string Budget_Delete = "Budget.Delete";
        public const string Budget_ChangeStatus = "Budget.ChangeStatus";

        // Expense
        public const string Expense_Read = "Expense.Read";
        public const string Expense_Create = "Expense.Create";
        public const string Expense_Update = "Expense.Update";
        public const string Expense_Delete = "Expense.Delete";
        public const string Expense_ChangeStatus = "Expense.ChangeStatus";

        // RBAC management
        public const string AppUser_Read = "AppUser.Read";
        public const string AppRole_Read = "AppRole.Read";
        public const string AppUser_Create = "AppUser.Create";
        public const string AppRole_Create = "AppRole.Create";
        public const string AppUser_Update = "AppUser.Update";
        public const string AppRole_Update = "AppRole.Update";
        public const string AppUser_Delete = "AppUser.Delete";
        public const string AppRole_Delete = "AppRole.Delete";
        public const string AppRole_AssignPermissions = "AppRole.AssignPermissions";

    }
}
