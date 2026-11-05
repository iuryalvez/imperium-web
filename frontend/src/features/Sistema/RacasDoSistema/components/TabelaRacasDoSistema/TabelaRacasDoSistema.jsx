import React, { useMemo } from "react";
import { Grid } from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  StopOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import colunas, { colunaClinica } from "./colunas";
import TabelaPadrao from "../../../../../components/table/TabelaPadrao";
import AcoesLinha from "../../../../../components/table/AcoesLinha";

/**
 * Tabela de raças do admin, paginada no servidor.
 * visao "copias" mostra a coluna Clínica (raças próprias das clínicas).
 * barra: busca da página, acima da tabela.
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
  barra,
}) => {
  // No celular as ações viram um menu "⋮"
  const telas = Grid.useBreakpoint();
  const compacta = telas.md === false;

  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      fixed: "right",
      align: "center",
      width: compacta ? 64 : 120,
      render: (_, record) => {
        const carregandoLinha = record.id === loadingRowKey;
        return (
          <AcoesLinha
            compacta={compacta}
            acoes={[
              { chave: "editar", rotulo: "Editar", icone: <EditOutlined />, onClick: () => onEdit(record) },
              {
                chave: "status",
                rotulo: record.isAtivo ? "Desativar" : "Ativar",
                icone: record.isAtivo ? <StopOutlined /> : <CheckCircleOutlined />,
                perigo: record.isAtivo,
                carregando: carregandoLinha,
                confirmar: `Tem certeza que deseja ${record.isAtivo ? "desativar" : "ativar"} esta raça?`,
                onClick: () => onToggleStatus(record),
              },
              {
                chave: "excluir",
                rotulo: "Excluir",
                icone: <DeleteOutlined />,
                perigo: true,
                confirmar: "Tem certeza que deseja excluir esta raça?",
                onClick: () => onDelete(record),
              },
            ]}
          />
        );
      },
    }),
    [onEdit, onDelete, onToggleStatus, loadingRowKey, compacta],
  );

  const colunasVisiveis = useMemo(
    () => [...(visao === "copias" ? [colunaClinica] : []), ...colunas, colunaAcoes],
    [visao, colunaAcoes],
  );

  return (
    <TabelaPadrao
      barra={barra}
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
