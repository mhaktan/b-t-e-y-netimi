// Auto-generated flow: Budget Approval Flow
// Auto-generated approval flow for Budget. Customize email templates and add conditions as needed.
// Resource: Budget
// Enabled: true
//
// Nodes:
  // trigger: On Budget Submit
  // condition: Status = PendingApproval?
  // approval: Budget Approval
  // action: Send Approval Email
  // trigger: On Budget Approved
  // action: Send Completion Email
//
// Edges:
  // On Budget Submit → Status = PendingApproval?
  // Status = PendingApproval? → Budget Approval (true)
  // Budget Approval → Send Approval Email
  // On Budget Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_BUDGET_APPROVAL_FLOW_ID = 'flow-Budget-approval';
