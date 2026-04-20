import React from "react";
import { Card, Statistic, Button, Modal, Avatar, Divider } from "antd";
import { 
  EditOutlined, UserOutlined, TeamOutlined, 
  FileTextOutlined, MedicineBoxOutlined, HeartOutlined 
} from "@ant-design/icons";
import LoadingSpinner from "../../components/atoms/LoadingSpinner.jsx";
import { usePerfilClinicaManager } from "./usePerfilClinicaManager";
import FormularioEdicaoClinica from "./components/FormularioEdicaoClinica.jsx";

const PerfilClinica = () => {
  const {
    dadosClinica,
    loading,
    isSubmitting,
    isModalOpen,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
  } = usePerfilClinicaManager();

  if (loading) {
    return <LoadingSpinner />;
  }

  return (
    <div className="max-w-7xl mx-auto">
      <div className="flex flex-col md:flex-row justify-between items-center bg-white p-6 rounded-lg shadow mb-6">
        <div className="flex items-center gap-4">
          <Avatar 
            size={80} 
            src={dadosClinica?.logoUrl} 
            icon={!dadosClinica?.logoUrl && <MedicineBoxOutlined />} 
            className="bg-[var(--verde-medio)]"
          />
          <div>
            <h1 className="text-2xl font-bold text-[var(--verde-medio)] m-0">
              {dadosClinica?.nomeFantasia || dadosClinica?.nomeClinica}
            </h1>
            <p className="text-gray-500 m-0">CNPJ: {dadosClinica?.cnpj?.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, "$1.$2.$3/$4-$5") || "Não informado"}</p>
            <p className="text-xs text-gray-400 mt-1">
              Cadastrada em: {dadosClinica?.dataCadastro ? new Date(dadosClinica.dataCadastro).toLocaleDateString('pt-BR') : '--/--/----'}
            </p>
          </div>
        </div>
        <Button 
          type="primary" 
          icon={<EditOutlined />} 
          onClick={handleOpenModal}
          className="mt-4 md:mt-0"
        >
          Editar Informações
        </Button>
      </div>

      <Divider className="my-2" />
      <h2 className="text-xl font-semibold text-gray-700 mb-4">Visão Geral da Clínica</h2>

      {/* Grid de Estatísticas */}
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        <Card className="shadow-sm hover:shadow-md transition-shadow">
          <Statistic title="Usuários Vinculados" value={dadosClinica?.totalUsuarios || 0} prefix={<TeamOutlined className="text-[var(--verde-medio)]" />} />
        </Card>
        <Card className="shadow-sm hover:shadow-md transition-shadow">
          <Statistic title="Tutores Cadastrados" value={dadosClinica?.totalTutores || 0} prefix={<UserOutlined className="text-[var(--verde-medio)]" />} />
        </Card>
        <Card className="shadow-sm hover:shadow-md transition-shadow">
          <Statistic title="Animais" value={dadosClinica?.totalAnimais || 0} prefix={<HeartOutlined className="text-[var(--verde-medio)]" />} />
        </Card>
        <Card className="shadow-sm hover:shadow-md transition-shadow">
          <Statistic title="Consultas Realizadas" value={dadosClinica?.totalConsultas || 0} prefix={<MedicineBoxOutlined className="text-[var(--verde-medio)]" />} />
        </Card>
        <Card className="shadow-sm hover:shadow-md transition-shadow">
          <Statistic title="Acompanhamentos" value={dadosClinica?.totalAcompanhamentos || 0} prefix={<FileTextOutlined className="text-[var(--verde-medio)]" />} />
        </Card>
        {/* Adicione mais cards conforme os dados do seu DTO (Raças, Rações, etc) */}
      </div>

      <Modal
        title="Editar Informações da Clínica"
        open={isModalOpen}
        onCancel={handleCloseModal}
        footer={null}
        width={800}
        destroyOnClose
        centered
      >
        <FormularioEdicaoClinica 
          initialValues={dadosClinica} 
          onFinish={handleSubmit} 
          isSubmitting={isSubmitting} 
        />
      </Modal>
    </div>
  );
};

export default PerfilClinica;