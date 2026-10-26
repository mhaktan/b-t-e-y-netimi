import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

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
};

const DEFAULTS: Partial<BudgetRequestRecord> = {} as Partial<BudgetRequestRecord>;

interface BudgetRequestCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const BudgetRequestCreate: React.FC<BudgetRequestCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<BudgetRequestRecord>>(DEFAULTS);
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<BudgetRequestRecord>) => dataProvider.create('BudgetRequest', values),
    onSuccess: (_data, values) => { triggerFlows('create', 'BudgetRequest', values as Record<string, unknown>); onSuccess(); onClose(); setForm(DEFAULTS); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>{"Yeni Bütçe Talebi"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Talep Başlığı *" value={String(form.title ?? '')} onChange={(v) => setField('title', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Talep Numarası" value={String(form.requestNo ?? '')} onChange={(v) => setField('requestNo', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Açıklama" value={String(form.description ?? '')} onChange={(v) => setField('description', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Gerekçe *" value={String(form.justification ?? '')} onChange={(v) => setField('justification', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Para Birimi *" value={String(form.currency ?? '')} onChange={(v) => setField('currency', v ? Number(v) : null)} searchable={false} options={[{ label: "Türk Lirası", value: "0" }, { label: "Dolar", value: "1" }, { label: "Euro", value: "2" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Toplam Tutar (TL Karşılığı)" value={String(form.totalAmountTRY ?? '')} onChange={(v) => setField('totalAmountTRY', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Talep Tarihi *" value={String(form.requestDate ?? '')} onChange={(v) => setField('requestDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Finans Onay Eşiği (TL)" value={String(form.financeApprovalThreshold ?? '')} onChange={(v) => setField('financeApprovalThreshold', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Yönetici Onaycı" value={String(form.managerApproverId ?? '')} onChange={(v) => setField('managerApproverId', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Finans Onaycı Rolü" resource="AppRole" value={String(form.financeApproverRole ?? '')} onChange={(v) => setField('financeApproverRole', v)} displayField="displayName" valueField="name" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Bütçe *" resource="Budget" value={String(form.budgetId ?? '')} onChange={(v) => setField('budgetId', v)} displayField="title" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Departman *" resource="Department" value={String(form.departmentId ?? '')} onChange={(v) => setField('departmentId', v)} displayField="name" />
                </div>
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <Button variant="primary" appearance="outlined" onClick={onClose}>{"Vazgeç"}</Button>
            <Button variant="primary" appearance="filled" type="submit" disabled={mutation.isPending}>{mutation.isPending ? "Kaydediliyor…" : "Kaydet"}</Button>
          </div>
        </form>
      </div>
    </div>
  );
};
