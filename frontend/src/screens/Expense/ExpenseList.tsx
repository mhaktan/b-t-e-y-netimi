import React, { useState, useCallback, useEffect, useRef } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Button } from '@takeoff-ui/react-spar';
import { dataProvider } from '../../dataProvider';
import { useListQuery } from '../../shared/useListQuery';
import { useDeleteMutation } from '../../shared/useDeleteMutation';
import { DeleteConfirmDialog } from '../../shared/DeleteConfirmDialog';
import { ListPageLayout } from '../../shared/ListPageLayout';
import type { TableColumn } from '../../shared/ListPageLayout';
import { actionColumn } from '../../shared/ActionButtons';
import { escHtml, formatDate } from '../../shared/format';
import { ExpenseCreate } from './ExpenseCreate';
import { ExpenseEdit } from './ExpenseEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type ExpenseRecord = {
  id: string | number;
  description: string;
  amount: number;
  currency: string;
  amountTRY?: number;
  expenseDate: string;
  receiptNo?: string;
  notes?: string;
  status: string;
  revisionNote?: string;
  expenseApproverId?: number;
  budgetId: string;
  budgetCategoryId: string;
  [key: string]: unknown;
};

// ---------------------------------------------------------------------------
// Column definition — edit this array to add/remove/reorder columns
// ---------------------------------------------------------------------------
//
// Override examples:
//   • Hide a column:     remove its entry from COLUMNS
//   • Add a custom col:  { field: 'fullName', header: 'Full Name', html: (row) => `${row.firstName} ${row.lastName}` }
//   • Enable filtering:  add searchable: true  or  filterType: 'text' | 'checkbox' | 'radio' | 'datepicker'
//   • Custom cell render: html: (row) => `<span style="color:green">${row.status}</span>`
//
// Shared components (src/shared/) can be edited to change behavior globally:
//   • ListPageLayout  — table wrapper, pagination, header layout
//   • ActionButtons   — edit/delete button styles, labels, and behavior
//   • useListQuery    — data fetching, sorting, filtering logic
//   • useDeleteMutation / DeleteConfirmDialog — delete flow
//
// Action buttons override: edit src/shared/ActionButtons.ts DEFAULT_CONFIG
// or pass custom config: actionColumn('id', { hasEdit: true, config: { edit: { label: 'View', style: '...' } } })
//

