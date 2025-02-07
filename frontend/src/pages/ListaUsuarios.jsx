import React, { useEffect, useState } from "react";
import { usuario } from "../api/usuariosAPI.js";
import Navsidebar from "../components/organisms/Navsidebar.jsx";
import TabelaUsuarios from "../components/organisms/TabelaUsuarios.jsx";

const ListaUsuarios = () => {
  const [usuarios, setUsuarios] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUsuarios = async () => {
      try {
        const response = await usuario.listar();
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
      <Navsidebar />
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

export default ListaUsuarios;
