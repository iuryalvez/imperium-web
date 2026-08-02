import React from "react";
import { Skeleton, Result, Button, Tabs, Modal, Space, Popconfirm } from "antd";
import { Link } from "react-router-dom";
import { FormOutlined, EditOutlined, ProfileOutlined, FileTextOutlined, DeleteOutlined } from "@ant-design/icons";
import dayjs from "dayjs";
import { useAcompanhamentoManager } from "./useAcompanhamentoManager";
import TabelaConsultasAvaliacoesDietas from "./components/TabelaConsultasAvaliacoesDietas";
import FormularioDieta from "./components/FormularioDieta";
import DetalhesDieta from "./components/DetalhesDieta";
import FichaGeralAnimal from "./components/FichaGeralAnimal";
import ModalPadrao from "../../../../components/modal/ModalPadrao";
import FormularioAvaliacao from "../../ConsultasDaClinica/components/FormularioAvaliacao";

const Acompanhamento = () => {
  const {
    historico,
    loading,
    racoes,
    isModalCriarDietaOpen,
    isModalVisualizarDietaOpen,
    isModalEditarDietaOpen,
    avaliacaoSelecionado,
    dietaParaVisualizar,
    dietaParaEditar,
    isSubmitting,
    isAvaliacaoModalOpen,
    selectedConsulta,
    avaliacaoData,
    loadingAvaliacao,
    handleOpenAvaliacaoModal,
    handleCloseAvaliacaoModal,
    handleSubmitAvaliacao,
    handleDeleteAvaliacao,
    handleOpenModalCriarDieta,
    handleCloseModalCriarDieta,
    handleOpenModalVisualizarDieta,
    handleCloseModalVisualizarDieta,
    handleOpenModalEditarDieta,
    handleCloseModalEditarDieta,
    handleCriarDieta,
    handleEditarDieta,
    handleExcluirDieta,
  } = useAcompanhamentoManager();

  if (loading) {
    return <Skeleton active paragraph={{ rows: 8 }} />;
  }

  if (!historico) {
    return (
      <Result
        status="404"
        title="Histórico não encontrado"
        subTitle="Não foi possível carregar os dados do acompanhamento."
        extra={
          <Button type="primary">
            <Link to="/clinica/animais">Voltar para a Lista</Link>
          </Button>
        }
      />
    );
  }

  // Ordena os relatórios pela data para extrair o mais recente de forma segura
  const avaliacoesOrdenados = [
    ...(historico.acompanhamento?.avaliacoes || []),
  ].sort((a, b) => new Date(b.dataConsulta) - new Date(a.dataConsulta));

  const avaliacaoMaisRecente = avaliacoesOrdenados[0];
  const dietaMaisRecente = avaliacoesOrdenados.find((rel) => rel.dieta)?.dieta;

  const tabItems = [
    {
      key: "1",
      label: "Ficha Geral",
      children: (
        <FichaGeralAnimal
          animal={historico}
          avaliacaoRecente={avaliacaoMaisRecente}
          dietaRecente={dietaMaisRecente}
        />
      ),
    },
    {
      key: "2",
      label: "Consultas",
      children: (
        <TabelaConsultasAvaliacoesDietas
          avaliacoes={historico.acompanhamento.avaliacoes}
          onOpenAvaliacao={handleOpenAvaliacaoModal}
          onOpenModalCriarDieta={handleOpenModalCriarDieta}
          onOpenModalVisualizarDieta={handleOpenModalVisualizarDieta}
          onOpenModalEditarDieta={handleOpenModalEditarDieta}
          onExcluirDieta={handleExcluirDieta}
        />
      ),
    },
  ];

  return (
    <div>
      <Tabs defaultActiveKey="1" items={tabItems} className="mt-4" />

      {/* Modal de Avaliação da Consulta */}
      <Modal
        title={
          <Space>
            <FileTextOutlined className="modal-title-icon" />
            <span className="text-verde-escuro m-0 text-lg">
              Avaliação da Consulta - {selectedConsulta?.nomeAnimal || historico?.nome}
            </span>
          </Space>
        }
        open={isAvaliacaoModalOpen}
        onCancel={handleCloseAvaliacaoModal}
        style={{ top: 40 }}
        footer={
          <div className="modal-footer-actions">
            <Button onClick={handleCloseAvaliacaoModal}>Cancelar</Button>

            {avaliacaoData && (
              <Popconfirm
                title="Excluir relatório"
                description="Tem certeza que deseja excluir este relatório? Esta ação não pode ser desfeita."
                onConfirm={() => handleDeleteAvaliacao(avaliacaoData.id)}
                okText="Sim, Excluir"
                cancelText="Cancelar"
              >
                <Button icon={<DeleteOutlined />} danger>
                  Excluir
                </Button>
              </Popconfirm>
            )}

            <Button
              form="form-avaliacao-consulta-clinica"
              type="primary"
              htmlType="submit"
            >
              Salvar Avaliação
            </Button>
          </div>
        }
        destroyOnClose
        width="min(800px, 95vw)"
      >
        <div className="modal-scroll-container">
          {loadingAvaliacao ? (
            <Skeleton active />
          ) : (
            <FormularioAvaliacao
              consulta={selectedConsulta || { especieAnimal: historico?.especie, sexoAnimal: historico?.sexo }}
              initialValues={avaliacaoData}
              onFinish={handleSubmitAvaliacao}
            />
          )}
        </div>
      </Modal>

      {/* Modal Padrao: Criar Dieta */}
      <ModalPadrao
        modo="criacao"
        open={isModalCriarDietaOpen}
        onCancel={handleCloseModalCriarDieta}
        title={`Dieta - Paciente: ${historico?.nome || "Desconhecido"} (${dayjs(avaliacaoSelecionado?.dataConsulta).format("DD/MM/YYYY")})`}
        icon={<FormOutlined />}
        formId="form-dieta-animal"
        isSubmitting={isSubmitting}
        width="min(700px, 95vw)"
      >
        {avaliacaoSelecionado && (
          <FormularioDieta
            onFinish={handleCriarDieta}
            avaliacaoId={avaliacaoSelecionado.id}
            racoes={racoes}
          />
        )}
      </ModalPadrao>

      {/* Modal Padrao: Editar Dieta */}
      <ModalPadrao
        modo="edicao"
        open={isModalEditarDietaOpen}
        onCancel={handleCloseModalEditarDieta}
        title={`Editar Dieta - Paciente: ${historico?.nome || "Desconhecido"} (${dayjs(dietaParaEditar?.dataConsulta).format("DD/MM/YYYY")})`}
        icon={<EditOutlined />}
        formId="form-dieta-animal"
        isSubmitting={isSubmitting}
        width="min(700px, 95vw)"
      >
        {dietaParaEditar && (
          <FormularioDieta
            onFinish={handleEditarDieta}
            initialValues={dietaParaEditar}
            avaliacaoId={dietaParaEditar.avaliacaoId}
            racoes={racoes}
          />
        )}
      </ModalPadrao>

      {/* Modal Padrao: Visualizar Dieta (Detalhes) */}
      <ModalPadrao
        modo="detalhes"
        open={isModalVisualizarDietaOpen}
        onCancel={handleCloseModalVisualizarDieta}
        title="Detalhes da Dieta Calculada"
        icon={<ProfileOutlined />}
        width="min(600px, 95vw)"
      >
        {dietaParaVisualizar && <DetalhesDieta dieta={dietaParaVisualizar} />}
      </ModalPadrao>
    </div>
  );
};

export default Acompanhamento;
