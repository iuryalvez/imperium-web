import React, { useState, useMemo } from "react";
import { Button, Space, Tooltip, Popconfirm, Dropdown, Checkbox } from "antd";
import { EyeOutlined, EditOutlined, DeleteOutlined, SettingOutlined } from "@ant-design/icons";
import TabelaPadrao from "../../../../../components/table/TabelaPadrao";
import colunas from './colunas';

const TabelaTutoresDaClinica = ({ tutores, onEdit, onDelete, onViewDetails, loadingRowKey }) => {
  const [visibleColumns, setVisibleColumns] = useState(["nome", "email", "cpf", "telefone"]);

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
  }), [onEdit, onDelete, onViewDetails, loadingRowKey]);

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
      <TabelaPadrao
        dataSource={tutores}
        columns={colunasVisiveis}
        rowKey="id"
        pagination={{ pageSize: 10, showSizeChanger: true }}
        scroll={{ x: "max-content" }}
      />
    </div>
  );
};

export default TabelaTutoresDaClinica;