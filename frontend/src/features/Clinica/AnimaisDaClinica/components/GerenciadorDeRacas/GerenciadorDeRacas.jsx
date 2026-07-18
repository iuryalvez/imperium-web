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
  Select,
} from "antd";
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  GlobalOutlined,
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
    mostrarRacasSistema,
    setMostrarRacasSistema,
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
          gap: 16,
          alignItems: "center",
          flexWrap: "wrap",
          justifyContent: "space-between",
        }}
      >
        <Space wrap>
          <Input.Search
            placeholder="Buscar por nome..."
            value={searchTerm}
            onChange={handleSearch}
            style={{ maxWidth: 300, flex: 1 }}
            allowClear
          />

          {/* Filters grouped into Select components */}
          <Select
            defaultValue={null}
            style={{ width: 200 }}
            onChange={(value) => handleFilterByEspecie(value)}
            options={[
              { value: null, label: "Todas as Espécies" },
              { value: "Cao", label: "Cães" },
              { value: "Gato", label: "Gatos" },
            ]}
          />

          <Select
            value={mostrarRacasSistema ? "todas" : "clinica"}
            style={{ width: 170 }}
            onChange={(value) => setMostrarRacasSistema(value === "todas")}
            options={[
              { value: "todas", label: "Todas as Origens" },
              { value: "clinica", label: "Apenas da Clínica" },
            ]}
          />
        </Space>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => handleOpenModal()}
          style={{ marginRight: 8 }}
        >
          Nova Raça
        </Button>
      </div>

      <Divider />

      <List
        loading={loading}
        grid={{ gutter: 16, column: 3, xs: 1, sm: 2, md: 3 }}
        dataSource={filteredRacas}
        pagination={{
          pageSize: 12,
          align: "center",
          size: "small",
        }}
        renderItem={(raca) => {
          const isSystemBreed =
            raca.clinicaId === null || raca.clinicaId === undefined;

          return (
            <List.Item
              style={{
                border: "1px solid #f0f0f0",
                borderRadius: 8,
                padding: "12px 16px",
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                marginBottom: 12,
              }}
            >
              <List.Item.Meta
                title={
                  <Space>
                    {raca.nome}
                    {isSystemBreed && (
                      <Tooltip title="Raça padrão do sistema">
                        <GlobalOutlined style={{ color: "#1890ff" }} />
                      </Tooltip>
                    )}
                  </Space>
                }
                description={
                  <Tag
                    color={raca.especie === "Cao" ? "blue" : "green"}
                    style={{ marginTop: 4 }}
                  >
                    {raca.especie}
                  </Tag>
                }
              />
              <Space size="small">
                <Tooltip title={isSystemBreed ? "Personalizar Raça" : "Editar"}>
                  <Button
                    type="text"
                    icon={<EditOutlined />}
                    onClick={() => handleOpenModal(raca)}
                  />
                </Tooltip>
                {!isSystemBreed && (
                  <Tooltip title="Excluir">
                    <Popconfirm
                      title="Deseja excluir esta raça?"
                      onConfirm={() => handleDelete(raca.id)}
                      okText="Sim"
                      cancelText="Não"
                    >
                      <Button type="text" icon={<DeleteOutlined />} danger />
                    </Popconfirm>
                  </Tooltip>
                )}
              </Space>
            </List.Item>
          );
        }}
        locale={{
          emptyText:
            "Nenhuma raça encontrada. Use a busca ou os filtros acima.",
        }}
      />

      <Modal
        title={editingRaca ? "Editar Raça" : "Cadastrar Nova Raça"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        footer={null}
        destroyOnClose
        centered
      >
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
