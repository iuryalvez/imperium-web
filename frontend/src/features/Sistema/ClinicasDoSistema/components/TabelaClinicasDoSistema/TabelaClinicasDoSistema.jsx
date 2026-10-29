import React, { useMemo } from "react";
import { Table, Button, Space, Tooltip } from "antd";
import { EyeOutlined, EditOutlined, LoginOutlined } from "@ant-design/icons";
import colunas from './colunas';

const TabelaClinicasDoSistema = ({ clinicas, paginacao, onChange, loading, onEdit, onViewDetails, onAccess }) => {
  const colunaAcoes = useMemo(() => ({
    title: "Ações",
    key: "acoes",
    width: 180,
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
        <Tooltip title="Acessar Clínica (Simular)">
          <Button size="small" icon={<LoginOutlined />} onClick={() => onAccess(record)} type="primary" ghost>
            Acessar
          </Button>
        </Tooltip>
      </Space>
    ),
  }), [onEdit, onViewDetails, onAccess]);

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