import React, { useEffect, useState } from "react";
import { usuario } from "../api/sistema/usuariosAPI.js";
import TemplateListaSistemaUsuarios from "../components/template/listagem/TemplateListaSistemaUsuarios.jsx";
import { ArrowLeftOutlined } from "@ant-design/icons";

const ListaSistemaUsuarios = () => {
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

  if (loading) return renderStatus("Carregando usuários...");
  if (error) return renderStatus(error, "text-red-500");

  return renderStatus(<TemplateListaSistemaUsuarios usuarios={usuarios} api={usuario} />);
};

export default ListaSistemaUsuarios;
