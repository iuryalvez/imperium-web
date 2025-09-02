import React from "react";
import DetalhesClinica from "./Detalhes/FormDetalhesClinica";
import DetalhesTutoresDaClinica from "./Detalhes/FormDetalhesTutoresDaClinica";
import { Button, Typography } from "antd";

// O mapa de componentes continua o mesmo
const componentesDeDetalhes = {
  clinicas: DetalhesClinica,
  tutoresDaClinica: DetalhesTutoresDaClinica,
};

const FormDetalhesEntidade = ({ entidade, valores, onCancel }) => {
  // A lógica para selecionar o componente especialista
  const ComponenteDeDetalhe = componentesDeDetalhes[entidade];

  return (
    // Usamos um <div> como container principal, já que não é um formulário
    <div>
      {ComponenteDeDetalhe ? (
        // Se um componente especialista for encontrado, ele é renderizado
        // o componente especialista É um form, então passamos apenas os valores para ele, diferente
        // da edicao e do cadastro que são form.items
        <ComponenteDeDetalhe valores={valores} />
      ) : (
        // Caso contrário, exibe a mensagem de fallback
        <Typography.Text type="danger">
          Visualização de detalhes para a entidade '{entidade}' não foi
          implementada.
        </Typography.Text>
      )}

      {/* A seção do botão agora faz parte deste componente */}
      <div className="flex justify-end mt-4">
        <Button onClick={onCancel}>Fechar</Button>
      </div>
    </div>
  );
};

export default FormDetalhesEntidade;
