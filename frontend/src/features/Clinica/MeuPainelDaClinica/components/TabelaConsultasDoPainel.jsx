import React, { useMemo } from "react";
import { Button, Table, Tooltip } from "antd";
import { LineChartOutlined } from "@ant-design/icons";
import colunasConsultas from "../../ConsultasDaClinica/components/colunas";

// Colunas reaproveitadas da tela de Consultas (sem a coluna de avaliação, que leva ao acompanhamento)
const CHAVES = ["dataConsulta", "nomeAnimal", "nomeTutor", "status"];

/**
 * Tabela compacta de consultas do Meu Painel, paginada no servidor.
 * Props: lista (retorno de usePaginacaoServidor) e onIrParaAcompanhamento(acompanhamentoId).
 */
const TabelaConsultasDoPainel = ({ lista, onIrParaAcompanhamento }) => {
  const colunas = useMemo(
    () => [
      ...colunasConsultas
        .filter((c) => CHAVES.includes(c.key))
        .map(({ sorter, filters, filterMultiple, ...c }) => c),
      {
        title: "",
        key: "acoes",
        align: "center",
        render: (_, consulta) => (
          <Tooltip title={consulta.acompanhamentoId ? "Ver Acompanhamento" : "Animal sem acompanhamento"}>
            <Button
              size="small"
              type="primary"
              icon={<LineChartOutlined />}
              disabled={!consulta.acompanhamentoId}
              onClick={() => onIrParaAcompanhamento(consulta.acompanhamentoId)}
            />
          </Tooltip>
        ),
      },
    ],
    [onIrParaAcompanhamento],
  );

  return (
    <Table
      size="small"
      rowKey="id"
      dataSource={lista.dados}
      columns={colunas}
      loading={lista.loading}
      pagination={{ ...lista.paginacao, pageSizeOptions: [5, 10, 20] }}
      onChange={lista.handleTableChange}
      scroll={{ x: "max-content" }}
      locale={{ emptyText: "Nenhuma consulta." }}
    />
  );
};

export default TabelaConsultasDoPainel;
