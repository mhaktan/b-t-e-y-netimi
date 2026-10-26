using System.Collections.Generic;
using Abp.Dependency;

namespace ButceYonetimi.Authorization
{
    /// <summary>Single permission descriptor — name, group (entity), description.</summary>
    public class PermissionInfo
    {
        public string Name { get; }
        public string Group { get; }
        public string Description { get; }
        public bool IsRbac { get; }

        public PermissionInfo(string name, string group, string description, bool isRbac)
        {
            Name = name; Group = group; Description = description; IsRbac = isRbac;
        }
    }

    public interface IPermissionRegistry
    {
        IReadOnlyList<PermissionInfo> All { get; }
    }

    public class PermissionRegistry : IPermissionRegistry, ISingletonDependency
    {
        public IReadOnlyList<PermissionInfo> All { get; } = new List<PermissionInfo>
        {
            new PermissionInfo("BudgetPeriod.Read", "BudgetPeriod", "Read BudgetPeriod", false),
            new PermissionInfo("BudgetPeriod.Create", "BudgetPeriod", "Create BudgetPeriod", false),
            new PermissionInfo("BudgetPeriod.Update", "BudgetPeriod", "Update BudgetPeriod", false),
            new PermissionInfo("BudgetPeriod.Delete", "BudgetPeriod", "Delete BudgetPeriod", false),
            new PermissionInfo("Department.Read", "Department", "Read Department", false),
            new PermissionInfo("Department.Create", "Department", "Create Department", false),
            new PermissionInfo("Department.Update", "Department", "Update Department", false),
            new PermissionInfo("Department.Delete", "Department", "Delete Department", false),
            new PermissionInfo("BudgetCategory.Read", "BudgetCategory", "Read BudgetCategory", false),
            new PermissionInfo("BudgetCategory.Create", "BudgetCategory", "Create BudgetCategory", false),
            new PermissionInfo("BudgetCategory.Update", "BudgetCategory", "Update BudgetCategory", false),
            new PermissionInfo("BudgetCategory.Delete", "BudgetCategory", "Delete BudgetCategory", false),
            new PermissionInfo("ExchangeRate.Read", "ExchangeRate", "Read ExchangeRate", false),
            new PermissionInfo("ExchangeRate.Create", "ExchangeRate", "Create ExchangeRate", false),
            new PermissionInfo("ExchangeRate.Update", "ExchangeRate", "Update ExchangeRate", false),
            new PermissionInfo("ExchangeRate.Delete", "ExchangeRate", "Delete ExchangeRate", false),
            new PermissionInfo("BudgetRequest.Read", "BudgetRequest", "Read BudgetRequest", false),
            new PermissionInfo("BudgetRequest.Create", "BudgetRequest", "Create BudgetRequest", false),
            new PermissionInfo("BudgetRequest.Update", "BudgetRequest", "Update BudgetRequest", false),
            new PermissionInfo("BudgetRequest.Delete", "BudgetRequest", "Delete BudgetRequest", false),
            new PermissionInfo("BudgetRequest.ChangeStatus", "BudgetRequest", "Change BudgetRequest status", false),
            new PermissionInfo("BudgetRequestItem.Read", "BudgetRequestItem", "Read BudgetRequestItem", false),
            new PermissionInfo("BudgetRequestItem.Create", "BudgetRequestItem", "Create BudgetRequestItem", false),
            new PermissionInfo("BudgetRequestItem.Update", "BudgetRequestItem", "Update BudgetRequestItem", false),
            new PermissionInfo("BudgetRequestItem.Delete", "BudgetRequestItem", "Delete BudgetRequestItem", false),
            new PermissionInfo("Budget.Read", "Budget", "Read Budget", false),
            new PermissionInfo("Budget.Create", "Budget", "Create Budget", false),
            new PermissionInfo("Budget.Update", "Budget", "Update Budget", false),
            new PermissionInfo("Budget.Delete", "Budget", "Delete Budget", false),
            new PermissionInfo("Budget.ChangeStatus", "Budget", "Change Budget status", false),
            new PermissionInfo("Expense.Read", "Expense", "Read Expense", false),
            new PermissionInfo("Expense.Create", "Expense", "Create Expense", false),
            new PermissionInfo("Expense.Update", "Expense", "Update Expense", false),
            new PermissionInfo("Expense.Delete", "Expense", "Delete Expense", false),
            new PermissionInfo("Expense.ChangeStatus", "Expense", "Change Expense status", false),
            new PermissionInfo("AppUser.Read", "AppUser", "Read users", true),
            new PermissionInfo("AppRole.Read", "AppRole", "Read roles", true),
            new PermissionInfo("AppUser.Create", "AppUser", "Create users", true),
            new PermissionInfo("AppRole.Create", "AppRole", "Create roles", true),
            new PermissionInfo("AppUser.Update", "AppUser", "Update users", true),
            new PermissionInfo("AppRole.Update", "AppRole", "Update roles", true),
            new PermissionInfo("AppUser.Delete", "AppUser", "Delete users", true),
            new PermissionInfo("AppRole.Delete", "AppRole", "Delete roles", true),
            new PermissionInfo("AppRole.AssignPermissions", "AppRole", "Assign permissions to roles", true),
        };
    }
}
