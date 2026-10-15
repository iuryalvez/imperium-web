import React from "react";
import { Table } from "antd";

/**
 * TabelaPadrao
 * Componente que padroniza o visual de todas as tabelas do sistema (Cores, Bordas, Gradientes).
 * Repassa todos os props recebidos diretamente para o componente Table do Ant Design.
 */
const TabelaPadrao = (props) => {
  return (
    <Table
      className="tabela-padrao"
      bordered={true}
      rowClassName={(record, index) => 
        `tabela-padrao-row ${index % 2 === 0 ? "linha-par" : "linha-impar"}`
      }
      {...props}
    />
  );
};

export default TabelaPadrao;
