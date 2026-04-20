import React from "react";
import {
  ArrowLeftOutlined,
  UserOutlined,
  ShopOutlined,
  CalendarOutlined,
} from "@ant-design/icons";
import { Avatar, Divider } from "antd";
import LoadingSpinner from "../../components/atoms/LoadingSpinner.jsx";
import { usePerfilPessoalManager } from "./usePerfilPessoalManager.js";
import FormularioEdicaoPerfilPessoal from "./components/FormularioEdicaoPerfilPessoal.jsx";

const PerfilPessoal = () => {
  const { perfil, loading, isSubmitting, handleSubmit } =
    usePerfilPessoalManager();

  if (loading) {
    return <LoadingSpinner />;
  }

  return (
    <div className="max-w-7xl mx-auto">
      <button
        onClick={() => window.history.back()}
        className="flex items-center font-bold text-[var(--verde-medio)] mb-4 hover:opacity-80 transition-opacity"
      >
        <ArrowLeftOutlined className="mr-2" /> Voltar
      </button>

      <div className="mb-6">
        <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
          Gerenciamento de Perfil Pessoal
        </h1>
        <p className="text-gray-500 text-sm">
          As alterações feitas aqui refletirão em todas as clínicas que você
          possui acesso.
        </p>
      </div>

      {/* Grid layout: 1 column on mobile, 3 columns on large screens. 
        Form takes 2 columns, Summary card takes 1.
      */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="bg-white p-6 rounded-lg shadow h-fit">
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

        <div className="lg:col-span-2 bg-white p-6 rounded-lg shadow">
          <FormularioEdicaoPerfilPessoal
            initialValues={perfil}
            onFinish={handleSubmit}
            isSubmitting={isSubmitting}
          />
        </div>
      </div>
    </div>
  );
};

export default PerfilPessoal;
