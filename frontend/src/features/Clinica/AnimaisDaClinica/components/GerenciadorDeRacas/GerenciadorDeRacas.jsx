import React from "react";
import {
  List,
  Button,
  Space,
  Modal,
  Popconfirm,
  Tooltip,
  Tag,
  Input,
  Divider,
} from "antd";
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
} from "@ant-design/icons";
import { useRacasDaClinicaManager } from "./useRacasDaClinicaManager";
import FormCadastroRaca from "./FormCadastroRaca";
import FormEdicaoRaca from "./FormEdicaoRaca";

const GerenciadorDeRacas = () => {
  const {
    loading,
    isModalOpen,
    isSubmitting,
    editingRaca,
    filteredRacas,
    searchTerm,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    handleSearch,
    handleFilterByEspecie,
  } = useRacasDaClinicaManager();

  return (
    <div>
      <div
        style={{
          display: "flex",
          flexDirection: "column",
          gap: 16,
          marginBottom: 16,
        }}
      >
        <div
          style={{
            display: "flex",
            gap: 16,
            alignItems: "center",
            flexWrap: "wrap",
          }}
        >
          <Input.Search
            placeholder="Ex: Golden Retriever"
            value={searchTerm}
            onChange={handleSearch}
            style={{ flex: 1, minWidth: 200 }}
            allowClear
          />
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => handleOpenModal()}
          >
            Nova Raça
          </Button>
        </div>
        <div>
          <Space wrap>
            <Button onClick={() => handleFilterByEspecie("Cao")}>Raças de Cães</Button>
            <Button onClick={() => handleFilterByEspecie("Gato")}>Raças de Gatos</Button>
            <Button onClick={() => handleFilterByEspecie(null)}>Todos</Button>
          </Space>
        </div>
      </div>

      <Divider />

      <List
        loading={loading}
        itemLayout="horizontal"
        dataSource={filteredRacas}
        renderItem={(raca) => (
          <List.Item
            actions={[
              <Tooltip title="Editar">
                <Button
                  type="text"
                  icon={<EditOutlined />}
                  onClick={() => handleOpenModal(raca)}
                />
              </Tooltip>,
              <Tooltip title="Excluir">
                <Popconfirm
                  title="Tem certeza?"
                  onConfirm={() => handleDelete(raca.id)}
                  okText="Sim"
                  cancelText="Não"
                >
                  <Button type="text" icon={<DeleteOutlined />} danger />
                </Popconfirm>
              </Tooltip>,
            ]}
          >
            <List.Item.Meta
              title={raca.nome}
              description={
                <Tag color={raca.especie === "Cao" ? "blue" : "green"}>
                  {raca.especie}
                </Tag>
              }
            />
          </List.Item>
        )}
        locale={{
          emptyText:
            "Nenhuma raça encontrada. Use a busca ou os filtros acima.",
        }}
      />

      <Modal
        // O título muda dinamicamente com base em 'editingRaca'
        title={editingRaca ? "Editar Raça" : "Cadastrar Nova Raça"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        footer={null}
        destroyOnClose
        centered
      >
        {/* Renderiza o formulário correto condicionalmente DENTRO do modal */}
        {editingRaca ? (
          <FormEdicaoRaca
            initialValues={editingRaca}
            onFinish={handleSubmit}
            onCancel={handleCloseModal}
            isSubmitting={isSubmitting}
          />
        ) : (
          <FormCadastroRaca
            onFinish={handleSubmit}
            onCancel={handleCloseModal}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>
    </div>
  );
};

export default GerenciadorDeRacas;
