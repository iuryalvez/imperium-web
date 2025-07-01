// src/components/detalhes/DetalhesAnimal.jsx
import React from 'react';

// Função auxiliar para formatar valores (pode ficar em um arquivo utils)
const formatBoolean = (value) => (value ? 'Sim' : 'Não');
const formatEspecie = (value) => (value === 1 ? 'Cão' : 'Gato');
const formatSexo = (value) => (value === 1 ? 'Macho' : 'Fêmea');

const DetalhesAnimal = ({ valores }) => {
    const isFemea = valores.sexo === 2;

    return (
        <div className="space-y-2">
            <p><strong>Nome:</strong> {valores.nome}</p>
            <p><strong>Espécie:</strong> {formatEspecie(valores.especie)}</p>
            <p><strong>Raça:</strong> {valores.raca?.nome || 'Não informada'}</p>
            <p><strong>Sexo:</strong> {formatSexo(valores.sexo)}</p>
            <p><strong>Castrado:</strong> {formatBoolean(valores.castrado)}</p>
            {isFemea && (
                <>
                    <p><strong>Gestação:</strong> {formatBoolean(valores.gestacao)}</p>
                    {valores.gestacao && <p><strong>Semanas de Gestação:</strong> {valores.semanaGestacao}</p>}
                    <p><strong>Lactação (Filhotes):</strong> {valores.lactacao > 0 ? valores.lactacao : 'Não'}</p>
                    {valores.lactacao > 0 && <p><strong>Semanas de Lactação:</strong> {valores.semanaLactacao}</p>}
                </>
            )}
        </div>
    );
};

export default DetalhesAnimal;