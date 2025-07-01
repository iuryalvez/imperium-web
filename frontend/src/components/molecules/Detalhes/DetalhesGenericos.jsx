import React from 'react';

const DetalhesGenericos = ({ valores, campos }) => {
    return (
        <div className="space-y-2">
            {campos.map(campo => (
                <p key={campo.name}>
                    <strong>{campo.label}:</strong> {String(valores[campo.name] ?? 'Não informado')}
                </p>
            ))}
        </div>
    );
};

export default DetalhesGenericos;