const COLUMNS: TableColumn[] = [
  { field: 'id', header: 'ID', sortable: true },
  { field: 'description', header: "Harcama Açıklaması", sortable: true, searchable: true, filterType: 'text' },
  { field: 'amount', header: "Tutar", sortable: true, html: (row: Record<string, unknown>) => row.amount == null ? '\u2014' : Number(row.amount).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'currency', header: "Para Birimi", sortable: true, filterType: 'radio', filterOptions: [{ label: "Türk Lirası", value: "0" }, { label: "Dolar", value: "1" }, { label: "Euro", value: "2" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Türk Lirası", ["1"]: "Dolar", ["2"]: "Euro"}; return escHtml(m[String(row.currency ?? '')] ?? String(row.currency ?? '\u2014')); } },
  { field: 'amountTRY', header: "Tutar (TL Karşılığı)", sortable: true, html: (row: Record<string, unknown>) => row.amountTRY == null ? '\u2014' : Number(row.amountTRY).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'expenseDate', header: "Harcama Tarihi", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => formatDate(row.expenseDate, false) },
  { field: 'receiptNo', header: "Fiş/Fatura No", sortable: true, searchable: true, filterType: 'text' },
  { field: 'notes', header: "Notlar", sortable: true, searchable: true, filterType: 'text' },
  { field: 'status', header: "Durum", sortable: true, filterType: 'radio', filterOptions: [{ label: "Taslak", value: "0" }, { label: "Onay Bekliyor", value: "1" }, { label: "Onaylandı", value: "2" }, { label: "Reddedildi", value: "3" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Taslak", ["1"]: "Onay Bekliyor", ["2"]: "Onaylandı", ["3"]: "Reddedildi"}; return escHtml(m[String(row.status ?? '')] ?? String(row.status ?? '\u2014')); } },
  { field: 'revisionNote', header: "Red/Revizyon Notu", sortable: true, searchable: true, filterType: 'text' },
  { field: 'expenseApproverId', header: "Harcama Onaylayıcısı", sortable: true },
  { field: 'budgetDisplay', header: "Bütçe", sortable: false },
  { field: 'budgetCategoryDisplay', header: "Bütçe Kategorisi", sortable: false },
];

// ---------------------------------------------------------------------------
// ExpenseList
// ---------------------------------------------------------------------------

export const ExpenseList: React.FC = () => {
  const list = useListQuery<ExpenseRecord>({ resource: 'Expense' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<ExpenseRecord | null>(null);
  const [selectedRows, setSelectedRows] = useState<ExpenseRecord[]>([]);

  // ?focus=<id> ile gelindiginde kayit dogrudan acilir (My Tasks -> kayit)
  const [searchParams, setSearchParams] = useSearchParams();
  const focusId = searchParams.get('focus');
  const focusedRef = useRef<string | null>(null);
  useEffect(() => {
    if (!focusId || focusedRef.current === focusId) return;
    focusedRef.current = focusId;
    const clear = () => {
      const next = new URLSearchParams(searchParams);
      next.delete('focus');
      setSearchParams(next, { replace: true });
    };
    Promise.resolve(dataProvider.getOne('Expense', focusId))
      .then((rec) => { if (rec) setEditRecord(rec as ExpenseRecord); })
      .catch(() => { /* kayit bulunamadi — liste normal acilir */ })
      .finally(clear);
  }, [focusId]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('Expense', (ids) => { setSelectedRows([]); ids.forEach(id => triggerFlows('delete', 'Expense', { id })); });


  const columns = [...COLUMNS, ...actionColumn('id', { hasEdit: true, hasDelete: true })];

  const handleCrudAction = useCallback((action: string, id: string) => {
    const row = list.records.find((r) => String(r.id) === id);
    if (!row) return;
    if (action === 'edit') setEditRecord(row);
    if (action === 'delete') del.requestSingleDelete(row.id);

  }, [list.records]);

  return (
    <>
      <ListPageLayout
        title={"Harcama"}
        subtitle={Object.keys(list.displayParams).length > 0 ? (
          <div style={{ fontSize: 13, color: '#666', marginTop: 4 }}>
            {Object.entries(list.displayParams).map(([k, v]) => (
              <span key={k} style={{ marginRight: 12 }}>{k}: <strong>{v}</strong></span>
            ))}
          </div>
        ) : undefined}
        records={list.records}
        columns={columns}
        dataKey="id"
        total={list.total}
        loading={list.isLoading}
        page={list.page}
        perPage={list.perPage}
        onPageChange={list.setPage}
        onPerPageChange={list.setPerPage}
        onTableRequest={list.handleTableRequest}
        selectionMode="checkbox"
        selectedRows={selectedRows}
        onSelectionChange={(rows) => setSelectedRows(rows as ExpenseRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <Button variant="danger" appearance="filled" onClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} kayıt`)}>{`Seçilenleri sil (${selectedRows.length})`}</Button>
          )}

          <Button variant="primary" appearance="filled" onClick={() => setShowCreate(true)}>{"+ Yeni Harcama"}</Button>
        </>}
      />

      {list.isError && (
        <div style={{ padding: '10px 14px', background: '#fff3f3', border: '1px solid #f5c6c6', borderRadius: 6, color: '#c62828', fontSize: 13, marginBottom: 12 }}>
          Failed to load data: {(list.error as Error).message}
        </div>
      )}

      <DeleteConfirmDialog
        visible={!!del.deleteTarget}
        label={del.deleteTarget?.label ?? ''}
        isPending={del.isPending}
        onConfirm={del.confirmDelete}
        onCancel={() => del.setDeleteTarget(null)}
      />
      <ExpenseCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <ExpenseEdit record={editRecord} onClose={() => setEditRecord(null)} onSuccess={list.invalidate} />

    </>
  );
};

