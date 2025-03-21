import React, { useEffect, useState } from "react";

import { usuariosDaClinicaAPI } from "../api/clinica/usuariosDaClinicaAPI.js";

import Navsidebar from "../components/organisms/Navsidebar.jsx";
import TemplateListaUsuarios from "../components/template/listagem/TemplateListaUsuarios.jsx";
import { Spin } from "antd";

const ListaUsuarios = () => {
  const [usuarios, setUsuarios] = useState([]);
  const [camposCadastro, setCamposCadastro] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchCamposCadastro = async () => {
          try {
            const url = usuariosDaClinicaAPI.listarCamposCadastroClinica("usuarios");
            console.log("Buscando campos em:", url);
            const response = await fetch(url);
            if (!response.ok) throw new Error("Erro ao buscar campos.");
            const data = await usuariosDaClinicaAPI.listarCamposCadastroClinica("usuarios");
            setCamposCadastro(data);
          } catch (error) {
            console.error("Erro ao buscar campos do usuário:", error);
          }
        };

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

    fetchCamposCadastro();
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
    <TemplateListaUsuarios
      usuarios={usuarios}
      camposCadastro={camposCadastro}
    />
  );
};

export default ListaUsuarios;
