import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';
import { ChildRecords } from '../../shared/ChildRecords';

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
const WORKFLOW: { statusField: string; statuses: string[]; actions: Array<{ action: string; label?: string; from: number[]; anyFrom?: boolean; needsNote?: boolean }>; locked: number[] } = {"statusField":"status","statuses":["Taslak","Yönetici Onayında","Finans Onayında","Onaylandı","Revizyon","Reddedildi","İptal"],"actions":[{"action":"Submit","label":"Gönder","from":[0]},{"action":"Resubmit","label":"Yeniden gönder","from":[4]},{"action":"Cancel","label":"İptal et","from":[],"anyFrom":true}],"locked":[1,2,3,6]};

interface BudgetRequestEditProps {
  record: BudgetRequestRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const BudgetRequestEdit: React.FC<BudgetRequestEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<BudgetRequestRecord>>(record ?? {});
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
      await dataProvider.changeStatus('BudgetRequest', record!.id, a.action, note);
      onSuccess(); onClose();
    } catch (err) {
      window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: (err as Error).message } }));
    } finally { setWfBusy(false); }
  };

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<BudgetRequestRecord>) =>
      dataProvider.update('BudgetRequest', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'BudgetRequest', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, display: 'flex', alignItems: 'center' }}>{"Bütçe Talebi düzenle"}
          <span style={{ marginLeft: 12, fontSize: 12, fontWeight: 600, padding: '2px 10px', borderRadius: 12, background: locked ? '#fff3e0' : '#e8f5e9', color: locked ? '#e65100' : '#2e7d32' }}>{WORKFLOW.statuses[status] ?? ''}</span></h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>

            {locked && <div role="status" style={{ marginBottom: 16, padding: '8px 12px', borderRadius: 8, background: '#fff3e0', color: '#e65100', fontSize: 13 }}>Bu kayıt onay sürecinde ya da onaylanmış; düzenlenemez.</div>}
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
              {record && [{"resource":"BudgetRequestItem","label":"Bütçe Talep Kalemi","fk":"budgetRequestId","columns":[{"field":"description","label":"Kalem Açıklaması","type":"text"},{"field":"amount","label":"Tutar","type":"currency"},{"field":"quantity","label":"Miktar","type":"currency"}]}].map((c) => <ChildRecords key={c.resource + c.fk} {...c} parentId={record.id as string | number} />)}
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
