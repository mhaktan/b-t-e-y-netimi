import React, { useState } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom';
import { isAuthenticated, clearAuth } from './dataProvider';
import { LoginPage } from './screens/LoginPage';
import { GlobalMenu } from './components/GlobalMenu';
import { FlowProvider } from './flows/FlowProvider';
import { DemoBar } from './shared/DemoBar';
import { BudgetPeriodList } from './screens/BudgetPeriod/BudgetPeriodList';
import { DepartmentList } from './screens/Department/DepartmentList';
import { BudgetCategoryList } from './screens/BudgetCategory/BudgetCategoryList';
import { ExchangeRateList } from './screens/ExchangeRate/ExchangeRateList';
import { BudgetRequestList } from './screens/BudgetRequest/BudgetRequestList';
import { BudgetRequestItemList } from './screens/BudgetRequestItem/BudgetRequestItemList';
import { BudgetList } from './screens/Budget/BudgetList';
import { ExpenseList } from './screens/Expense/ExpenseList';
import { DashboardScreen } from './screens/dashboard/DashboardScreen';
import TaskInboxScreen from './screens/tasks/TaskInboxScreen';
import UserListScreen from './admin/UserListScreen';
import RoleListScreen from './admin/RoleListScreen';

const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: 1, staleTime: 0 } },
});

export const App: React.FC = () => {
  const [authenticated, setAuthenticated] = useState(isAuthenticated());


  const handleLogout = () => {
    clearAuth();
    setAuthenticated(false);
    queryClient.clear();
  };

  return (
    <QueryClientProvider client={queryClient}>
      <FlowProvider>
      <BrowserRouter>
        <Routes>
          <Route path="*" element={
            authenticated
              ? (
                <GlobalMenu>
                  <DemoBar />
                  <Routes>
                    <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<DashboardScreen />} />
          <Route path="/BudgetPeriod" element={<BudgetPeriodList />} />
          <Route path="/Department" element={<DepartmentList />} />
          <Route path="/BudgetCategory" element={<BudgetCategoryList />} />
          <Route path="/ExchangeRate" element={<ExchangeRateList />} />
          <Route path="/BudgetRequest" element={<BudgetRequestList />} />
          <Route path="/BudgetRequestItem" element={<BudgetRequestItemList />} />
          <Route path="/Budget" element={<BudgetList />} />
          <Route path="/Expense" element={<ExpenseList />} />
          <Route path="/tasks" element={<TaskInboxScreen />} />
          <Route path="/users" element={<UserListScreen />} />
          <Route path="/roles" element={<RoleListScreen />} />
                  </Routes>
                </GlobalMenu>
              )
              : <LoginPage onLogin={() => setAuthenticated(true)} />
          } />
        </Routes>
      </BrowserRouter>
        </FlowProvider>
    </QueryClientProvider>
  );
};
