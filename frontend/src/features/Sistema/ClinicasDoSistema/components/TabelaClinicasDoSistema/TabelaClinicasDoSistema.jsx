import React, { useMemo } from "react";
import { Table, Button, Space, Tooltip, Popconfirm } from "antd";
import { EyeOutlined, EditOutlined, LoginOutlined, DeleteOutlined } from "@ant-design/icons";
import colunas from './colunas';

const TabelaClinicasDoSistema = ({ clinicas, paginacao, onChange, loading, loadingRowKey, onEdit, onViewDetails, onAccess, onDelete }) => {
  const colunaAcoes = useMemo(() => ({
    title: "Ações",
    key: "acoes",
    width: 220,
    align: "center",
    fixed: "right",
    render: (_, record) => (
      <Space>
        <Tooltip title="Ver Detalhes">
          <Button size="small" icon={<EyeOutlined />} onClick={() => onViewDetails(record)} />
        </Tooltip>
        <Tooltip title="Editar">
          <Button size="small" icon={<EditOutlined />} onClick={() => onEdit(record)} />
        </Tooltip>
        <Tooltip title="Excluir">
          <Popconfirm
            title="Tem certeza que deseja excluir esta clínica?"
            onConfirm={() => onDelete(record)}
            okText="Sim"
            cancelText="Não"
          >
            <Button
              size="small"
              icon={<DeleteOutlined />}
              danger
              loading={record.id === loadingRowKey}
            />
          </Popconfirm>
        </Tooltip>
        <Tooltip title="Acessar Clínica (Simular)">
          <Button size="small" icon={<LoginOutlined />} onClick={() => onAccess(record)} type="primary" ghost>
            Acessar
          </Button>
        </Tooltip>
      </Space>
    ),
  }), [onEdit, onViewDetails, onAccess, onDelete, loadingRowKey]);

  const memoColunas = useMemo(() => [...colunas, colunaAcoes], [colunaAcoes]);

  return (
    <Table
      dataSource={clinicas}
      columns={memoColunas}
      rowKey="id"
      pagination={paginacao} // paginação no servidor (usePaginacaoServidor)
      onChange={onChange}
      loading={loading}
      scroll={{ x: "max-content" }}
    />
  );
};

export default TabelaClinicasDoSistema;