import React, { useMemo, useRef } from "react";
import { Grid, Input } from "antd";
import {
  LineChartOutlined,
  EditOutlined,
  DeleteOutlined,
  StopOutlined,
  UndoOutlined,
} from "@ant-design/icons";
import TabelaPadrao from "../../../../components/table/TabelaPadrao";
import AcoesLinha from "../../../../components/table/AcoesLinha";
import colunas from "./colunas";

// Colunas que aparecem ao abrir a tela; as outras ficam no botão "Colunas".
const COLUNAS_INICIAIS = [
  "dataConsulta",
  "nomeAnimal",
  "nomeTutor",
  "nomeUsuario",
  "avaliacaoPreenchida",
  "status",
];

const COLUNAS_CELULAR = ["dataConsulta", "nomeAnimal"];

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
  barra, // busca da página, na mesma linha do botão "Colunas"
}) => {
  // No celular as ações viram um menu "⋮"
  const telas = Grid.useBreakpoint();
  const compacta = telas.md === false;

  // Motivo digitado na confirmação de cancelamento (opcional)
  const motivoRef = useRef("");

  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      width: compacta ? 64 : 150,
      align: "center",
      fixed: "right",
      render: (_, consulta) => {
        const cancelada = consulta.status === "Cancelada";
        const pendente = consulta.status === "Pendente";
        return (
          <AcoesLinha
            compacta={compacta}
            acoes={[
              {
                chave: "editar",
                rotulo: cancelada ? "Reabra a consulta para editar" : "Editar agendamento",
                icone: <EditOutlined />,
                desabilitado: cancelada,
                onClick: () => onEdit(consulta),
              },
              // Só consultas pendentes (sem avaliação) podem ser canceladas; canceladas podem ser reabertas
              pendente && {
                chave: "cancelar",
                rotulo: "Cancelar consulta",
                icone: <StopOutlined />,
                perigo: true,
                confirmar: "Cancelar consulta",
                confirmarDescricao: (
                  <Input.TextArea
                    placeholder="Motivo (opcional)"
                    maxLength={500}
                    autoSize={{ minRows: 2, maxRows: 4 }}
                    onChange={(e) => (motivoRef.current = e.target.value)}
                  />
                ),
                aoAbrirConfirmacao: () => (motivoRef.current = ""),
                confirmarOk: "Cancelar consulta",
                confirmarCancelar: "Voltar",
                onClick: () => onCancelar(consulta, motivoRef.current),
              },
              cancelada && {
                chave: "reabrir",
                rotulo: "Reabrir consulta",
                icone: <UndoOutlined />,
                confirmar: "Reabrir consulta",
                confirmarDescricao: "A consulta volta a ficar pendente.",
                confirmarOk: "Reabrir",
                confirmarCancelar: "Voltar",
                onClick: () => onReabrir(consulta),
              },
              {
                chave: "excluir",
                rotulo: "Excluir consulta",
                icone: <DeleteOutlined />,
                perigo: true,
                confirmar: "Excluir consulta",
                confirmarDescricao: "Tem certeza que deseja excluir esta consulta?",
                onClick: () => onDelete(consulta.id),
              },
              {
                chave: "acompanhamento",
                rotulo: consulta.acompanhamentoId ? "Ver acompanhamento" : "Animal sem acompanhamento",
                icone: <LineChartOutlined />,
                tipo: "primary",
                desabilitado: !consulta.acompanhamentoId,
                onClick: () => onIrParaAcompanhamento(consulta.acompanhamentoId),
              },
            ]}
          />
        );
      },
    }),
    [onEdit, onDelete, onIrParaAcompanhamento, onCancelar, onReabrir, compacta],
  );

  const colunasComAcoes = useMemo(() => [...colunas, colunaAcoes], [colunaAcoes]);

  return (
    <TabelaPadrao
      barra={barra}
      selecionarColunas
      colunasIniciais={COLUNAS_INICIAIS}
      colunasCelular={COLUNAS_CELULAR}
      dataSource={consultas}
      columns={colunasComAcoes}
      rowKey="id"
      pagination={paginacao} // paginação no servidor (usePaginacaoServidor)
      onChange={onChange}
      loading={loading}
      scroll={{ x: "max-content" }}
    />
  );
};

export default TabelaConsultas;
