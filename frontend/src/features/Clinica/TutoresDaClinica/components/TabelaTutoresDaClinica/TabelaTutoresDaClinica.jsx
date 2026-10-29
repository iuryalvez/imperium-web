import React, { useState, useMemo } from "react";
import { Table, Button, Space, Tooltip, Popconfirm, Dropdown, Checkbox } from "antd";
import { EyeOutlined, EditOutlined, DeleteOutlined, SettingOutlined, StopOutlined, CheckCircleOutlined } from "@ant-design/icons";
import colunas from './colunas';

const TabelaTutoresDaClinica = ({ tutores, paginacao, onChange, loading, onEdit, onDelete, onToggleStatus, onViewDetails, loadingRowKey }) => {
  const [visibleColumns, setVisibleColumns] = useState(["nome", "email", "cpf", "telefone", "isAtivo"]);

  const handleColumnVisibilityChange = (key) => {
    setVisibleColumns((prev) =>
      prev.includes(key) ? prev.filter((k) => k !== key) : [...prev, key]
    );
  };

  const colunaAcoes = useMemo(() => ({
    title: "Ações",
    key: "acoes",
    width: 150,
    align: "center",
    fixed: "right",
    render: (_, record) => {
      const isCurrentRowLoading = record.id === loadingRowKey;
      return (
        <Space>
          <Tooltip title="Ver Detalhes">
            <Button size="small" icon={<EyeOutlined />} onClick={() => onViewDetails(record)} disabled={isCurrentRowLoading} />
          </Tooltip>
          <Tooltip title="Editar">
            <Button size="small" icon={<EditOutlined />} onClick={() => onEdit(record)} disabled={isCurrentRowLoading} />
          </Tooltip>
          <Tooltip title={record.isAtivo ? "Desativar" : "Ativar"}>
            <Popconfirm
              title={`Tem certeza que deseja ${record.isAtivo ? "desativar" : "ativar"} este tutor?`}
              onConfirm={() => onToggleStatus(record)}
              okText="Sim"
              cancelText="Não"
              disabled={isCurrentRowLoading}
            >
              <Button
                size="small"
                icon={record.isAtivo ? <StopOutlined /> : <CheckCircleOutlined />}
                danger={record.isAtivo}
                loading={isCurrentRowLoading}
              />
            </Popconfirm>
          </Tooltip>
          <Tooltip title="Remover">
            <Popconfirm
              title="Tem certeza que deseja remover este tutor?"
              onConfirm={() => onDelete(record)}
              okText="Sim"
              cancelText="Não"
              disabled={isCurrentRowLoading}
            >
              <Button size="small" icon={<DeleteOutlined />} danger loading={isCurrentRowLoading} />
            </Popconfirm>
          </Tooltip>
        </Space>
      );
    },
  }), [onEdit, onDelete, onToggleStatus, onViewDetails, loadingRowKey]);

  const colunasVisiveis = useMemo(() => [
    ...colunas.filter((col) => visibleColumns.includes(col.key)),
    colunaAcoes,
  ], [visibleColumns, colunaAcoes]);

  const menuColunas = (
    <div style={{ padding: 8, backgroundColor: "white", border: "1px solid #d9d9d9", borderRadius: 4 }}>
      {colunas.map((col) => (
        <div key={col.key}>
          <Checkbox checked={visibleColumns.includes(col.key)} onChange={() => handleColumnVisibilityChange(col.key)}>
            {col.title}
          </Checkbox>
        </div>
      ))}
    </div>
  );

  return (
    <div>
      <div style={{ marginBottom: 16, textAlign: "right" }}>
        <Dropdown dropdownRender={() => menuColunas} trigger={["click"]}>
          <Button icon={<SettingOutlined />}>Colunas</Button>
        </Dropdown>
      </div>
      <Table
        dataSource={tutores}
        columns={colunasVisiveis}
        rowKey="id"
        pagination={paginacao} // paginação no servidor (usePaginacaoServidor)
        onChange={onChange}
        loading={loading}
        scroll={{ x: "max-content" }}
      />
    </div>
  );
};

export default TabelaTutoresDaClinica;