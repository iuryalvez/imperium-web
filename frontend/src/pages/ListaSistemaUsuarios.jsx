import React, { useEffect, useState } from "react";
import { usuariosDoSistemaAPI } from "../api/sistema/usuariosDoSistemaAPI.js";
import TemplateListaSistemaUsuarios from "../components/template/listagem/TemplateListaSistemaUsuarios.jsx";
import { ArrowLeftOutlined } from "@ant-design/icons";
import { message } from "antd";

const ListaSistemaUsuarios = () => {
  const [usuarios, setUsuarios] = useState([]);
  const [camposCadastro, setCamposCadastro] = useState([]);
  const [loading, setLoading] = useState(true);
  // const [error, setError] = useState(null);

  useEffect(() => {
    const fetchCamposCadastro = async () => {
      try {
        const url = usuariosDoSistemaAPI.listarCamposCadastroSistema("usuarios");
        console.log("Buscando campos em:", url);
        const response = await fetch(url);
        if (!response.ok) throw new Error("Erro ao buscar campos.");
        const data = await usuariosDoSistemaAPI.listarCamposCadastroSistema("usuarios");
        setCamposCadastro(data);
      } catch (error) {
        console.error("Erro ao buscar campos do usuário:", error);
      }
    };
    
    const fetchUsuarios = async () => {
      try {
        const response = await usuariosDoSistemaAPI.listar();
        console.log("Usuários encontrados: ", response);
        setUsuarios(response);
      } catch (err) {
        console.error("Erro ao buscar usuários: ", err);
      } finally {
        setLoading(false);
      }
    };

    fetchCamposCadastro();
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
  // if (error) return renderStatus(error, "text-red-500");

  return renderStatus(<TemplateListaSistemaUsuarios usuarios={usuarios} camposCadastro={camposCadastro} />);
};

export default ListaSistemaUsuarios;
