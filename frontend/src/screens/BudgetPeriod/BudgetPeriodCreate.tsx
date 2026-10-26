import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparCheckbox, SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { useFlows } from '../../flows/FlowProvider';

type BudgetPeriodRecord = {
  id: string | number;
  name: string;
  year: number;
  startDate: string;
  endDate: string;
  description?: string;
  isActive?: boolean;
};

const DEFAULTS: Partial<BudgetPeriodRecord> = {"isActive":true} as Partial<BudgetPeriodRecord>;

interface BudgetPeriodCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const BudgetPeriodCreate: React.FC<BudgetPeriodCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<BudgetPeriodRecord>>(DEFAULTS);
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<BudgetPeriodRecord>) => dataProvider.create('BudgetPeriod', values),
    onSuccess: (_data, values) => { triggerFlows('create', 'BudgetPeriod', values as Record<string, unknown>); onSuccess(); onClose(); setForm(DEFAULTS); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>{"Yeni Bütçe Dönemi"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Dönem Adı *" value={String(form.name ?? '')} onChange={(v) => setField('name', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Yıl *" value={String(form.year ?? '')} onChange={(v) => setField('year', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Başlangıç Tarihi *" value={String(form.startDate ?? '')} onChange={(v) => setField('startDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Bitiş Tarihi *" value={String(form.endDate ?? '')} onChange={(v) => setField('endDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Açıklama" value={String(form.description ?? '')} onChange={(v) => setField('description', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparCheckbox label="Aktif" checked={!!form.isActive} onChange={(v) => setField('isActive', v)} />
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
