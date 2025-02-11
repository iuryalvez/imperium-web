import React, { useEffect, useState } from "react";
import { sistema } from "../api/sistemaAPI.js";
import TabelaUsuarios from "../components/organisms/TabelaUsuarios.jsx";

const ListaUsuariosSistema = () => {
  const [usuarios, setUsuarios] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUsuarios = async () => {
      try {
        const response = await sistema.listarUsuarios();
        console.log("Usuários na Page: ", response);
        setUsuarios(response);
      } catch (err) {
        setError("Erro ao carregar os usuários.");
      } finally {
        setLoading(false);
      }
    };

    fetchUsuarios();
  }, []);

  const renderStatus = (message, className = "") => (
    <div className="main-content">
      <div className="info-content">
        <h1 className="text-2xl font-bold mb-4 text-green-dark">
          Lista de Usuários
        </h1>
        <p className={className}>{message}</p>
      </div>
    </div>
  );

  if (loading) return renderStatus("Carregando usuários...");
  if (error) return renderStatus(error, "text-red-500");

  return renderStatus(<TabelaUsuarios usuarios={usuarios} />);
};

export default ListaUsuariosSistema;
