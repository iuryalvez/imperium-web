import React from "react";
import { Skeleton, Result, Button, Tabs, Modal, Space, Popconfirm } from "antd";
import { Link } from "react-router-dom";
import { FormOutlined, EditOutlined, ProfileOutlined, FileTextOutlined, DeleteOutlined } from "@ant-design/icons";
import dayjs from "dayjs";
import { useAcompanhamentoManager } from "./useAcompanhamentoManager";
import ListaConsultasCards from "./components/ListaConsultasCards";
import FormularioDieta from "./components/FormularioDieta";
import DetalhesDieta from "./components/DetalhesDieta";
import FichaGeralAnimal from "./components/FichaGeralAnimal";
import ModalPadrao from "../../../../components/modal/ModalPadrao";
import FormularioAvaliacao from "../../ConsultasDaClinica/components/FormularioAvaliacao";
import FormularioConsulta from "../../ConsultasDaClinica/components/FormularioConsulta";
import FormEdicaoAnimal from "../components/FormularioEdicaoAnimaisDaClinica/FormEdicaoAnimais";

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
    handleDeleteConsulta,
    handleEditConsulta,
    handleCloseConsultaModal,
    handleSubmitConsulta,
    isConsultaModalOpen,
    consultaToEdit,
    isEditAnimalModalOpen,
    handleOpenEditAnimalModal,
    handleCloseEditAnimalModal,
    handleSubmitEditAnimal,
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

  // Extrai a avaliação e a dieta mais recentes a partir do histórico de consultas
  const consultas = historico.consultas || [];
  const avaliacaoMaisRecente = consultas.find((c) => c.avaliacao)?.avaliacao;
  const dietaMaisRecente = consultas.find((c) => c.avaliacao?.dieta)?.avaliacao.dieta;

  return (
    <div className="mt-4 flex flex-col gap-6">
      <FichaGeralAnimal
        animal={historico}
        avaliacaoRecente={avaliacaoMaisRecente}
        dietaRecente={dietaMaisRecente}
        onOpenDieta={handleOpenModalVisualizarDieta}
        onEditAnimal={handleOpenEditAnimalModal}
      />
      
      <div>
        <h2 className="text-xl font-semibold text-verde-escuro mb-4">
          Consultas e Acompanhamento
        </h2>
        <ListaConsultasCards
          consultas={consultas}
          onEditConsulta={handleEditConsulta}
          onDeleteConsulta={handleDeleteConsulta}
          onOpenAvaliacao={handleOpenAvaliacaoModal}
          onDeleteAvaliacao={handleDeleteAvaliacao}
          onOpenModalCriarDieta={handleOpenModalCriarDieta}
          onOpenModalVisualizarDieta={handleOpenModalVisualizarDieta}
          onOpenModalEditarDieta={handleOpenModalEditarDieta}
          onExcluirDieta={handleExcluirDieta}
        />
      </div>

      {/* Modal de Edição da Consulta */}
      <Modal
        title="Editar Consulta"
        open={isConsultaModalOpen}
        onCancel={handleCloseConsultaModal}
        footer={null}
        destroyOnClose
      >
        <FormularioConsulta
          initialValues={{ dataConsulta: consultaToEdit?.dataConsulta, animalId: historico?.id, tutorId: consultaToEdit?.tutorId }}
          onFinish={handleSubmitConsulta}
        />
      </Modal>

      {/* Modal de Edição do Animal */}
      <ModalPadrao
        modo="edicao"
        open={isEditAnimalModalOpen}
        onCancel={handleCloseEditAnimalModal}
        title="Editar Animal"
        icon={<EditOutlined />}
        formId="form-edicao-animal-clinica"
        isSubmitting={isSubmitting}
      >
        {historico && isEditAnimalModalOpen && (
          <FormEdicaoAnimal
            initialValues={{
              ...historico,
              especie: historico.especie === "Cao" ? 1 : 2,
              sexo: historico.sexo === "Macho" ? 1 : 2,
              racaId: historico.raca?.id,
              tutorIds: historico.tutores?.map((t) => t.id) || [],
            }}
            onFinish={handleSubmitEditAnimal}
          />
        )}
      </ModalPadrao>

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
