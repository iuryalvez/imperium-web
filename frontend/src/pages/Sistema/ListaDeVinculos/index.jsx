import React, { useEffect, useState } from "react";

import { autenticacaoAPI } from "../../../api/autenticacaoAPI";

import { ArrowLeftOutlined } from "@ant-design/icons";
import ListaCards from "../../../components/organisms/ListaCards";
import LoadingSpinner from "../../../components/atoms/LoadingSpinner";

const ListaDeVinculos = () => {
  const [vinculos, setVinculos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchVinculos = async () => {
      try {
        const response = await autenticacaoAPI.listarVinculos();
        console.log("Vinculos na Page: ", response);
        setVinculos(response);
      } catch (err) {
        setError("Erro ao carregar os vínculos.");
      } finally {
        setLoading(false);
      }
    };

    fetchVinculos();
  }, []);

  const renderStatus = (message, className = "") => (
    <div className="container mx-auto my-8">
      <button
        onClick={() => window.history.back()}
        className="flex items-center font-bold text-green-dark mb-2"
      >
        <ArrowLeftOutlined className="mr-2" /> Voltar
      </button>
      <h1 className="text-2xl font-bold mb-4 text-green-dark">
        Lista de Vínculos
      </h1>
      <div className={className}>{message}</div>
    </div>
  );

  if (loading) return <LoadingSpinner />;
  if (error) return renderStatus(error, "text-red-500");

  return renderStatus(<ListaCards tipoEntidade="Vínculo" dados={vinculos} />);
};

export default ListaDeVinculos;
