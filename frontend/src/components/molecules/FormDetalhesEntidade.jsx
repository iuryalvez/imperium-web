import React from "react";
import DetalhesAnimaisDaClinica from "./Detalhes/DetalhesAnimaisDaClinica";
import DetalhesClinica from "./Detalhes/DetalhesClinica";
import DetalhesUsuariosDoSistema from "./Detalhes/DetalhesUsuariosDoSistema";
import DetalhesUsuariosDaClinica from "./Detalhes/DetalhesUsuariosDaClinica";
import DetalhesTutoresDaClinica from "./Detalhes/DetalhesTutoresDaClinica";
import { Button, Typography } from "antd";

// O mapa de componentes continua o mesmo
const componentesDeDetalhes = {
  animaisDaClinica: DetalhesAnimaisDaClinica,
  clinicas: DetalhesClinica,
  usuariosDoSistema: DetalhesUsuariosDoSistema,
  usuariosDaClinica: DetalhesUsuariosDaClinica,
  tutoresDaClinica: DetalhesTutoresDaClinica,
};

const FormDetalhesEntidade = ({ entidade, valores, tituloModal, onCancel }) => {
  // A lógica para selecionar o componente especialista
  const ComponenteDeDetalhe = componentesDeDetalhes[entidade];

  return (
    // Usamos um <div> como container principal, já que não é um formulário
    <div className="p-4">
      {ComponenteDeDetalhe ? (
        // Se um componente especialista for encontrado, ele é renderizado
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
