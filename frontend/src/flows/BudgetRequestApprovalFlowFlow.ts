// Auto-generated flow: BudgetRequest Approval Flow
// Auto-generated approval flow for BudgetRequest. Customize email templates and add conditions as needed.
// Resource: BudgetRequest
// Enabled: true
//
// Nodes:
  // trigger: On BudgetRequest Submit
  // condition: Status = PendingManagerApproval?
  // approval: BudgetRequest Approval
  // action: Send Approval Email
  // trigger: On BudgetRequest Approved
  // action: Send Completion Email
//
// Edges:
  // On BudgetRequest Submit → Status = PendingManagerApproval?
  // Status = PendingManagerApproval? → BudgetRequest Approval (true)
  // BudgetRequest Approval → Send Approval Email
  // On BudgetRequest Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_BUDGETREQUEST_APPROVAL_FLOW_ID = 'flow-BudgetRequest-approval';
