import React from "react";
import {
  UserOutlined,
  ShopOutlined,
  CalendarOutlined,
} from "@ant-design/icons";
import { Avatar, Divider } from "antd";
import { useLocation } from "react-router-dom";
import PaginaPadrao from "../../../components/layout/PaginaPadrao.jsx";
import { useAuth } from "../../../services/AuthContext";
import LoadingSpinner from "../../../components/atoms/LoadingSpinner.jsx";
import { usePerfilPessoalManager } from "./usePerfilPessoalManager.js";
import FormularioEdicaoPerfilPessoal from "./components/FormularioEdicaoPerfilPessoal.jsx";

const PerfilPessoal = () => {
  const { perfil, loading, isSubmitting, handleSubmit } =
    usePerfilPessoalManager();
  const { pathname } = useLocation();
  const { isAdmin } = useAuth();

  // Breadcrumb só fora da clínica (/sistema/perfil): dentro dela não há uma tela "raiz" para apontar
  const naClinica = pathname.startsWith("/clinica");
  const inicio = isAdmin
    ? { titulo: "Início", para: "/sistema" }
    : { titulo: "Vínculos", para: "/sistema/vinculos" };

  if (loading) {
    return <LoadingSpinner />;
  }

  return (
    <PaginaPadrao
      preencher={false}
      titulo="Gerenciamento de Perfil Pessoal"
      descricao="As alterações feitas aqui refletirão em todas as clínicas que você possui acesso."
      // Breadcrumb só fora da clínica (/sistema/perfil)
      trilha={naClinica ? undefined : [inicio]}
    >
      {/* Grid layout: 1 column on mobile, 3 columns on large screens. 
        Form takes 2 columns, Summary card takes 1.
      */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-100 h-fit">
          <div className="flex flex-col items-center text-center">
            <Avatar
              size={80}
              icon={<UserOutlined />}
              className="bg-[var(--verde-medio)] mb-4"
            />
            <h2 className="text-lg font-bold text-gray-800">{perfil?.nome}</h2>
            <p className="text-gray-500">{perfil?.email}</p>
          </div>

          <Divider />

          <div className="space-y-4">
            <h3 className="font-semibold text-gray-700 mb-4">
              Resumo da Conta
            </h3>
            <div className="flex items-center justify-between text-sm text-gray-600">
              <div className="flex items-center">
                <CalendarOutlined className="mr-2 text-[var(--verde-medio)]" />
                <span>Membro desde</span>
              </div>
              <strong>{perfil?.dataCadastro}</strong>
            </div>

            <div className="flex items-center justify-between text-sm text-gray-600">
              <div className="flex items-center">
                <ShopOutlined className="mr-2 text-[var(--verde-medio)]" />
                <span>Clínicas vinculadas</span>
              </div>
              <strong>
                {perfil?.totalClinicasVinculadas > 0
                  ? perfil.totalClinicasVinculadas
                  : "Não há"}
              </strong>
            </div>
          </div>
        </div>

        <div className="lg:col-span-2 bg-white p-6 rounded-lg shadow-sm border border-gray-100">
          <FormularioEdicaoPerfilPessoal
            initialValues={perfil}
            onFinish={handleSubmit}
            isSubmitting={isSubmitting}
          />
        </div>
      </div>
    </PaginaPadrao>
  );
};

export default PerfilPessoal;
