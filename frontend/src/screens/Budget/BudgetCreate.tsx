import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparCheckbox, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type BudgetRecord = {
  id: string | number;
  title: string;
  allocatedAmount: number;
  currency: string;
  allocatedAmountTRY?: number;
  description?: string;
  isAutoCreated?: boolean;
  status: string;
  revisionNote?: string;
  financeDirectorId?: number;
  budgetPeriodId: string;
  departmentId: string;
  budgetCategoryId: string;
};

const DEFAULTS: Partial<BudgetRecord> = {} as Partial<BudgetRecord>;

interface BudgetCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const BudgetCreate: React.FC<BudgetCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<BudgetRecord>>(DEFAULTS);
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<BudgetRecord>) => dataProvider.create('Budget', values),
    onSuccess: (_data, values) => { triggerFlows('create', 'Budget', values as Record<string, unknown>); onSuccess(); onClose(); setForm(DEFAULTS); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>{"Yeni Bütçe"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Bütçe Başlığı *" value={String(form.title ?? '')} onChange={(v) => setField('title', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Tahsis Edilen Tutar *" value={String(form.allocatedAmount ?? '')} onChange={(v) => setField('allocatedAmount', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Para Birimi *" value={String(form.currency ?? '')} onChange={(v) => setField('currency', v ? Number(v) : null)} searchable={false} options={[{ label: "Türk Lirası", value: "0" }, { label: "Dolar", value: "1" }, { label: "Euro", value: "2" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Tahsis Edilen Tutar (TL Karşılığı)" value={String(form.allocatedAmountTRY ?? '')} onChange={(v) => setField('allocatedAmountTRY', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Açıklama" value={String(form.description ?? '')} onChange={(v) => setField('description', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparCheckbox label="Otomatik Oluşturuldu" checked={!!form.isAutoCreated} onChange={(v) => setField('isAutoCreated', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Finans Direktörü" value={String(form.financeDirectorId ?? '')} onChange={(v) => setField('financeDirectorId', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Bütçe Dönemi *" resource="BudgetPeriod" value={String(form.budgetPeriodId ?? '')} onChange={(v) => setField('budgetPeriodId', v)} displayField="name" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Departman *" resource="Department" value={String(form.departmentId ?? '')} onChange={(v) => setField('departmentId', v)} displayField="name" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Bütçe Kategorisi *" resource="BudgetCategory" value={String(form.budgetCategoryId ?? '')} onChange={(v) => setField('budgetCategoryId', v)} displayField="name" />
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
