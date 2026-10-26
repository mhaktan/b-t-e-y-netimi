import React from 'react';
import { UiCard } from '../../shared/ui';

import {
  ResponsiveContainer,
  LineChart, Line,
  BarChart, Bar,
  PieChart, Pie, Cell,
  CartesianGrid, XAxis, YAxis, Tooltip,
} from 'recharts';

import { API_BASE } from '../../config';
import { authFetch } from '../../dataProvider';
import { formatDate } from '../../shared/format';
const cellText = (v: unknown): string => typeof v === 'boolean' ? (v ? "Evet" : "Hayır") : typeof v === 'string' && /^\d{4}-\d{2}-\d{2}T/.test(v) ? formatDate(v, !/T00:00:00/.test(v)) : String(v ?? '');


const LOADING_KEYFRAMES = `
@keyframes pulse { 0%, 100% { opacity: 1; } 50% { opacity: 0.4; } }
@keyframes spin { to { transform: rotate(360deg); } }
`;

export const DashboardScreen: React.FC = () => {
  const getNestedValue = (obj: Record<string, unknown>, path: string): unknown => {
    const direct = path.split('.').reduce<unknown>((o, k) => (o && typeof o === 'object') ? (o as Record<string, unknown>)[k] : undefined, obj);
    if (direct !== undefined) return direct;
    // Fallback: try last segment at top level (handles ABP-style {result: {...}} unwrapping)
    const segments = path.split('.');
    if (segments.length > 1 && obj && typeof obj === "object") {
      const lastKey = segments[segments.length - 1];
      const top = (obj as Record<string, unknown>)[lastKey];
      if (top !== undefined) return top;
    }
    return undefined;
  };

  const sharedGet = (url: string): Promise<Response> => {
    const cache = ((window as unknown as { __dashCache?: Map<string, Promise<Response>> }).__dashCache ??= new Map());
    let p = cache.get(url);
    if (!p) { p = authFetch(url); cache.set(url, p); setTimeout(() => cache.delete(url), 10_000); }
    return p.then((r: Response) => r.clone());
  };

  // Unwrap common API envelopes: ABP {result, __abp}, generic {data}, etc.
  const unwrapResponse = (json: unknown): unknown => {
    if (!json || typeof json !== "object" || Array.isArray(json)) return json;
    const obj = json as Record<string, unknown>;
    // ABP envelope: {result, success, error, __abp}
    if ("__abp" in obj && "result" in obj) return obj.result;
    // Generic envelope: {success: true, data: X}
    if ("success" in obj && "data" in obj && Object.keys(obj).length <= 4) return obj.data;
    return json;
  };

  const extractArray = (json: unknown): Record<string, unknown>[] => {
    if (Array.isArray(json)) return json;
    if (json && typeof json === 'object') {
      const obj = json as Record<string, unknown>;
      for (const key of ['items', 'data', 'results', 'records', 'rows', 'list']) {
        if (Array.isArray(obj[key])) return obj[key] as Record<string, unknown>[];
      }
      // Recurse one level — handles {result: {items: [...]}}
      for (const val of Object.values(obj)) {
        if (val && typeof val === 'object' && !Array.isArray(val)) {
          const inner = val as Record<string, unknown>;
          for (const key of ['items', 'data', 'results', 'records', 'rows', 'list']) {
            if (Array.isArray(inner[key])) return inner[key] as Record<string, unknown>[];
          }
        }
        if (Array.isArray(val)) return val as Record<string, unknown>[];
      }
    }
    return [];
  };

  const [count_budgetRequest_0Data, setCount_budgetRequest_0Data] = React.useState<Record<string, unknown> | null>(null);
  const [count_budgetRequest_0Loading, setCount_budgetRequest_0Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await sharedGet(`${API_BASE}/api/services/app/BudgetRequest/GetAll?StatusIn=1%2C2&MaxResultCount=1`);
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        setCount_budgetRequest_0Data(json);
      } catch { /* ignore */ }
      finally { setCount_budgetRequest_0Loading(false); }
    };
    fetchData();
  }, []);

  const [mytasks_1Data, setMytasks_1Data] = React.useState<Record<string, unknown> | null>(null);
  const [mytasks_1Loading, setMytasks_1Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await sharedGet(`${API_BASE}/api/services/app/Approval/GetMyPendingApprovals`);
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        setMytasks_1Data(json);
      } catch { /* ignore */ }
      finally { setMytasks_1Loading(false); }
    };
    fetchData();
    const timer = setInterval(fetchData, 30000);
    return () => clearInterval(timer);
  }, []);

  const [count_budget_2Data, setCount_budget_2Data] = React.useState<Record<string, unknown> | null>(null);
  const [count_budget_2Loading, setCount_budget_2Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await sharedGet(`${API_BASE}/api/services/app/Budget/GetAll?Status=2&MaxResultCount=1`);
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        setCount_budget_2Data(json);
      } catch { /* ignore */ }
      finally { setCount_budget_2Loading(false); }
    };
    fetchData();
  }, []);

  const [count_budgetRequest_3Data, setCount_budgetRequest_3Data] = React.useState<Record<string, unknown> | null>(null);
  const [count_budgetRequest_3Loading, setCount_budgetRequest_3Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await sharedGet(`${API_BASE}/api/services/app/BudgetRequest/GetAll?Status=3&MaxResultCount=1`);
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        setCount_budgetRequest_3Data(json);
      } catch { /* ignore */ }
      finally { setCount_budgetRequest_3Loading(false); }
    };
    fetchData();
  }, []);

  const [breakdown_budgetRequest_4Data, setBreakdown_budgetRequest_4Data] = React.useState<Record<string, unknown>[]>([]);
  const [breakdown_budgetRequest_4Loading, setBreakdown_budgetRequest_4Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await sharedGet(`${API_BASE}/api/services/app/BudgetRequest/GetGroupedCount?GroupBy=Status`);
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result') ?? getNestedValue(json, 'result');
        setBreakdown_budgetRequest_4Data(extractArray(target ?? json));
      } catch { /* ignore */ }
      finally { setBreakdown_budgetRequest_4Loading(false); }
    };
    fetchData();
  }, []);

  const [breakdown_budgetRequest_5Data, setBreakdown_budgetRequest_5Data] = React.useState<Record<string, unknown>[]>([]);
  const [breakdown_budgetRequest_5Loading, setBreakdown_budgetRequest_5Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await sharedGet(`${API_BASE}/api/services/app/BudgetRequest/GetGroupedCount?GroupBy=DepartmentId`);
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result') ?? getNestedValue(json, 'result');
        setBreakdown_budgetRequest_5Data(extractArray(target ?? json));
      } catch { /* ignore */ }
      finally { setBreakdown_budgetRequest_5Loading(false); }
    };
    fetchData();
  }, []);

  const [breakdown_expense_6Data, setBreakdown_expense_6Data] = React.useState<Record<string, unknown>[]>([]);
  const [breakdown_expense_6Loading, setBreakdown_expense_6Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await sharedGet(`${API_BASE}/api/services/app/Expense/GetGroupedCount?GroupBy=BudgetCategoryId`);
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result') ?? getNestedValue(json, 'result');
        setBreakdown_expense_6Data(extractArray(target ?? json));
      } catch { /* ignore */ }
      finally { setBreakdown_expense_6Loading(false); }
    };
    fetchData();
  }, []);

  const [list_budgetRequestItem_7_tableData, setList_budgetRequestItem_7_tableData] = React.useState<Record<string, unknown> | null>(null);
  const [list_budgetRequestItem_7_tableLoading, setList_budgetRequestItem_7_tableLoading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await sharedGet(`${API_BASE}/api/services/app/BudgetRequestItem/GetAll?PlannedDateTo=2026-10-26&MaxResultCount=10&Sorting=id%20desc`);
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result.items') ?? getNestedValue(json, 'result.items');
        setList_budgetRequestItem_7_tableData(target != null ? (target as Record<string, unknown>) : json);
      } catch { /* ignore */ }
      finally { setList_budgetRequestItem_7_tableLoading(false); }
    };
    fetchData();
  }, []);

  return (
    <div>
      <style dangerouslySetInnerHTML={{ __html: LOADING_KEYFRAMES }} />
      <h1 style={{ margin: '0 0 24px', fontSize: 22, fontWeight: 700 }}>{"Gösterge Paneli"}</h1>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, 1fr)', gap: 16 }}>
        <div style={{ gridColumn: 'span 12' }}>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: 16 }}>
            <div style={{ gridColumn: 'span 1' }}>
              {count_budgetRequest_0Loading ? (
                <UiCard bodyStyle={{ padding: 20 }}>
                  <div style={{ height: 12, width: '40%', background: '#e0e0e0', borderRadius: 4, marginBottom: 12, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 28, width: '60%', background: '#e0e0e0', borderRadius: 4, marginBottom: 8, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 10, width: '50%', background: '#f0f0f0', borderRadius: 4, animation: 'pulse 1.5s ease-in-out infinite' }} />
                </UiCard>
              ) : (
              <UiCard bodyStyle={{ padding: 20 }}>
                <div style={{ fontSize: 12, color: '#888', marginBottom: 4 }}>Onay Bekleyen Talepler</div>
                <div style={{ fontSize: 28, fontWeight: 700, color: '#1976d2' }}>{(getNestedValue(count_budgetRequest_0Data ?? {}, 'result.totalCount') as string | number) ?? '—'}</div>
              </UiCard>
              )}

            </div>
            <div style={{ gridColumn: 'span 1' }}>
              {mytasks_1Loading ? (
                <UiCard bodyStyle={{ padding: 20 }}>
                  <div style={{ height: 12, width: '40%', background: '#e0e0e0', borderRadius: 4, marginBottom: 12, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 28, width: '60%', background: '#e0e0e0', borderRadius: 4, marginBottom: 8, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 10, width: '50%', background: '#f0f0f0', borderRadius: 4, animation: 'pulse 1.5s ease-in-out infinite' }} />
                </UiCard>
              ) : (
              <UiCard bodyStyle={{ padding: 20 }}>
                <div style={{ fontSize: 12, color: '#888', marginBottom: 4 }}>Onayımı Bekleyen İşler</div>
                <div style={{ fontSize: 28, fontWeight: 700, color: '#4caf50' }}>{Array.isArray((Array.isArray(mytasks_1Data) ? mytasks_1Data as unknown[] : mytasks_1Data?.['result'])) ? ((Array.isArray(mytasks_1Data) ? mytasks_1Data as unknown[] : mytasks_1Data?.['result']) as unknown[]).length : (((Array.isArray(mytasks_1Data) ? mytasks_1Data as unknown[] : mytasks_1Data?.['result']) as string | number) ?? '—')}</div>
              </UiCard>
              )}

            </div>
            <div style={{ gridColumn: 'span 1' }}>
              {count_budget_2Loading ? (
                <UiCard bodyStyle={{ padding: 20 }}>
                  <div style={{ height: 12, width: '40%', background: '#e0e0e0', borderRadius: 4, marginBottom: 12, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 28, width: '60%', background: '#e0e0e0', borderRadius: 4, marginBottom: 8, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 10, width: '50%', background: '#f0f0f0', borderRadius: 4, animation: 'pulse 1.5s ease-in-out infinite' }} />
                </UiCard>
              ) : (
              <UiCard bodyStyle={{ padding: 20 }}>
                <div style={{ fontSize: 12, color: '#888', marginBottom: 4 }}>Aktif Bütçeler</div>
                <div style={{ fontSize: 28, fontWeight: 700, color: '#ff9800' }}>{(getNestedValue(count_budget_2Data ?? {}, 'result.totalCount') as string | number) ?? '—'}</div>
              </UiCard>
              )}

            </div>
            <div style={{ gridColumn: 'span 1' }}>
              {count_budgetRequest_3Loading ? (
                <UiCard bodyStyle={{ padding: 20 }}>
                  <div style={{ height: 12, width: '40%', background: '#e0e0e0', borderRadius: 4, marginBottom: 12, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 28, width: '60%', background: '#e0e0e0', borderRadius: 4, marginBottom: 8, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 10, width: '50%', background: '#f0f0f0', borderRadius: 4, animation: 'pulse 1.5s ease-in-out infinite' }} />
                </UiCard>
              ) : (
              <UiCard bodyStyle={{ padding: 20 }}>
                <div style={{ fontSize: 12, color: '#888', marginBottom: 4 }}>Onaylanan Talepler</div>
                <div style={{ fontSize: 28, fontWeight: 700, color: '#e91e63' }}>{(getNestedValue(count_budgetRequest_3Data ?? {}, 'result.totalCount') as string | number) ?? '—'}</div>
              </UiCard>
              )}

            </div>
          </div>
        </div>
        <div style={{ gridColumn: 'span 6' }}>
          {breakdown_budgetRequest_4Loading ? (
            <UiCard header="Talep Durumu Dağılımı" bodyStyle={{ padding: 16, display: 'flex', alignItems: 'center', justifyContent: 'center', height: 280 }}>
              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12 }}>
                <div style={{ width: 32, height: 32, border: '3px solid #e0e0e0', borderTopColor: '#1976d2', borderRadius: '50%', animation: 'spin 0.8s linear infinite' }} />
                <span style={{ fontSize: 12, color: '#999' }}>{"Yükleniyor…"}</span>
              </div>
            </UiCard>
          ) : (
          <UiCard header="Talep Durumu Dağılımı" bodyStyle={{ padding: 16 }}>
            {breakdown_budgetRequest_4Data.filter((d) => Number(d['count']) > 0).length <= 1 ? (
              <div style={{ height: 140, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: 4 }}>
                {breakdown_budgetRequest_4Data.some((d) => Number(d['count']) > 0) ? (<>
                  <div style={{ fontSize: 32, fontWeight: 700, color: '#1a1a2e' }}>{String(breakdown_budgetRequest_4Data.find((d) => Number(d['count']) > 0)?.['count'] ?? '')}</div>
                  <div style={{ fontSize: 13, color: '#777' }}>{String(breakdown_budgetRequest_4Data.find((d) => Number(d['count']) > 0)?.['label'] ?? '')}</div>
                </>) : <span style={{ fontSize: 13, color: '#999' }}>{"Kayıt yok"}</span>}
              </div>
            ) : (
              <ResponsiveContainer width="100%" height={280}>
                  <PieChart>
                    <Pie data={breakdown_budgetRequest_4Data} dataKey="count" nameKey="label" cx="50%" cy="50%" outerRadius={80} label>
                      <Cell fill="#1976d2" />
                      <Cell fill="#ff9800" />
                      <Cell fill="#4caf50" />
                      <Cell fill="#e91e63" />
                      <Cell fill="#9c27b0" />
                    </Pie>
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
            )}
          </UiCard>
          )}
        </div>
        <div style={{ gridColumn: 'span 6' }}>
          {breakdown_budgetRequest_5Loading ? (
            <UiCard header="Departmana Göre Talepler" bodyStyle={{ padding: 16, display: 'flex', alignItems: 'center', justifyContent: 'center', height: 280 }}>
              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12 }}>
                <div style={{ width: 32, height: 32, border: '3px solid #e0e0e0', borderTopColor: '#1976d2', borderRadius: '50%', animation: 'spin 0.8s linear infinite' }} />
                <span style={{ fontSize: 12, color: '#999' }}>{"Yükleniyor…"}</span>
              </div>
            </UiCard>
          ) : (
          <UiCard header="Departmana Göre Talepler" bodyStyle={{ padding: 16 }}>
            {breakdown_budgetRequest_5Data.filter((d) => Number(d['count']) > 0).length <= 1 ? (
              <div style={{ height: 140, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: 4 }}>
                {breakdown_budgetRequest_5Data.some((d) => Number(d['count']) > 0) ? (<>
                  <div style={{ fontSize: 32, fontWeight: 700, color: '#1a1a2e' }}>{String(breakdown_budgetRequest_5Data.find((d) => Number(d['count']) > 0)?.['count'] ?? '')}</div>
                  <div style={{ fontSize: 13, color: '#777' }}>{String(breakdown_budgetRequest_5Data.find((d) => Number(d['count']) > 0)?.['label'] ?? '')}</div>
                </>) : <span style={{ fontSize: 13, color: '#999' }}>{"Kayıt yok"}</span>}
              </div>
            ) : (
              <ResponsiveContainer width="100%" height={280}>
                  <BarChart data={breakdown_budgetRequest_5Data}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="label" />
                    <YAxis />
                    <Tooltip />
                    <Bar dataKey="count" fill="#1976d2" />
                  </BarChart>
                </ResponsiveContainer>
            )}
          </UiCard>
          )}
        </div>
        <div style={{ gridColumn: 'span 6' }}>
          {breakdown_expense_6Loading ? (
            <UiCard header="Kategoriye Göre Harcamalar" bodyStyle={{ padding: 16, display: 'flex', alignItems: 'center', justifyContent: 'center', height: 280 }}>
              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12 }}>
                <div style={{ width: 32, height: 32, border: '3px solid #e0e0e0', borderTopColor: '#1976d2', borderRadius: '50%', animation: 'spin 0.8s linear infinite' }} />
                <span style={{ fontSize: 12, color: '#999' }}>{"Yükleniyor…"}</span>
              </div>
            </UiCard>
          ) : (
          <UiCard header="Kategoriye Göre Harcamalar" bodyStyle={{ padding: 16 }}>
            {breakdown_expense_6Data.filter((d) => Number(d['count']) > 0).length <= 1 ? (
              <div style={{ height: 140, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: 4 }}>
                {breakdown_expense_6Data.some((d) => Number(d['count']) > 0) ? (<>
                  <div style={{ fontSize: 32, fontWeight: 700, color: '#1a1a2e' }}>{String(breakdown_expense_6Data.find((d) => Number(d['count']) > 0)?.['count'] ?? '')}</div>
                  <div style={{ fontSize: 13, color: '#777' }}>{String(breakdown_expense_6Data.find((d) => Number(d['count']) > 0)?.['label'] ?? '')}</div>
                </>) : <span style={{ fontSize: 13, color: '#999' }}>{"Kayıt yok"}</span>}
              </div>
            ) : (
              <ResponsiveContainer width="100%" height={280}>
                  <BarChart data={breakdown_expense_6Data}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="label" />
                    <YAxis />
                    <Tooltip />
                    <Bar dataKey="count" fill="#1976d2" />
                  </BarChart>
                </ResponsiveContainer>
            )}
          </UiCard>
          )}
        </div>
        <div style={{ gridColumn: 'span 12' }}>
          <h3 style={{ fontSize: 16, fontWeight: 600, margin: '0 0 12px' }}>Planlanan Harcama Tarihi Geçmiş Kalemler</h3>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(1, 1fr)', gap: 16 }}>
            <div style={{ gridColumn: 'span 1' }}>
              <div style={{ background: '#fff', borderRadius: 8, overflow: 'hidden', border: '1px solid #e8e8e8' }}>
                {list_budgetRequestItem_7_tableLoading ? (
                  <div style={{ padding: 20, textAlign: 'center', color: '#999' }}>Loading...</div>
                ) : (
                  <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                    <thead><tr><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Description</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Amount</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Planned Date</th></tr></thead>
                    <tbody>
                      {(Array.isArray(list_budgetRequestItem_7_tableData) ? list_budgetRequestItem_7_tableData : extractArray(list_budgetRequestItem_7_tableData)).map((row: Record<string, unknown>, i: number) => (
                        <tr key={i}><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{cellText(row["description"])}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{cellText(row["amount"])}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{cellText(row["plannedDate"])}</td></tr>
                      ))}
                    </tbody>
                  </table>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
