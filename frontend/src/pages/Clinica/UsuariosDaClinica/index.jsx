import React, { useEffect, useState } from "react";

import { usuariosDaClinicaAPI } from "../../../api/clinica/usuariosDaClinicaAPI.js";

import Navsidebar from "../../../components/organisms/Navsidebar.jsx";
import { Spin } from "antd";
import TabelaEntidade from "../../../components/organisms/TabelaEntidade.jsx";

const TabelaUsuariosDaClinica = () => {
  const [usuarios, setUsuarios] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchUsuarios = async () => {
      try {
        const response = await usuariosDaClinicaAPI.listar();
        console.log("Usuários encontrados: ", response);
        setUsuarios(response);
      } catch (err) {
        console.error("Erro ao buscar usuários: ", err);
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
        <div className={className}>{message}</div>
      </div>
    </div>
  );

  if (loading) return <Spin size="large" />;
  // if (error) return renderStatus(error, "text-red-500");

  return renderStatus(
    <TabelaEntidade entidade="usuáriosDaClínica" dados={usuarios} />
  );
};

export default TabelaUsuariosDaClinica;
