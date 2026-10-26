// Auto-generated flow: Expense Approval Flow
// Auto-generated approval flow for Expense. Customize email templates and add conditions as needed.
// Resource: Expense
// Enabled: true
//
// Nodes:
  // trigger: On Expense Submit
  // condition: Status = PendingApproval?
  // approval: Expense Approval
  // action: Send Approval Email
  // trigger: On Expense Approved
  // action: Send Completion Email
//
// Edges:
  // On Expense Submit → Status = PendingApproval?
  // Status = PendingApproval? → Expense Approval (true)
  // Expense Approval → Send Approval Email
  // On Expense Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_EXPENSE_APPROVAL_FLOW_ID = 'flow-Expense-approval';
