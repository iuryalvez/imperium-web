import React from "react";
import {
  Button,
  Col,
  Modal,
  Popconfirm,
  Result,
  Row,
  Skeleton,
  Space,
} from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  FileTextOutlined,
  DeleteOutlined,
  CalculatorOutlined,
  LineChartOutlined,
  InfoCircleOutlined,
  ProfileOutlined,
} from "@ant-design/icons";
import { useConsultasManager } from "./useConsultasManager";
import TabelaConsultas from "./components/TabelaConsultas";
import FormularioConsulta from "./components/FormularioConsulta";
import FormularioAvaliacao from "./components/FormularioAvaliacao";
import ModalPadrao from "../../../components/modal/ModalPadrao";
import FormularioDieta from "../AnimaisDaClinica/Acompanhamento/components/FormularioDieta";
import DetalhesDieta from "../AnimaisDaClinica/Acompanhamento/components/DetalhesDieta";

const ConsultasDaClinica = () => {
  const {
    consultas,
    loading,
    isModalOpen,
    editingConsulta,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    isAvaliacaoModalOpen,
    selectedConsulta,
    avaliacaoData,
    loadingAvaliacao,
    handleOpenAvaliacaoModal,
    handleCloseAvaliacaoModal,
    handleSubmitAvaliacao,
    handleDeleteAvaliacao,
    isNextActionModalOpen,
    idsSalvos,
    handleNextActionClose,
    handleIrParaAcompanhamento,
    isModalDietaOpen,
    racoesDisponiveis,
    isSubmittingDieta,
    handleCalcularDietaPosAvaliacao,
    handleCloseModalDieta,
    handleSubmitDieta,
    handleCalcularDietaDaTabela,
    isModalVisualizarDietaOpen,
    dietaParaVisualizar,
    handleVerDietaDaTabela,
    handleCloseModalVisualizarDieta,
  } = useConsultasManager();

  if (loading) {
    return <Skeleton active />;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
          Gerenciamento de Consultas
        </h1>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => handleOpenModal()}
        >
          Agendar Consulta
        </Button>
      </div>

      <TabelaConsultas
        consultas={consultas}
        onEdit={handleOpenModal}
        onDelete={handleDelete}
        onOpenAvaliacao={handleOpenAvaliacaoModal}
        onVerDieta={handleVerDietaDaTabela}
        onCalcularDieta={handleCalcularDietaDaTabela}
        onIrParaAcompanhamento={handleIrParaAcompanhamento}
        loading={loading}
      />

      {/* Modal de Avaliação da Consulta */}
      <Modal
        title={
          <Space>
            <FileTextOutlined className="modal-title-icon" />
            <span className="text-verde-escuro m-0 text-lg">
              Avaliação da Consulta - {selectedConsulta?.nomeAnimal || ""}
            </span>
          </Space>
        }
        open={isAvaliacaoModalOpen}
        onCancel={handleCloseAvaliacaoModal}
        style={{ top: 40 }} // Mantém o modal travado no topo, igual ao ModalPadrao
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
              consulta={selectedConsulta}
              initialValues={avaliacaoData}
              onFinish={handleSubmitAvaliacao}
            />
          )}
        </div>
      </Modal>

      {/* Modal de Criação / Edição de Consulta */}
      <ModalPadrao
        modo={editingConsulta ? "edicao" : "criacao"}
        open={isModalOpen}
        onCancel={handleCloseModal}
        title={editingConsulta ? "Editar Consulta" : "Agendar Nova Consulta"}
        icon={editingConsulta ? <EditOutlined /> : <FormOutlined />}
        formId="form-cadastro-edicao-consulta"
        width="min(500px, 95vw)"
      >
        <FormularioConsulta
          initialValues={editingConsulta}
          onFinish={handleSubmit}
        />
      </ModalPadrao>

      {/* Modal de Próxima Ação */}
      <Modal
        open={isNextActionModalOpen}
        onCancel={handleNextActionClose}
        footer={null}
        closable={false}
        centered
        width={450}
      >
        <Result
          status="success"
          title="Avaliação salva com sucesso!"
          subTitle="O histórico clínico do paciente foi atualizado. O que deseja fazer agora?"
          extra={
            <Row gutter={[0, 12]} justify="center" className="mt-4">
              <Col span={24}>
                <Button
                  key="calc"
                  type="primary"
                  size="large"
                  icon={<CalculatorOutlined />}
                  onClick={handleCalcularDietaPosAvaliacao}
                  block // Faz o botão ocupar 100% da Col
                >
                  Calcular Dieta Nutricional
                </Button>
              </Col>
              <Col span={24}>
                <Button
                  key="timeline"
                  size="large"
                  icon={<LineChartOutlined />}
                  onClick={() =>
                    handleIrParaAcompanhamento(idsSalvos?.animalId)
                  }
                  block
                >
                  Ir para Linha do Tempo
                </Button>
              </Col>
              <Col span={24}>
                <Button
                  key="close"
                  type="text"
                  onClick={handleNextActionClose}
                  block
                >
                  Fechar e Concluir
                </Button>
              </Col>
            </Row>
          }
        />
      </Modal>

      {/* Modal Padrao: Criar Dieta na Tela de Consultas */}
      <ModalPadrao
        modo="criacao"
        open={isModalDietaOpen}
        onCancel={handleCloseModalDieta}
        title="Confirmar Cálculo da Dieta"
        icon={<CalculatorOutlined />}
        formId="form-dieta-animal"
        isSubmitting={isSubmittingDieta}
        width="min(700px, 95vw)"
      >
        <div className="mb-4 p-3 bg-blue-50 text-blue-800 rounded-md border border-blue-200 text-sm">
          <InfoCircleOutlined className="mr-2" />
          Os dados abaixo foram importados automaticamente da avaliação da
          consulta salva. Ajuste apenas se necessário.
        </div>

        {idsSalvos && (
          <FormularioDieta
            onFinish={handleSubmitDieta}
            initialValues={idsSalvos}
            racoes={racoesDisponiveis}
            dadosRaca={{
              nome: idsSalvos.nomeRaca,
              pesoMin: idsSalvos.pesoMinimoRaca,
              pesoMax: idsSalvos.pesoMaximoRaca,
            }}
          />
        )}
      </ModalPadrao>

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

export default ConsultasDaClinica;
