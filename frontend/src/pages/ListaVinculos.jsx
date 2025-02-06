import React, { useEffect, useState } from 'react';
import TemplateListaVinculos from '../components/template/TemplateListaVinculos';
import { autenticacao } from '../api/autenticacaoAPI';

const ListaVinculos = () => {
    const [vinculos, setVinculos] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchVinculos = async () => {
            try {
                const response = await autenticacao.listarVinculos();
                console.log("Vinculos na Page: ", response);
                setVinculos(response);
            } catch (err) {
                setError('Erro ao carregar os vínculos.');
            } finally {
                setLoading(false);
            }
        };

        fetchVinculos();
    }, []);

    const renderStatus = (message, className = "") => (
        <div className="container mx-auto my-8">
            <h1 className="text-2xl font-bold mb-4 text-green-dark">Lista de Vínculos</h1>
            <p className={className}>{message}</p>
        </div>
    );
    
    if (loading) return renderStatus("Carregando vínculos...");
    if (error) return renderStatus(error, "text-red-500");

    return (
        renderStatus(<TemplateListaVinculos vinculos={vinculos} />)
    );
};

export default ListaVinculos;
