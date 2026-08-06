import React from "react";
import { Button, Skeleton, Space } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  ProfileOutlined,
  UnorderedListOutlined,
} from "@ant-design/icons";
import { useAnimaisDaClinicaManager } from "./useAnimaisDaClinicaManager";
import FormCadastroAnimal from "./components/FormularioCadastroAnimaisDaClinica/FormCadastroAnimais";
import FormEdicaoAnimal from "./components/FormularioEdicaoAnimaisDaClinica/FormEdicaoAnimais";
import GerenciadorDeRacas from "./components/GerenciadorDeRacas/GerenciadorDeRacas";
import TabelaAnimais from "./components/TabelaAnimaisDaClinica/TabelaAnimaisDaClinica";
import FormDetalhesAnimal from "./components/FormularioDetalhesAnimaisDaClinica/FormDetalhesAnimais";
import ModalPadrao from "../../../components/modal/ModalPadrao";
const AnimaisDaClinica = () => {
  const {
    animais,
    total,
    page,
    pageSize,
    loading,
    loadingRowKey,
    isSubmitting,
    isModalAnimalOpen,
    isModalRacasOpen,
    isModalDetalhesOpen,
    editingAnimal,
    viewingAnimal,
    handleTableChange,
    handleOpenModal,
    handleCloseModal,
    handleOpenModalRacas,
    handleCloseModalRacas,
    handleNavigateToAcompanhamento,
    handleSubmit,
    handleDelete,
    handleOpenModalDetalhes,
    handleCloseModalDetalhes,
  } = useAnimaisDaClinicaManager();

  if (loading) {
    return <Skeleton active />;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
          Gerenciamento de Animais
        </h1>
        <Space>
          <Button onClick={handleOpenModalRacas}>Gerenciar Raças</Button>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => handleOpenModal()}
          >
            Cadastrar Animal
          </Button>
        </Space>
      </div>

      <TabelaAnimais
        data={animais}
        total={total}
        page={page}
        pageSize={pageSize}
        onTableChange={handleTableChange}
        onEdit={handleOpenModal}
        onDelete={handleDelete}
        onNavigateToAcompanhamento={handleNavigateToAcompanhamento}
        onViewDetails={handleOpenModalDetalhes}
        loadingRowKey={loadingRowKey}
      />

      {/* Modal de Criação / Edição de Animal */}
      <ModalPadrao
        modo={editingAnimal ? "edicao" : "criacao"}
        open={isModalAnimalOpen}
        onCancel={handleCloseModal}
        title={editingAnimal ? "Editar Animal" : "Cadastrar Novo Animal"}
        icon={editingAnimal ? <EditOutlined /> : <FormOutlined />}
        formId={
          editingAnimal
            ? "form-edicao-animal-clinica"
            : "form-cadastro-animal-clinica"
        }
        isSubmitting={isSubmitting}
      >
        {editingAnimal ? (
          <FormEdicaoAnimal
            initialValues={editingAnimal}
            onFinish={handleSubmit}
          />
        ) : (
          <FormCadastroAnimal onFinish={handleSubmit} />
        )}
      </ModalPadrao>

      {/* Modal de Gerenciamento de Raças */}
      <ModalPadrao
        modo="detalhes"
        open={isModalRacasOpen}
        onCancel={handleCloseModalRacas}
        title="Gerenciamento de Raças"
        icon={<UnorderedListOutlined />}
      >
        <GerenciadorDeRacas />
      </ModalPadrao>

      {/* Modal de Detalhes do Animal */}
      <ModalPadrao
        modo="detalhes"
        open={isModalDetalhesOpen}
        onCancel={handleCloseModalDetalhes}
        title={`Detalhes de ${viewingAnimal?.nome || "Animal"}`}
        icon={<ProfileOutlined />}
      >
        {viewingAnimal && <FormDetalhesAnimal valores={viewingAnimal} />}
      </ModalPadrao>
    </div>
  );
};

export default AnimaisDaClinica;
