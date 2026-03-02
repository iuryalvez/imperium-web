import React from "react";
import { ArrowLeftOutlined } from "@ant-design/icons";
import LoadingSpinner from "../../components/atoms/LoadingSpinner.jsx";
import { usePerfilPessoalManager } from "./usePerfilPessoalManager.js";
import FormularioEdicaoPerfilPessoal from "./components/FormularioEdicaoPerfilPessoal.jsx";

const PerfilPessoal = () => {
  const { perfil, loading, isSubmitting, handleSubmit } = usePerfilPessoalManager();

  if (loading) {
    return <LoadingSpinner />;
  }

  return (
    <div>
      <button
        onClick={() => window.history.back()}
        className="flex items-center font-bold text-[var(--verde-medio)] mb-2"
      >
        <ArrowLeftOutlined className="mr-2" /> Voltar
      </button>
      
      <div className="mb-4">
        <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
          Gerenciamento de Perfil Pessoal
        </h1>
        <p className="text-gray-500 text-sm">
          As alterações feitas aqui refletirão em todas as clínicas que você possui acesso.
        </p>
      </div>

      <div className="bg-white p-6 rounded-lg shadow max-w-4xl">
        <FormularioEdicaoPerfilPessoal
          initialValues={perfil}
          onFinish={handleSubmit}
          isSubmitting={isSubmitting}
        />
      </div>
    </div>
  );
};

export default PerfilPessoal;