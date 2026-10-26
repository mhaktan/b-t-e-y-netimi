using Abp.Authorization;
using Abp.Localization;

namespace ButceYonetimi.Authorization
{
    public class ButceYonetimiAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull("Pages") ?? context.CreatePermission("Pages", L("Pages"));

            // BudgetPeriod
            pages.CreateChildPermission(PermissionNames.BudgetPeriod_Read, L("BudgetPeriod.Read"));
            pages.CreateChildPermission(PermissionNames.BudgetPeriod_Create, L("BudgetPeriod.Create"));
            pages.CreateChildPermission(PermissionNames.BudgetPeriod_Update, L("BudgetPeriod.Update"));
            pages.CreateChildPermission(PermissionNames.BudgetPeriod_Delete, L("BudgetPeriod.Delete"));

            // Department
            pages.CreateChildPermission(PermissionNames.Department_Read, L("Department.Read"));
            pages.CreateChildPermission(PermissionNames.Department_Create, L("Department.Create"));
            pages.CreateChildPermission(PermissionNames.Department_Update, L("Department.Update"));
            pages.CreateChildPermission(PermissionNames.Department_Delete, L("Department.Delete"));

            // BudgetCategory
            pages.CreateChildPermission(PermissionNames.BudgetCategory_Read, L("BudgetCategory.Read"));
            pages.CreateChildPermission(PermissionNames.BudgetCategory_Create, L("BudgetCategory.Create"));
            pages.CreateChildPermission(PermissionNames.BudgetCategory_Update, L("BudgetCategory.Update"));
            pages.CreateChildPermission(PermissionNames.BudgetCategory_Delete, L("BudgetCategory.Delete"));

            // ExchangeRate
            pages.CreateChildPermission(PermissionNames.ExchangeRate_Read, L("ExchangeRate.Read"));
            pages.CreateChildPermission(PermissionNames.ExchangeRate_Create, L("ExchangeRate.Create"));
            pages.CreateChildPermission(PermissionNames.ExchangeRate_Update, L("ExchangeRate.Update"));
            pages.CreateChildPermission(PermissionNames.ExchangeRate_Delete, L("ExchangeRate.Delete"));

            // BudgetRequest
            pages.CreateChildPermission(PermissionNames.BudgetRequest_Read, L("BudgetRequest.Read"));
            pages.CreateChildPermission(PermissionNames.BudgetRequest_Create, L("BudgetRequest.Create"));
            pages.CreateChildPermission(PermissionNames.BudgetRequest_Update, L("BudgetRequest.Update"));
            pages.CreateChildPermission(PermissionNames.BudgetRequest_Delete, L("BudgetRequest.Delete"));
            pages.CreateChildPermission(PermissionNames.BudgetRequest_ChangeStatus, L("BudgetRequest.ChangeStatus"));

            // BudgetRequestItem
            pages.CreateChildPermission(PermissionNames.BudgetRequestItem_Read, L("BudgetRequestItem.Read"));
            pages.CreateChildPermission(PermissionNames.BudgetRequestItem_Create, L("BudgetRequestItem.Create"));
            pages.CreateChildPermission(PermissionNames.BudgetRequestItem_Update, L("BudgetRequestItem.Update"));
            pages.CreateChildPermission(PermissionNames.BudgetRequestItem_Delete, L("BudgetRequestItem.Delete"));

            // Budget
            pages.CreateChildPermission(PermissionNames.Budget_Read, L("Budget.Read"));
            pages.CreateChildPermission(PermissionNames.Budget_Create, L("Budget.Create"));
            pages.CreateChildPermission(PermissionNames.Budget_Update, L("Budget.Update"));
            pages.CreateChildPermission(PermissionNames.Budget_Delete, L("Budget.Delete"));
            pages.CreateChildPermission(PermissionNames.Budget_ChangeStatus, L("Budget.ChangeStatus"));

            // Expense
            pages.CreateChildPermission(PermissionNames.Expense_Read, L("Expense.Read"));
            pages.CreateChildPermission(PermissionNames.Expense_Create, L("Expense.Create"));
            pages.CreateChildPermission(PermissionNames.Expense_Update, L("Expense.Update"));
            pages.CreateChildPermission(PermissionNames.Expense_Delete, L("Expense.Delete"));
            pages.CreateChildPermission(PermissionNames.Expense_ChangeStatus, L("Expense.ChangeStatus"));

            // RBAC
            pages.CreateChildPermission(PermissionNames.AppUser_Read, L("AppUser.Read"));
            pages.CreateChildPermission(PermissionNames.AppRole_Read, L("AppRole.Read"));
            pages.CreateChildPermission(PermissionNames.AppUser_Create, L("AppUser.Create"));
            pages.CreateChildPermission(PermissionNames.AppRole_Create, L("AppRole.Create"));
            pages.CreateChildPermission(PermissionNames.AppUser_Update, L("AppUser.Update"));
            pages.CreateChildPermission(PermissionNames.AppRole_Update, L("AppRole.Update"));
            pages.CreateChildPermission(PermissionNames.AppUser_Delete, L("AppUser.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_Delete, L("AppRole.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_AssignPermissions, L("AppRole.AssignPermissions"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ButceYonetimiConsts.LocalizationSourceName);
        }
    }
}
