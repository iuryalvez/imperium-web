import React, { useMemo } from "react";
import { Table, Space, Tooltip, Button, Popconfirm } from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  StopOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import colunas, { colunaClinica } from "./colunas";

/**
 * Tabela de raças do admin, paginada no servidor.
 * visao "copias" mostra a coluna Clínica (raças próprias das clínicas).
 */
const TabelaRacasDoSistema = ({
  racas,
  loading,
  paginacao,
  onChange,
  visao,
  onEdit,
  onDelete,
  onToggleStatus,
  loadingRowKey,
}) => {
  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      fixed: "right",
      align: "center",
      render: (_, record) => {
        const isRowLoading = record.id === loadingRowKey;
        return (
          <Space>
            <Tooltip title="Editar">
              <Button size="small" icon={<EditOutlined />} onClick={() => onEdit(record)} />
            </Tooltip>
            <Tooltip title={record.isAtivo ? "Desativar" : "Ativar"}>
              <Popconfirm
                title={`Tem certeza que deseja ${record.isAtivo ? "desativar" : "ativar"} esta raça?`}
                onConfirm={() => onToggleStatus(record)}
                okText="Sim"
                cancelText="Não"
              >
                <Button
                  size="small"
                  loading={isRowLoading}
                  danger={record.isAtivo}
                  icon={record.isAtivo ? <StopOutlined /> : <CheckCircleOutlined />}
                />
              </Popconfirm>
            </Tooltip>
            <Tooltip title="Excluir">
              <Popconfirm
                title="Tem certeza que deseja excluir esta raça?"
                onConfirm={() => onDelete(record)}
                okText="Sim"
                cancelText="Não"
              >
                <Button size="small" icon={<DeleteOutlined />} danger />
              </Popconfirm>
            </Tooltip>
          </Space>
        );
      },
    }),
    [onEdit, onDelete, onToggleStatus, loadingRowKey],
  );

  const colunasVisiveis = useMemo(
    () => [...(visao === "copias" ? [colunaClinica] : []), ...colunas, colunaAcoes],
    [visao, colunaAcoes],
  );

  return (
    <Table
      dataSource={racas}
      columns={colunasVisiveis}
      rowKey="id"
      loading={loading}
      pagination={paginacao}
      onChange={onChange}
      scroll={{ x: "max-content" }}
    />
  );
};

export default TabelaRacasDoSistema;
