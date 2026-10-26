import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { useFlows } from '../../flows/FlowProvider';
import { ChildRecords } from '../../shared/ChildRecords';

type DepartmentRecord = {
  id: string | number;
  name: string;
  code: string;
  description?: string;
};

interface DepartmentEditProps {
  record: DepartmentRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const DepartmentEdit: React.FC<DepartmentEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<DepartmentRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<DepartmentRecord>) =>
      dataProvider.update('Department', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'Department', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, display: 'flex', alignItems: 'center' }}>{"Departman düzenle"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>

            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Departman Adı *" value={String(form.name ?? '')} onChange={(v) => setField('name', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Departman Kodu *" value={String(form.code ?? '')} onChange={(v) => setField('code', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Açıklama" value={String(form.description ?? '')} onChange={(v) => setField('description', v)} />
                </div>
              {record && [{"resource":"Budget","label":"Bütçe","fk":"departmentId","columns":[{"field":"title","label":"Bütçe Başlığı","type":"text"},{"field":"allocatedAmount","label":"Tahsis Edilen Tutar","type":"currency"},{"field":"currency","label":"Para Birimi","type":"select"}]},{"resource":"BudgetRequest","label":"Bütçe Talebi","fk":"departmentId","columns":[{"field":"title","label":"Talep Başlığı","type":"text"},{"field":"requestNo","label":"Talep Numarası","type":"text"},{"field":"description","label":"Açıklama","type":"text"}]}].map((c) => <ChildRecords key={c.resource + c.fk} {...c} parentId={record.id as string | number} />)}
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
