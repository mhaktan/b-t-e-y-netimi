import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type ExchangeRateRecord = {
  id: string | number;
  name: string;
  fromCurrency: string;
  toCurrency: string;
  rate: number;
  effectiveDate: string;
};

const DEFAULTS: Partial<ExchangeRateRecord> = {} as Partial<ExchangeRateRecord>;

interface ExchangeRateCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const ExchangeRateCreate: React.FC<ExchangeRateCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<ExchangeRateRecord>>(DEFAULTS);
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<ExchangeRateRecord>) => dataProvider.create('ExchangeRate', values),
    onSuccess: (_data, values) => { triggerFlows('create', 'ExchangeRate', values as Record<string, unknown>); onSuccess(); onClose(); setForm(DEFAULTS); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>{"Yeni Döviz Kuru"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Kur Adı *" value={String(form.name ?? '')} onChange={(v) => setField('name', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Kaynak Para Birimi *" value={String(form.fromCurrency ?? '')} onChange={(v) => setField('fromCurrency', v ? Number(v) : null)} searchable={false} options={[{ label: "Türk Lirası", value: "0" }, { label: "Dolar", value: "1" }, { label: "Euro", value: "2" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Hedef Para Birimi *" value={String(form.toCurrency ?? '')} onChange={(v) => setField('toCurrency', v ? Number(v) : null)} searchable={false} options={[{ label: "Türk Lirası", value: "0" }, { label: "Dolar", value: "1" }, { label: "Euro", value: "2" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Kur Değeri *" value={String(form.rate ?? '')} onChange={(v) => setField('rate', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Geçerlilik Tarihi *" value={String(form.effectiveDate ?? '')} onChange={(v) => setField('effectiveDate', v)} />
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
