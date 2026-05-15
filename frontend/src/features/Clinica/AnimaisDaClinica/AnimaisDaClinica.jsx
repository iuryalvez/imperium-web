import React from "react";
import { Button, Input, Modal, Skeleton, Space } from "antd";
import { PlusOutlined, SearchOutlined } from "@ant-design/icons";
import { useAnimaisDaClinicaManager } from "./useAnimaisDaClinicaManager";
import FormCadastroAnimal from "./components/FormularioCadastroAnimaisDaClinica/FormCadastroAnimais";
import FormEdicaoAnimal from "./components/FormularioEdicaoAnimaisDaClinica/FormEdicaoAnimais";
import GerenciadorDeRacas from "./components/GerenciadorDeRacas/GerenciadorDeRacas";
import TabelaAnimais from "./components/TabelaAnimaisDaClinica/TabelaAnimaisDaClinica";
import FormDetalhesAnimal from "./components/FormularioDetalhesAnimaisDaClinica/FormDetalhesAnimais";

const AnimaisDaClinica = () => {
  const {
    animais,
    loading,
    loadingRowKey,
    searchTerm,
    isSubmitting,
    isModalAnimalOpen,
    isModalRacasOpen,
    isModalDetalhesOpen,
    editingAnimal,
    viewingAnimal,
    handleSearch,
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
        <h1 className="text-2xl font-bold">Gerenciamento de Animais</h1>
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

      {/* <Input
        placeholder="Buscar animal pelo nome..."
        prefix={<SearchOutlined />}
        value={searchTerm}
        onChange={handleSearch}
        style={{ marginBottom: 16 }}
        allowClear
      /> */}

      <TabelaAnimais
        data={animais}
        onEdit={handleOpenModal}
        onDelete={handleDelete}
        onNavigateToAcompanhamento={handleNavigateToAcompanhamento}
        onViewDetails={handleOpenModalDetalhes}
        loadingRowKey={loadingRowKey}
      />

      <Modal
        title={editingAnimal ? "Editar Animal" : "Cadastrar Novo Animal"}
        open={isModalAnimalOpen}
        onCancel={handleCloseModal}
        footer={null}
        destroyOnClose
        width="min(800px, 95vw)"
      >
        {/* Renderiza o formulário correto com base em 'editingAnimal' */}
        {editingAnimal ? (
          <FormEdicaoAnimal
            initialValues={editingAnimal}
            onFinish={handleSubmit}
            onCancel={handleCloseModal}
            isSubmitting={isSubmitting}
          />
        ) : (
          <FormCadastroAnimal
            onFinish={handleSubmit}
            onCancel={handleCloseModal}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>

      <Modal
        title="Gerenciamento de Raças"
        open={isModalRacasOpen}
        onCancel={handleCloseModalRacas}
        footer={null}
        destroyOnClose
        width="min(800px, 95vw)"
      >
        <GerenciadorDeRacas />
      </Modal>

      <Modal
        title="Detalhes do Animal"
        open={isModalDetalhesOpen}
        onCancel={handleCloseModalDetalhes}
        footer={[
          <Button key="close" onClick={handleCloseModalDetalhes}>
            Fechar
          </Button>,
        ]}
        destroyOnClose
        width="min(800px, 95vw)"
        centered
        styles={{ body: { maxHeight: "70vh", overflowY: "auto" } }}
      >
        {/* Renderiza o componente de detalhes apenas se houver um animal selecionado */}
        {viewingAnimal && <FormDetalhesAnimal valores={viewingAnimal} />}
      </Modal>
    </div>
  );
};

export default AnimaisDaClinica;
