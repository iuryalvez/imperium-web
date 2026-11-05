import React, { useMemo } from "react";
import { Grid } from "antd";
import {
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
  LineChartOutlined,
  StopOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import TabelaPadrao from "../../../../../components/table/TabelaPadrao";
import AcoesLinha from "../../../../../components/table/AcoesLinha";
import colunas from "./colunas";

// Colunas que aparecem ao abrir a tela; as outras (sexo, castrado, tutores) ficam no botão "Colunas".
const COLUNAS_INICIAIS = ["nome", "especie", "racaNome", "dataCadastro", "isAtivo"];

const TabelaAnimais = ({
  data,
  paginacao,
  onChange,
  loading,
  onEdit,
  onDelete,
  onToggleStatus,
  onViewDetails,
  onNavigateToAcompanhamento,
  loadingRowKey,
  barra, // busca da página, mostrada na mesma linha do botão "Colunas"
  podeEditar = true, // perfis sem "gerenciaAnimais" veem a lista sem os botões de edição
}) => {
  // No celular as ações viram um menu "⋮", para a coluna não ocupar metade da tela.
  const telas = Grid.useBreakpoint();
  const compacta = telas.md === false;

  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      width: compacta ? 64 : 170,
      align: "center",
      fixed: "right",
      render: (_, record) => {
        const carregandoLinha = record.id === loadingRowKey;
        return (
          <AcoesLinha
            compacta={compacta}
            acoes={[
              {
                chave: "detalhes",
                rotulo: "Ver detalhes",
                icone: <EyeOutlined />,
                onClick: () => onViewDetails(record),
                desabilitado: carregandoLinha,
              },
              podeEditar && {
                chave: "editar",
                rotulo: "Editar",
                icone: <EditOutlined />,
                onClick: () => onEdit(record),
                desabilitado: carregandoLinha,
              },
              {
                chave: "acompanhamento",
                rotulo: "Ver acompanhamento",
                icone: <LineChartOutlined />,
                tipo: "primary",
                onClick: () => onNavigateToAcompanhamento(record.acompanhamentoId),
                desabilitado: carregandoLinha || record.acompanhamentoId == null,
              },
              podeEditar && {
                chave: "status",
                rotulo: record.isAtivo ? "Desativar" : "Ativar",
                icone: record.isAtivo ? <StopOutlined /> : <CheckCircleOutlined />,
                perigo: record.isAtivo,
                carregando: carregandoLinha,
                confirmar: `Tem certeza que deseja ${record.isAtivo ? "desativar" : "ativar"} este animal?`,
                onClick: () => onToggleStatus(record),
              },
              podeEditar && {
                chave: "excluir",
                rotulo: "Excluir",
                icone: <DeleteOutlined />,
                perigo: true,
                desabilitado: carregandoLinha,
                confirmar: "Tem certeza que deseja excluir?",
                onClick: () => onDelete(record.id),
              },
            ]}
          />
        );
      },
    }),
    [onEdit, onDelete, onToggleStatus, onNavigateToAcompanhamento, onViewDetails, loadingRowKey, podeEditar, compacta]
  );

  const colunasComAcoes = useMemo(() => [...colunas, colunaAcoes], [colunaAcoes]);

  return (
    <TabelaPadrao
      barra={barra}
      selecionarColunas
      colunasIniciais={COLUNAS_INICIAIS}
      dataSource={data}
      columns={colunasComAcoes}
      rowKey="id"
      pagination={paginacao} // paginação no servidor (usePaginacaoServidor)
      onChange={onChange}
      loading={loading}
      scroll={{ x: "max-content" }}
    />
  );
};

export default TabelaAnimais;
