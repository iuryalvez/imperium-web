import React, { useEffect, useState } from "react";

import { usuariosDoSistemaAPI } from "../../../api/sistema/usuariosDoSistemaAPI.js";

import { ArrowLeftOutlined } from "@ant-design/icons";
import { Spin } from "antd";
import TabelaEntidade from "../../../components/organisms/TabelaEntidade.jsx";

const UsuariosDoSistema = () => {
  const [usuariosDoSistema, setUsuariosDoSistema] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchUsuarios = async () => {
      try {
        const response = await usuariosDoSistemaAPI.listar();
        console.log("Usuários encontrados: ", response);
        setUsuariosDoSistema(response);
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
      <div className="info-content">
        <button
          onClick={() => window.history.back()}
          className="flex items-center font-bold text-green-dark mb-2"
        >
          <ArrowLeftOutlined className="mr-2" /> Voltar
        </button>
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
    <TabelaEntidade entidade="usuariosDoSistema" dados={usuariosDoSistema} />
  );
};

export default UsuariosDoSistema;
