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
  EyeOutlined,
  EyeInvisibleOutlined,
  StopOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import { useRacasDaClinicaManager } from "./useRacasDaClinicaManager";
import { usePermissao } from "../../../../../utils/usePermissao";
import FormCadastroRaca from "./FormCadastroRaca";
import FormEdicaoRaca from "./FormEdicaoRaca";

const GerenciadorDeRacas = () => {
  const {
    racas,
    loading,
    paginacao,
    loadingItemId,
    isModalOpen,
    isSubmitting,
    editingRaca,
    setBusca,
    setFiltros,
    setPagina,
    handleFiltroStatus,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    handleToggleStatus,
    handleToggleOculta,
  } = useRacasDaClinicaManager();

  const pode = usePermissao();
  const podeEditar = pode("gerenciaAnimais");

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
            onChange={(e) => setBusca(e.target.value)}
            style={{ maxWidth: 300, flex: 1 }}
            allowClear
          />

          {/* Filtros aplicados no servidor */}
          <Select
            defaultValue={null}
            style={{ width: 170 }}
            onChange={(value) => setFiltros({ especie: value })}
            options={[
              { value: null, label: "Todas as Espécies" },
              { value: 1, label: "Cães" },
              { value: 2, label: "Gatos" },
            ]}
          />

          <Select
            defaultValue={null}
            style={{ width: 170 }}
            onChange={(value) => setFiltros({ origem: value })}
            options={[
              { value: null, label: "Todas as Origens" },
              { value: "sistema", label: "Apenas do Sistema" },
              { value: "clinica", label: "Apenas da Clínica" },
            ]}
          />

          <Select
            defaultValue={null}
            style={{ width: 170 }}
            onChange={handleFiltroStatus}
            options={[
              { value: null, label: "Todos os Status" },
              { value: "ativos", label: "Ativas" },
              { value: "inativos", label: "Inativas" },
              { value: "ocultas", label: "Ocultas" },
            ]}
          />
        </Space>
        {podeEditar && (
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => handleOpenModal()}
            style={{ marginRight: 8 }}
          >
            Nova Raça
          </Button>
        )}
      </div>

      <Divider />

      <List
        loading={loading}
        grid={{ gutter: 16, column: 3, xs: 1, sm: 2, md: 3 }}
        dataSource={racas}
        pagination={{
          ...paginacao,
          pageSizeOptions: [12, 24, 48, 96],
          onChange: setPagina,
          align: "center",
          size: "small",
        }}
        renderItem={(raca) => {
          const isSystemBreed = raca.isSistema;

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
                opacity: raca.isAtivo && !raca.isOculta ? 1 : 0.6,
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
                  <Space size={4} wrap style={{ marginTop: 4 }}>
                    <Tag color={raca.especie === "Cao" ? "blue" : "green"}>
                      {raca.especie}
                    </Tag>
                    {!raca.isAtivo && <Tag color="red">Inativa</Tag>}
                    {raca.isOculta && <Tag>Oculta</Tag>}
                  </Space>
                }
              />
              {/* Ações só para quem pode gerenciar raças (GESTOR, VETERINARIO, TECNICO) */}
              {podeEditar && (
              <Space size="small">
                <Tooltip title={isSystemBreed ? "Personalizar Raça (cria uma cópia da clínica)" : "Editar"}>
                  <Button
                    type="text"
                    icon={<EditOutlined />}
                    onClick={() => handleOpenModal(raca)}
                  />
                </Tooltip>
                {isSystemBreed ? (
                  <Tooltip title={raca.isOculta ? "Mostrar para esta clínica" : "Ocultar para esta clínica"}>
                    <Popconfirm
                      title={raca.isOculta ? "Voltar a mostrar esta raça?" : "Ocultar esta raça para a clínica?"}
                      onConfirm={() => handleToggleOculta(raca)}
                      okText="Sim"
                      cancelText="Não"
                    >
                      <Button
                        type="text"
                        loading={loadingItemId === raca.id}
                        icon={raca.isOculta ? <EyeOutlined /> : <EyeInvisibleOutlined />}
                      />
                    </Popconfirm>
                  </Tooltip>
                ) : (
                  <>
                    <Tooltip title={raca.isAtivo ? "Desativar" : "Ativar"}>
                      <Popconfirm
                        title={`Deseja ${raca.isAtivo ? "desativar" : "ativar"} esta raça?`}
                        onConfirm={() => handleToggleStatus(raca)}
                        okText="Sim"
                        cancelText="Não"
                      >
                        <Button
                          type="text"
                          loading={loadingItemId === raca.id}
                          icon={raca.isAtivo ? <StopOutlined /> : <CheckCircleOutlined />}
                        />
                      </Popconfirm>
                    </Tooltip>
                    <Tooltip title="Excluir">
                      <Popconfirm
                        title="Deseja excluir esta raça?"
                        onConfirm={() => handleDelete(raca)}
                        okText="Sim"
                        cancelText="Não"
                      >
                        <Button type="text" icon={<DeleteOutlined />} danger />
                      </Popconfirm>
                    </Tooltip>
                  </>
                )}
              </Space>
              )}
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
