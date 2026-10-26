import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparCheckbox, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';
import { ChildRecords } from '../../shared/ChildRecords';

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
const WORKFLOW: { statusField: string; statuses: string[]; actions: Array<{ action: string; label?: string; from: number[]; anyFrom?: boolean; needsNote?: boolean }>; locked: number[] } = {"statusField":"status","statuses":["Taslak","Onay Bekliyor","Aktif","Kapalı","İptal"],"actions":[{"action":"Submit","label":"Gönder","from":[0]},{"action":"Close","label":"Kapat","from":[2]},{"action":"Cancel","label":"İptal et","from":[],"anyFrom":true}],"locked":[1,2,4]};

interface BudgetEditProps {
  record: BudgetRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const BudgetEdit: React.FC<BudgetEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<BudgetRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();
  const [wfBusy, setWfBusy] = useState(false);
  const status = Number((record as Record<string, unknown> | null)?.[WORKFLOW.statusField]);
  const locked = WORKFLOW.locked.includes(status);
  const wfActions = WORKFLOW.actions.filter((a) => a.anyFrom || a.from.includes(status));
  const runAction = async (a: { action: string; label?: string; needsNote?: boolean }) => {
    const note = a.needsNote ? window.prompt(`${a.label ?? a.action}: açıklama`) ?? '' : undefined;
    if (a.needsNote && !note?.trim()) return;
    setWfBusy(true);
    try {
      await dataProvider.changeStatus('Budget', record!.id, a.action, note);
      onSuccess(); onClose();
    } catch (err) {
      window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: (err as Error).message } }));
    } finally { setWfBusy(false); }
  };

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<BudgetRecord>) =>
      dataProvider.update('Budget', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'Budget', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, display: 'flex', alignItems: 'center' }}>{"Bütçe düzenle"}
          <span style={{ marginLeft: 12, fontSize: 12, fontWeight: 600, padding: '2px 10px', borderRadius: 12, background: locked ? '#fff3e0' : '#e8f5e9', color: locked ? '#e65100' : '#2e7d32' }}>{WORKFLOW.statuses[status] ?? ''}</span></h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>

            {locked && <div role="status" style={{ marginBottom: 16, padding: '8px 12px', borderRadius: 8, background: '#fff3e0', color: '#e65100', fontSize: 13 }}>Bu kayıt onay sürecinde ya da onaylanmış; düzenlenemez.</div>}
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
              {record && [{"resource":"BudgetRequest","label":"Bütçe Talebi","fk":"budgetId","columns":[{"field":"title","label":"Talep Başlığı","type":"text"},{"field":"requestNo","label":"Talep Numarası","type":"text"},{"field":"description","label":"Açıklama","type":"text"}]},{"resource":"Expense","label":"Harcama","fk":"budgetId","columns":[{"field":"description","label":"Harcama Açıklaması","type":"text"},{"field":"amount","label":"Tutar","type":"currency"},{"field":"currency","label":"Para Birimi","type":"select"}]}].map((c) => <ChildRecords key={c.resource + c.fk} {...c} parentId={record.id as string | number} />)}
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <div style={{ marginRight: 'auto', display: 'flex', gap: 8 }}>
              {wfActions.map((a) => (
                <React.Fragment key={a.action}>
                  <Button variant="primary" appearance="outlined" onClick={() => void runAction(a)} disabled={wfBusy}>{`${a.label ?? a.action}`}</Button>
                </React.Fragment>
              ))}
            </div>
            <Button variant="primary" appearance="outlined" onClick={onClose}>{"Vazgeç"}</Button>
            {!locked && <Button variant="primary" appearance="filled" type="submit" disabled={mutation.isPending}>{mutation.isPending ? "Kaydediliyor…" : "Kaydet"}</Button>}
          </div>
        </form>
      </div>
    </div>
  );
};
