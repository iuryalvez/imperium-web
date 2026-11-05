import React, { useState, useMemo } from "react";
import {
  Space,
  Tooltip,
  Button,
  Popconfirm,
  Dropdown,
  Checkbox,
} from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  SettingOutlined,
  CalculatorOutlined,
  StopOutlined,
  CheckCircleOutlined,
  EyeOutlined,
  EyeInvisibleOutlined,
} from "@ant-design/icons";
import colunas, { colunaClinica } from "./colunas";
import TabelaPadrao from "../../../../../components/table/TabelaPadrao";

/**
 * Tabela de rações paginada no servidor. Usada pela clínica e pelo admin (Sistema).
 * modo:
 * - "clinica": ração do sistema pode ser ocultada (não excluída/desativada); ração própria pode ser desativada/excluída.
 * - "sistema": catálogo do sistema (admin).
 * - "copias": rações próprias das clínicas vistas pelo admin (mostra a coluna Clínica).
 */
const TabelaRacoesDaClinica = ({
  racoes,
  loading,
  paginacao,
  onChange,
  modo = "clinica",
  onEdit,
  onDelete,
  onView,
  onToggleStatus,
  onToggleOculta,
  loadingRowKey,
  podeEditar = true, // perfis sem "gerenciaRacoes" veem só os cálculos
}) => {
  const colunasDisponiveis = useMemo(
    () => (modo === "copias" ? [colunaClinica, ...colunas] : colunas),
    [modo],
  );
  const [visibleColumns, setVisibleColumns] = useState(
    [colunaClinica, ...colunas].map((c) => c.key),
  );

  const handleColumnVisibilityChange = (key) => {
    setVisibleColumns((prev) =>
      prev.includes(key) ? prev.filter((k) => k !== key) : [...prev, key],
    );
  };

  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      fixed: "right",
      render: (_, record) => {
        const isRowLoading = record.id === loadingRowKey;
        // Na clínica, a ração do sistema só pode ser ocultada; o resto vale para rações próprias e para o admin
        const ehDoSistemaNaClinica = modo === "clinica" && record.isSistema;
        const podeOcultar = podeEditar && ehDoSistemaNaClinica;
        const podeAlterarStatusEExcluir = podeEditar && !ehDoSistemaNaClinica;

        return (
          <Space>
            <Tooltip title="Ver Cálculos">
              <Button
                icon={<CalculatorOutlined />}
                size="small"
                onClick={() => onView(record)}
              />
            </Tooltip>
            {podeEditar && (
              <Tooltip
                title={
                  ehDoSistemaNaClinica
                    ? "Editar (cria uma cópia desta ração para a clínica)"
                    : "Editar"
                }
              >
                <Button icon={<EditOutlined />} size="small" onClick={() => onEdit(record)} />
              </Tooltip>
            )}
            {podeOcultar && (
              <Tooltip title={record.isOculta ? "Mostrar para esta clínica" : "Ocultar para esta clínica"}>
                <Popconfirm
                  title={record.isOculta ? "Voltar a mostrar esta ração?" : "Ocultar esta ração para a clínica?"}
                  onConfirm={() => onToggleOculta(record)}
                  okText="Sim"
                  cancelText="Não"
                >
                  <Button
                    size="small"
                    loading={isRowLoading}
                    icon={record.isOculta ? <EyeOutlined /> : <EyeInvisibleOutlined />}
                  />
                </Popconfirm>
              </Tooltip>
            )}
            {podeAlterarStatusEExcluir && (
              <>
                <Tooltip title={record.isAtivo ? "Desativar" : "Ativar"}>
                  <Popconfirm
                    title={`Tem certeza que deseja ${record.isAtivo ? "desativar" : "ativar"} esta ração?`}
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
                    title="Tem certeza que deseja excluir esta ração?"
                    onConfirm={() => onDelete(record)}
                    okText="Sim"
                    cancelText="Não"
                  >
                    <Button icon={<DeleteOutlined />} size="small" danger />
                  </Popconfirm>
                </Tooltip>
              </>
            )}
          </Space>
        );
      },
    }),
    [modo, onEdit, onDelete, onView, onToggleStatus, onToggleOculta, loadingRowKey, podeEditar],
  );

  const colunasVisiveis = useMemo(
    () => [
      ...colunasDisponiveis.filter((col) => visibleColumns.includes(col.key)),
      colunaAcoes,
    ],
    [colunasDisponiveis, visibleColumns, colunaAcoes],
  );

  const menuColunas = (
    <div
      style={{
        padding: 8,
        backgroundColor: "white",
        border: "1px solid #d9d9d9",
        borderRadius: 4,
      }}
    >
      {colunasDisponiveis.map((col) => (
        <div key={col.key}>
          <Checkbox
            checked={visibleColumns.includes(col.key)}
            onChange={() => handleColumnVisibilityChange(col.key)}
          >
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
        dataSource={racoes}
        columns={colunasVisiveis}
        rowKey="id"
        pagination={paginacao}
        onChange={onChange}
        scroll={{ x: "max-content" }}
        loading={loading}
      />
    </div>
  );
};

export default TabelaRacoesDaClinica;
