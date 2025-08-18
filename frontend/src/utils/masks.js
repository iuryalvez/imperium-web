import React from 'react';
import { IMaskInput } from 'react-imask';

// Estilo base para os inputs, para manter a consistência
const inputStyle = {
  width: '100%',
  padding: '4px 11px',
  border: '1px solid #d9d9d9',
  borderRadius: '6px',
  fontSize: '14px',
};

/**
 * Componente de Input com máscara para CPF.
 * @param {object} props - Props do Ant Design Form.Item (value, onChange)
 */
export const CpfMaskInput = (props) => (
  <IMaskInput
    mask="000.000.000-00"
    unmask={true} // Envia para o form apenas os números
    style={inputStyle}
    placeholder="000.000.000-00"
    value={props.value}
    onAccept={(value) => props.onChange(value)}
  />
);

/**
 * Componente de Input com máscara para CNPJ.
 * @param {object} props - Props do Ant Design Form.Item (value, onChange)
 */
export const CnpjMaskInput = (props) => (
  <IMaskInput
    mask="00.000.000/0000-00"
    unmask={true}
    style={inputStyle}
    placeholder="00.000.000/0000-00"
    value={props.value}
    onAccept={(value) => props.onChange(value)}
  />
);

/**
 * Componente de Input com máscara para Telefone.
 * @param {object} props - Props do Ant Design Form.Item (value, onChange)
 */
export const TelefoneMaskInput = (props) => (
  <IMaskInput
    mask="(00) 00000-0000"
    unmask={true}
    style={inputStyle}
    placeholder="(00) 99999-9999"
    value={props.value}
    onAccept={(value) => props.onChange(value)}
  />
);

/**
 * Componente de Input com máscara para CEP.
 * @param {object} props - Props do Ant Design Form.Item (value, onChange)
 */
export const CepMaskInput = (props) => (
  <IMaskInput
    mask="00000-000"
    unmask={true}
    style={inputStyle}
    placeholder="00000-000"
    value={props.value}
    onAccept={(value) => props.onChange(value)}
  />
);