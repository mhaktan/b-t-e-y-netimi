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
import { BudgetRequestCreate } from './BudgetRequestCreate';
import { BudgetRequestEdit } from './BudgetRequestEdit';
import { usePdfGenerate } from '../../pdf/usePdfGenerate';
import pdfTemplate from '../../pdf-templates/b-t-e-talep-raporu.json';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type BudgetRequestRecord = {
  id: string | number;
  title: string;
  requestNo?: string;
  description?: string;
  justification: string;
  totalAmount?: number;
  currency: string;
  totalAmountTRY?: number;
  requestDate: string;
  status: string;
  revisionNote?: string;
  financeApprovalThreshold?: number;
  managerApproverId?: number;
  financeApproverRole?: string;
  budgetId: string;
  departmentId: string;
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
  { field: 'title', header: "Talep Başlığı", sortable: true, searchable: true, filterType: 'text' },
  { field: 'requestNo', header: "Talep Numarası", sortable: true, searchable: true, filterType: 'text' },
  { field: 'description', header: "Açıklama", sortable: true, searchable: true, filterType: 'text' },
  { field: 'justification', header: "Gerekçe", sortable: true, searchable: true, filterType: 'text' },
  { field: 'totalAmount', header: "Toplam Tutar", sortable: true, html: (row: Record<string, unknown>) => row.totalAmount == null ? '\u2014' : Number(row.totalAmount).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'currency', header: "Para Birimi", sortable: true, filterType: 'radio', filterOptions: [{ label: "Türk Lirası", value: "0" }, { label: "Dolar", value: "1" }, { label: "Euro", value: "2" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Türk Lirası", ["1"]: "Dolar", ["2"]: "Euro"}; return escHtml(m[String(row.currency ?? '')] ?? String(row.currency ?? '\u2014')); } },
  { field: 'totalAmountTRY', header: "Toplam Tutar (TL Karşılığı)", sortable: true, html: (row: Record<string, unknown>) => row.totalAmountTRY == null ? '\u2014' : Number(row.totalAmountTRY).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'requestDate', header: "Talep Tarihi", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => formatDate(row.requestDate, false) },
  { field: 'status', header: "Durum", sortable: true, filterType: 'radio', filterOptions: [{ label: "Taslak", value: "0" }, { label: "Yönetici Onayında", value: "1" }, { label: "Finans Onayında", value: "2" }, { label: "Onaylandı", value: "3" }, { label: "Revizyon", value: "4" }, { label: "Reddedildi", value: "5" }, { label: "İptal", value: "6" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Taslak", ["1"]: "Yönetici Onayında", ["2"]: "Finans Onayında", ["3"]: "Onaylandı", ["4"]: "Revizyon", ["5"]: "Reddedildi", ["6"]: "İptal"}; return escHtml(m[String(row.status ?? '')] ?? String(row.status ?? '\u2014')); } },
  { field: 'revisionNote', header: "Revizyon Notu", sortable: true, searchable: true, filterType: 'text' },
  { field: 'financeApprovalThreshold', header: "Finans Onay Eşiği (TL)", sortable: true, html: (row: Record<string, unknown>) => row.financeApprovalThreshold == null ? '\u2014' : Number(row.financeApprovalThreshold).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'managerApproverId', header: "Yönetici Onaycı", sortable: true },
  { field: 'financeApproverRole', header: "Finans Onaycı Rolü", sortable: true },
  { field: 'budgetDisplay', header: "Bütçe", sortable: false },
  { field: 'departmentDisplay', header: "Departman", sortable: false },
];

// ---------------------------------------------------------------------------
// BudgetRequestList
// ---------------------------------------------------------------------------

export const BudgetRequestList: React.FC = () => {
  const list = useListQuery<BudgetRequestRecord>({ resource: 'BudgetRequest' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<BudgetRequestRecord | null>(null);
  const [selectedRows, setSelectedRows] = useState<BudgetRequestRecord[]>([]);

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
    Promise.resolve(dataProvider.getOne('BudgetRequest', focusId))
      .then((rec) => { if (rec) setEditRecord(rec as BudgetRequestRecord); })
      .catch(() => { /* kayit bulunamadi — liste normal acilir */ })
      .finally(clear);
  }, [focusId]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('BudgetRequest', (ids) => { setSelectedRows([]); ids.forEach(id => triggerFlows('delete', 'BudgetRequest', { id })); });

  // PDF generation
  const { blobUrl: pdfBlobUrl, loading: pdfLoading, generate: generatePdf } = usePdfGenerate();
  const [showPdf, setShowPdf] = useState(false);
  const handlePdfGenerate = (params?: Record<string, string>) => {
    setShowPdf(true);
    generatePdf(pdfTemplate as any, params).catch((e: unknown) => { setShowPdf(false); alert(e instanceof Error ? e.message : String(e)); });
  };

  const columns = [...COLUMNS, ...actionColumn('id', { hasEdit: true, hasDelete: true, pdfLabel: 'PDF' })];

  const handleCrudAction = useCallback((action: string, id: string) => {
    const row = list.records.find((r) => String(r.id) === id);
    if (!row) return;
    if (action === 'edit') setEditRecord(row);
    if (action === 'delete') del.requestSingleDelete(row.id);
    if (action === 'pdf') handlePdfGenerate({ id });
  }, [list.records]);

  return (
    <>
      <ListPageLayout
        title={"Bütçe Talebi"}
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
        onSelectionChange={(rows) => setSelectedRows(rows as BudgetRequestRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <Button variant="danger" appearance="filled" onClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} kayıt`)}>{`Seçilenleri sil (${selectedRows.length})`}</Button>
          )}

          <Button variant="primary" appearance="filled" onClick={() => setShowCreate(true)}>{"+ Yeni Bütçe Talebi"}</Button>
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
      <BudgetRequestCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <BudgetRequestEdit record={editRecord} onClose={() => setEditRecord(null)} onSuccess={list.invalidate} />

      {/* PDF Preview Modal */}
      {showPdf && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.5)', zIndex: 1000, display: 'flex', alignItems: 'center', justifyContent: 'center' }} onClick={() => setShowPdf(false)}>
          <div style={{ background: '#fff', borderRadius: 8, width: '80%', maxWidth: 900, height: '80vh', display: 'flex', flexDirection: 'column', overflow: 'hidden' }} onClick={(e) => e.stopPropagation()}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '12px 16px', borderBottom: '1px solid #e0e0e0' }}>
              <span style={{ fontWeight: 600, fontSize: 15 }}>PDF Preview</span>
              <div style={{ display: 'flex', gap: 8 }}>
                {pdfBlobUrl && <a href={pdfBlobUrl} download="b-t-e-talep-raporu.pdf" style={{ padding: '4px 12px', background: '#1976d2', color: '#fff', borderRadius: 4, textDecoration: 'none', fontSize: 13 }}>Download</a>}
                <button onClick={() => setShowPdf(false)} style={{ background: 'none', border: 'none', fontSize: 18, cursor: 'pointer', color: '#666' }}>✕</button>
              </div>
            </div>
            <div style={{ flex: 1, padding: 8, background: '#f0f0f0' }}>
              {pdfLoading ? (
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%', color: '#999' }}>Generating PDF...</div>
              ) : pdfBlobUrl ? (
                <iframe src={pdfBlobUrl} style={{ width: '100%', height: '100%', border: 'none', borderRadius: 4 }} title="PDF Preview" />
              ) : null}
            </div>
          </div>
        </div>
      )}
    </>
  );
};

