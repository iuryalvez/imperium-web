import React from "react";
import { Skeleton, Result, Button, Popconfirm, Tooltip } from "antd";
import { Link } from "react-router-dom";
import {
  FormOutlined,
  EditOutlined,
  ProfileOutlined,
  FileTextOutlined,
  DeleteOutlined,
} from "@ant-design/icons";
import { useAcompanhamentoManager } from "./useAcompanhamentoManager";
import { usePermissao } from "../../../../utils/usePermissao";
import ListaConsultasCards from "./components/ListaConsultasCards";
import FormularioDieta from "./components/Dieta/FormularioDieta";
import DetalhesDieta from "./components/Dieta/DetalhesDieta";
import FichaGeralAnimal from "./components/FichaGeralAnimal";
import ModalPadrao from "../../../../components/modal/ModalPadrao";
import PaginaPadrao from "../../../../components/layout/PaginaPadrao";
import FormularioAvaliacao from "./components/Avaliacao/FormularioAvaliacao";
import FormularioConsulta from "../../ConsultasDaClinica/components/FormularioConsulta";
import FormEdicaoAnimal from "../components/FormularioEdicaoAnimaisDaClinica/FormEdicaoAnimais";

const Acompanhamento = () => {
  const {
    historico,
    loading,
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
    handleIniciarDieta,
    handleConcluirDieta,
    handleRegistrarPeso,
    handleRemoverPeso,
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

  // O TECNICO (e o SECRETARIO) veem o acompanhamento; editar animal, avaliação, dieta e consulta depende do perfil
  const pode = usePermissao();

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
  const dietaMaisRecente = consultas.find((c) => c.avaliacao?.dieta)?.avaliacao
    .dieta;

  const isGato = historico?.especie?.toLowerCase() === "gato";
  const isFemea =
    historico?.sexo?.toLowerCase() === "fêmea" ||
    historico?.sexo?.toLowerCase() === "femea";

  const especieInt = isGato ? 2 : 1;
  const sexoInt = isFemea ? 2 : 1;

  return (
    <PaginaPadrao
      preencher={false}
      trilha={[{ titulo: "Animais", para: "/clinica/animais" }]}
      titulo={`Acompanhamento de ${historico.nome || "animal"}`}
    >
      {/* >= lg: ficha numa coluna lateral fixa (resumo sempre à vista) e consultas na principal.
          Abaixo de lg: uma coluna só, ficha em cima. */}
      <div className="grid grid-cols-1 lg:grid-cols-[minmax(300px,380px)_minmax(0,1fr)] gap-6 items-start">
        <div className="lg:sticky lg:top-0">
          <FichaGeralAnimal
            data={historico}
            avaliacaoRecente={avaliacaoMaisRecente}
            dietaRecente={dietaMaisRecente}
            onOpenDieta={handleOpenModalVisualizarDieta}
            onEditAnimal={pode("gerenciaAnimais") ? handleOpenEditAnimalModal : undefined}
          />
        </div>

        <section className="flex flex-col gap-4 min-w-0">
          <h2 className="m-0 text-xl font-semibold text-verde-escuro">Consultas</h2>
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
            onIniciarDieta={handleIniciarDieta}
            onConcluirDieta={handleConcluirDieta}
            podeGerenciarConsultas={pode("gerenciaConsultas")}
            podeGerenciarAcompanhamento={pode("gerenciaAcompanhamento")}
          />
        </section>
      </div>

      {/* Modal de Edição da Consulta */}
      <ModalPadrao
        modo="edicao"
        open={isConsultaModalOpen}
        onCancel={handleCloseConsultaModal}
        title="Editar Consulta"
        icon={<EditOutlined />}
        formId="form-cadastro-edicao-consulta"
        isSubmitting={isSubmitting}
        tamanho="sm"
      >
        <FormularioConsulta
          initialValues={{
            dataConsulta: consultaToEdit?.dataConsulta,
            animalId: historico?.id,
            // O histórico manda tutorId 0 quando a consulta não tem tutor: vira vazio no select
            tutorId: consultaToEdit?.tutorId || undefined,
          }}
          onFinish={handleSubmitConsulta}
        />
      </ModalPadrao>

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
      <ModalPadrao
        open={isAvaliacaoModalOpen}
        onCancel={handleCloseAvaliacaoModal}
        title={`Avaliação da Consulta - ${selectedConsulta?.nomeAnimal || historico?.nome}`}
        icon={<FileTextOutlined />}
        tamanho="md"
        rodape={
          <>
            <Button onClick={handleCloseAvaliacaoModal}>Cancelar</Button>

            {avaliacaoData && (
              <Tooltip
                title={
                  !!avaliacaoData.dieta
                    ? "Não é possível excluir uma avaliação com dieta preenchida"
                    : ""
                }
              >
                <div>
                  <Popconfirm
                    title="Excluir relatório"
                    description="Tem certeza que deseja excluir este relatório? Esta ação não pode ser desfeita."
                    onConfirm={() => handleDeleteAvaliacao(avaliacaoData.id)}
                    okText="Sim, Excluir"
                    cancelText="Cancelar"
                    disabled={!!avaliacaoData.dieta}
                  >
                    <Button
                      icon={<DeleteOutlined />}
                      danger
                      disabled={!!avaliacaoData.dieta}
                    >
                      Excluir
                    </Button>
                  </Popconfirm>
                </div>
              </Tooltip>
            )}

            <Button
              form="form-avaliacao-consulta-clinica"
              type="primary"
              htmlType="submit"
            >
              Salvar Avaliação
            </Button>
          </>
        }
      >
        {loadingAvaliacao ? (
          <Skeleton active />
        ) : (
          <FormularioAvaliacao
            data={{
              ...selectedConsulta,
              especieAnimal: especieInt,
              sexoAnimal: sexoInt,
              faseVidaAtual: historico.faseVidaAtual,
            }}
            initialValues={avaliacaoData}
            onFinish={handleSubmitAvaliacao}
          />
        )}
      </ModalPadrao>

      {/* Modal Padrao: Criar Dieta */}
      <ModalPadrao
        modo="criacao"
        open={isModalCriarDietaOpen}
        onCancel={handleCloseModalCriarDieta}
        title={`Dieta - Paciente: ${historico?.nome || "Desconhecido"}`}
        icon={<FormOutlined />}
        formId="form-dieta-animal"
        isSubmitting={isSubmitting}
        tamanho="md"
      >
        {avaliacaoSelecionado && (
          <FormularioDieta
            initialValues={{
              ...avaliacaoSelecionado,
              especieAnimal: especieInt,
            }}
            onFinish={handleCriarDieta}
            avaliacaoId={avaliacaoSelecionado.id}
            dadosRaca={historico?.raca}
            dadosAnimal={historico}
          />
        )}
      </ModalPadrao>

      {/* Modal Padrao: Editar Dieta */}
      <ModalPadrao
        modo="edicao"
        open={isModalEditarDietaOpen}
        onCancel={handleCloseModalEditarDieta}
        title={`Editar Dieta - Paciente: ${historico?.nome || "Desconhecido"}`}
        icon={<EditOutlined />}
        formId="form-dieta-animal"
        isSubmitting={isSubmitting}
        tamanho="md"
      >
        {dietaParaEditar && avaliacaoSelecionado && (
          <FormularioDieta
            onFinish={handleEditarDieta}
            initialValues={{
              ...avaliacaoSelecionado,
              ...dietaParaEditar,
              especieAnimal: especieInt,
            }}
            avaliacaoId={dietaParaEditar.avaliacaoId || avaliacaoSelecionado.id}
            dadosRaca={historico?.raca}
            dadosAnimal={historico}
          />
        )}
      </ModalPadrao>

      {/* Modal Padrao: Visualizar Dieta (Detalhes) */}
      <ModalPadrao
        modo="detalhes"
        open={isModalVisualizarDietaOpen}
        onCancel={handleCloseModalVisualizarDieta}
        title="Detalhes da Dieta Calculada"
        subtitle={
          dietaParaVisualizar
            ? `${dietaParaVisualizar.statusLabel} ${
                dietaParaVisualizar.dataInicio
                  ? `- Início em ${new Date(dietaParaVisualizar.dataInicio + "T00:00:00").toLocaleDateString()}` 
                  : ""
              }${
                dietaParaVisualizar.dataConclusao
                  ? `- Conclusão em ${new Date(dietaParaVisualizar.dataConclusao + "T00:00:00").toLocaleDateString()}`
                  : ""
              }`
            : ""
        }
        icon={<ProfileOutlined />}
        tamanho="lg"
      >
        {dietaParaVisualizar && <DetalhesDieta dieta={dietaParaVisualizar} isGato={isGato} onRegistrarPeso={pode("gerenciaAcompanhamento") ? handleRegistrarPeso : undefined} onRemoverPeso={pode("gerenciaAcompanhamento") ? handleRemoverPeso : undefined} />}
      </ModalPadrao>
    </PaginaPadrao>
  );
};

export default Acompanhamento;
