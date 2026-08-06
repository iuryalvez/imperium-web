import React, { useState } from "react";
import { Button, Input, Space, DatePicker } from "antd";
import { SearchOutlined, ClearOutlined } from "@ant-design/icons";

const { RangePicker } = DatePicker;

/**
 * Componente genérico para filtros de busca nas tabelas do sistema.
 * @param {Function} onFiltrar Função chamada ao clicar em buscar ou limpar, recebe um objeto com os valores dos filtros.
 * @param {Array} campos Lista de campos de filtro. Ex: [{ name: 'searchNome', placeholder: 'Nome', type: 'text' }]
 */
const FiltrosBusca = ({ onFiltrar, campos = [] }) => {
  const [valores, setValores] = useState({});

  const handleChange = (name, val) => {
    setValores((prev) => ({ ...prev, [name]: val }));
  };

  const handleClear = () => {
    setValores({});
    onFiltrar({});
  };

  const handleApply = () => {
    onFiltrar(valores);
  };

  return (
    <div
      style={{
        marginBottom: 24,
        marginTop: 24,
        padding: 16,
        background: "#f9f9f9",
        border: "1px solid #e8e8e8",
        borderRadius: 8,
      }}
    >
      <Space wrap size="middle" style={{ width: "100%" }}>
        {campos.map((campo) => {
          if (campo.type === "text") {
            return (
              <Input
                key={campo.name}
                placeholder={campo.placeholder}
                value={valores[campo.name] || ""}
                onChange={(e) => handleChange(campo.name, e.target.value)}
                onPressEnter={handleApply}
                style={{ width: campo.width || 250 }}
                allowClear
              />
            );
          }
          if (campo.type === "dateRange") {
            return (
              <RangePicker
                key={campo.name}
                value={valores[campo.name] || null}
                onChange={(val) => handleChange(campo.name, val)}
                format="DD/MM/YYYY"
                style={{ width: campo.width || 280 }}
              />
            );
          }
          return null;
        })}
        <Button type="primary" icon={<SearchOutlined />} onClick={handleApply}>
          Filtrar
        </Button>
        <Button icon={<ClearOutlined />} onClick={handleClear}>
          Limpar
        </Button>
      </Space>
    </div>
  );
};

export default FiltrosBusca;
