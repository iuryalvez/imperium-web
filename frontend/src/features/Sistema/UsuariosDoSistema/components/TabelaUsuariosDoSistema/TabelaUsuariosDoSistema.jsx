import React, { useMemo } from "react";
import { Grid } from "antd";
import {
  EyeOutlined,
  EditOutlined,
  StopOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import TabelaPadrao from "../../../../../components/table/TabelaPadrao";
import AcoesLinha from "../../../../../components/table/AcoesLinha";
import colunas from "./colunas";

// Colunas que aparecem ao abrir a tela; as outras ficam no botão "Colunas".
const COLUNAS_INICIAIS = ["nome", "perfis", "nomeClinica", "dataCadastro"];
const COLUNAS_CELULAR = ["nome"];

// Lista ainda paginada no cliente: GET sistema/usuarios não segue o contrato de paginação (pendência conhecida).
const TabelaUsuariosDoSistema = ({
  data,
  onEdit,
  onToggleStatus,
  onViewDetails,
  loadingRowKey,
}) => {
  // No celular as ações viram um menu "⋮"
  const telas = Grid.useBreakpoint();
  const compacta = telas.md === false;

  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      width: compacta ? 64 : 120,
      align: "center",
      fixed: "right",
      render: (_, record) => {
        const carregandoLinha = (record.compositeKey || record.id) === loadingRowKey;
        return (
          <AcoesLinha
            compacta={compacta}
            acoes={[
              { chave: "detalhes", rotulo: "Ver detalhes", icone: <EyeOutlined />, onClick: () => onViewDetails(record), desabilitado: carregandoLinha },
              { chave: "editar", rotulo: "Editar", icone: <EditOutlined />, onClick: () => onEdit(record), desabilitado: carregandoLinha },
              {
                chave: "status",
                rotulo: record.isAtivo ? "Inativar" : "Ativar",
                icone: record.isAtivo ? <StopOutlined /> : <CheckCircleOutlined />,
                perigo: record.isAtivo,
                carregando: carregandoLinha,
                confirmar: `Tem certeza que deseja ${record.isAtivo ? "inativar" : "ativar"} este usuário?`,
                onClick: () => onToggleStatus(record),
              },
            ]}
          />
        );
      },
    }),
    [onEdit, onToggleStatus, onViewDetails, loadingRowKey, compacta]
  );

  const colunasComAcoes = useMemo(() => [...colunas, colunaAcoes], [colunaAcoes]);

  return (
    <TabelaPadrao
      selecionarColunas
      colunasIniciais={COLUNAS_INICIAIS}
      colunasCelular={COLUNAS_CELULAR}
      dataSource={data}
      columns={colunasComAcoes}
      rowKey={(record) => record.compositeKey || record.id}
      pagination={{ pageSize: 10, showSizeChanger: true }}
      scroll={{ x: "max-content" }}
    />
  );
};

export default TabelaUsuariosDoSistema;
