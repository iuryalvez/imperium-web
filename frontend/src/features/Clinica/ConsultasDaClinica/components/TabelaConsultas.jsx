import React, { useState, useMemo, useRef } from "react";
import {
  Button,
  Space,
  Popconfirm,
  Tooltip,
  Dropdown,
  Checkbox,
  Input,
} from "antd";
import {
  LineChartOutlined,
  EditOutlined,
  DeleteOutlined,
  SettingOutlined,
  StopOutlined,
  UndoOutlined,
} from "@ant-design/icons";
import TabelaPadrao from "../../../../components/table/TabelaPadrao";
import colunas from "./colunas";

const TabelaConsultas = ({
  consultas,
  paginacao,
  onChange,
  onEdit,
  onDelete,
  onIrParaAcompanhamento,
  onCancelar,
  onReabrir,
  loading,
}) => {
  const [visibleColumns, setVisibleColumns] = useState([
    "dataConsulta",
    "nomeAnimal",
    "nomeTutor",
    "nomeUsuario",
    "avaliacaoPreenchida",
    "status",
  ]);

  const handleColumnVisibilityChange = (key) => {
    setVisibleColumns((prev) =>
      prev.includes(key) ? prev.filter((k) => k !== key) : [...prev, key],
    );
  };

  // Motivo digitado no Popconfirm de cancelamento (opcional)
  const motivoRef = useRef("");

  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      width: 180,
      align: "center",
      fixed: "right",
      render: (_, consulta) => {
        const cancelada = consulta.status === "Cancelada";
        const pendente = consulta.status === "Pendente";
        return (
          <Space size="small">
            <Tooltip title={cancelada ? "Reabra a consulta para editar" : "Editar Agendamento"}>
              <Button
                size="small"
                icon={<EditOutlined />}
                onClick={() => onEdit(consulta)}
                disabled={cancelada}
              />
            </Tooltip>

            {/* Só consultas pendentes (sem avaliação) podem ser canceladas; canceladas podem ser reabertas */}
            {pendente && (
              <Tooltip title="Cancelar Consulta">
                <Popconfirm
                  title="Cancelar consulta"
                  description={
                    <Input.TextArea
                      placeholder="Motivo (opcional)"
                      maxLength={500}
                      autoSize={{ minRows: 2, maxRows: 4 }}
                      onChange={(e) => (motivoRef.current = e.target.value)}
                    />
                  }
                  onOpenChange={(aberto) => aberto && (motivoRef.current = "")}
                  onConfirm={() => onCancelar(consulta, motivoRef.current)}
                  okText="Cancelar consulta"
                  cancelText="Voltar"
                >
                  <Button size="small" danger icon={<StopOutlined />} />
                </Popconfirm>
              </Tooltip>
            )}
            {cancelada && (
              <Tooltip title="Reabrir Consulta">
                <Popconfirm
                  title="Reabrir consulta"
                  description="A consulta volta a ficar pendente."
                  onConfirm={() => onReabrir(consulta)}
                  okText="Reabrir"
                  cancelText="Voltar"
                >
                  <Button size="small" icon={<UndoOutlined />} />
                </Popconfirm>
              </Tooltip>
            )}

            <Tooltip title="Excluir Consulta">
              <Popconfirm
                title="Excluir consulta"
                description="Tem certeza que deseja excluir esta consulta?"
                onConfirm={() => onDelete(consulta.id)}
                okText="Sim"
                cancelText="Não"
              >
                <Button size="small" danger icon={<DeleteOutlined />} />
              </Popconfirm>
            </Tooltip>

            <Tooltip title={consulta.acompanhamentoId ? "Ver Acompanhamento" : "Animal sem acompanhamento"}>
              <Button
                type="primary"
                size="small"
                icon={<LineChartOutlined />}
                onClick={() => onIrParaAcompanhamento(consulta.acompanhamentoId)}
                disabled={!consulta.acompanhamentoId}
                className="bg-verde-medio border-none hover:bg-verde-realce text-white"
              />
            </Tooltip>
          </Space>
        );
      },
    }),
    [onEdit, onDelete, onIrParaAcompanhamento, onCancelar, onReabrir],
  );

  const colunasVisiveis = useMemo(
    () => [
      ...colunas.filter((col) => visibleColumns.includes(col.key)),
      colunaAcoes,
    ],
    [visibleColumns, colunaAcoes],
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
      {colunas.map((col) => (
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
        dataSource={consultas}
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

export default TabelaConsultas;
