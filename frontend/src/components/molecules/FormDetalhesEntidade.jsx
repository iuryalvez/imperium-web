import React from 'react';
import DetalhesAnimal from './Detalhes/DetalhesAnimal';
import DetalhesGenericos from './Detalhes/DetalhesGenericos';
import DetalhesClinica from './Detalhes/DetalhesClinica';
import DetalhesUsuariosDoSistema from './Detalhes/DetalhesUsuariosDoSistema';
import DetalhesUsuariosDaClinica from './Detalhes/DetalhesUsuariosDaClinica';
import DetalhesTutoresDaClinica from './Detalhes/DetalhesTutoresDaClinica';

// O mapa de componentes continua o mesmo
const componentesDeDetalhes = {
    animaisDaClinica: DetalhesAnimal,
    clinicas: DetalhesClinica,
    usuariosDoSistema: DetalhesUsuariosDoSistema,
    usuariosDaClinica: DetalhesUsuariosDaClinica,
    tutoresDaClinica: DetalhesTutoresDaClinica
};

// Este componente agora é apenas um 'bloco de conteúdo'
const FormDetalhesEntidade = ({
    entidade,
    valores,
    camposDetalhes,
}) => {
    // A lógica para selecionar o componente correto permanece
    const Detalhes = componentesDeDetalhes[entidade] || DetalhesGenericos;

    // Retornamos APENAS o JSX do conteúdo, sem Modal, sem Typography.Title
    return (
        <div className="p-4">
            <Detalhes valores={valores} campos={camposDetalhes} />
        </div>
    );
};

export default FormDetalhesEntidade;
