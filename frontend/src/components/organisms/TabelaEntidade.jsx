import { useState, useEffect } from "react";
import {
  Table,
  Button,
  Space,
  Modal,
  Popconfirm,
  message,
  Tooltip,
} from "antd";
import {
  DeleteOutlined,
  EditOutlined,
  EyeOutlined,
  LoginOutlined,
  StopOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import "../styles/TabelaEntidade.css";
import FormAcessarClinica from "../molecules/FormAcessarClinica";
import FormDetalhesEntidade from "../molecules/DetalhesEntidade";
import FormEdicaoEntidade from "../molecules/FormEdicaoEntidade";

const TabelaEntidade = ({
  entidade,
  nomeEntidade,
  dados,
  entidadeAPI,
  onAtualizar,
  colunasBase,
}) => {
  const [registros, setRegistros] = useState(dados); // armazena os dados atuais da tabela, set atualiza
  const [backupRegistros, setBackupRegistros] = useState([]); // guarda cópia de segurança antes de operações que alteram os registros
  const [modalDetalhesVisivel, setModalDetalhesVisivel] = useState(false);
  const [modalAcessoAberto, setModalAcessoAberto] = useState(false); // controla visualização do modal, inicialmente falsa
  const [modalEdicaoVisivel, setModalEdicaoVisivel] = useState(false); // controla visualização do modal, inicialmente falsa
  const [registroSelecionado, setRegistroSelecionado] = useState(null); // guarda registro selecionado, inicialmente null (nenhum)
  const [clinicaSelecionada, setClinicaSelecionada] = useState(null); // controla clinica selecionada, inicialmente null (nenhum) - no dto vem como clinicaId e não Id
  const [loading, setLoading] = useState(false); // controla o estado de loading, inicialmente falso

  useEffect(() => {
    setRegistros(dados);
  }, [dados]); // atualiza quando a entidade dados muda

  const handleSucessoEdicao = () => {
    setModalEdicaoVisivel(false); // Fecha o modal
    if (onAtualizar) {
      onAtualizar(); // Atualiza a tabela
    }
  };

  const sanitize = (value) => {
    if (!value) return ""; // Retorna string vazia se o valor for nulo/undefined
    return String(value).replace(/\D/g, "");
  };

  const editarRegistro = async (valores) => {
    setBackupRegistros(registros);
    setRegistros((prev) =>
      prev.map((reg) =>
        reg.id === valores.id && reg.upcId === valores.upcId
          ? { ...reg, ...valores }
          : reg
      )
    );
    let processedValues = { ...valores };

    processedValues = {
      ...processedValues,
      cpf: sanitize(valores.cpf),
      telefone1: sanitize(valores.telefone1),
      whatsapp: sanitize(valores.whatsapp),
      cep: sanitize(valores.cep),
      cnpj: sanitize(valores.cnpj),
    };

    await entidadeAPI.editar(processedValues);
  };

  const handleToggleAtivacao = async (record) => {
    const estaAtivo =
      entidade === "usuariosDoSistema"
        ? record.isAtivo
        : record.isAtivoNaClinica;
    const acao = estaAtivo ? "inativar" : "ativar";

    setLoading(true);
    try {
      // A chamada agora é simples. Passamos o registro inteiro e a ação desejada.
      // A camada de API (`entidadeAPI`) vai descobrir o que fazer com ele.
      await entidadeAPI.setStatus(record, !estaAtivo);

      message.success(`${nomeEntidade} alterado com sucesso.`);
      if (onAtualizar) onAtualizar();
    } catch (error) {
      console.error(`Erro ao ${acao} usuário:`, error);
      message.error(
        error.response?.data?.message || `Falha ao ${acao} usuário.`
      );
    } finally {
      setLoading(false);
    }
  };

  const deletarRegistro = async (registroId) => {
    console.log("Iniciando deleção de registro com Id:", registroId);
    setLoading(true);
    try {
      // salva estado atual
      setBackupRegistros(registros);

      // atualização otimista: deleta no frontend
      setRegistros((prev) => prev.filter((reg) => reg.id !== registroId));

      await entidadeAPI.deletar(registroId);

      message.success(`${nomeEntidade} deletado com sucesso.`);

      if (onAtualizar) onAtualizar();
    } catch (error) {
      setRegistros(backupRegistros); // rollback
      console.error("Erro ao deletar registro:", error);
      message.error(
        error.response?.data?.message ||
          `Falha ao deletar ${nomeEntidade.toLowerCase()}`
      );
    } finally {
      setLoading(false);
    }
  };

  const abrirModalDetalhes = (registro) => {
    setRegistroSelecionado(registro);
    setModalDetalhesVisivel(true);
  };

  const abrirModalEdicao = (registro) => {
    setRegistroSelecionado(registro);
    setModalEdicaoVisivel(true);
  };

  const abrirModalAcessoClinica = (clinica) => {
    setClinicaSelecionada(clinica);
    setModalAcessoAberto(true);
  };

  const fecharModalAcessoClinica = () => {
    setModalAcessoAberto(false);
    setClinicaSelecionada(null);
  };

  const colunasComAcoes = [
    ...(colunasBase || []),
    {
      title: "Ações",
      key: "acoes",
      width: "auto",
      render: (_, record) => {
        // Determina o status com base no tipo de entidade
        const estaAtivo =
          entidade === "usuariosDoSistema"
            ? record.isAtivo
            : record.isAtivoNaClinica;

        return (
          <Space>
            <Tooltip title="Ver Detalhes">
              <Button
                icon={<EyeOutlined />}
                onClick={() => abrirModalDetalhes(record)}
                style={{
                  backgroundColor: "var(--verde-medio)",
                  color: "white",
                }}
              />
            </Tooltip>
            <Tooltip title="Editar">
              <Button
                icon={<EditOutlined />}
                onClick={() => abrirModalEdicao(record)}
                style={{
                  backgroundColor: "var(--verde-medio)",
                  color: "white",
                }}
              />
            </Tooltip>
            {entidade === "clinicas" && (
              <Tooltip title="Acessar Clínica">
                <Button
                  icon={<LoginOutlined />}
                  onClick={() => abrirModalAcessoClinica(record)}
                  style={{
                    backgroundColor: "var(--verde-medio)",
                    color: "white",
                  }}
                >
                  Acessar
                </Button>
              </Tooltip>
            )}

            {["usuariosDaClinica", "usuariosDoSistema"].includes(entidade) && (
              <Tooltip title={estaAtivo ? "Inativar" : "Ativar"}>
                <Popconfirm
                  title={`Tem certeza que deseja ${
                    estaAtivo ? "inativar" : "ativar"
                  } este usuário?`}
                  onConfirm={() => handleToggleAtivacao(record)}
                  okText="Sim"
                  cancelText="Não"
                >
                  <Button
                    icon={
                      estaAtivo ? <StopOutlined /> : <CheckCircleOutlined />
                    }
                    style={{
                      backgroundColor: estaAtivo
                        ? "var(--vermelho-delete)"
                        : "var(--verde-medio)",
                      color: "white",
                    }}
                  />
                </Popconfirm>
              </Tooltip>
            )}

            {!["usuariosDaClinica", "usuariosDoSistema"].includes(entidade) && (
              <Tooltip title="Deletar">
                <Popconfirm
                  title="Tem certeza que deseja deletar este registro?"
                  onConfirm={() => deletarRegistro(record.id)}
                  okText="Sim"
                  cancelText="Não"
                >
                  <Button
                    icon={<DeleteOutlined />}
                    style={{
                      backgroundColor: "var(--vermelho-delete)",
                      color: "white",
                    }}
                  />
                </Popconfirm>
              </Tooltip>
            )}
          </Space>
        );
      },
    },
  ];

  const getRowKey = (record) => {
    if (entidade === "usuariosDoSistema") {
      return record.compositeKey;
    }
    // Para outras entidades, o Id principal é suficiente.
    return record.id;
  };

  return (
    <div>
      <Table
        dataSource={registros}
        columns={colunasComAcoes}
        rowKey={getRowKey}
        pagination={{ pageSize: 10 }}
        scroll={{ x: "max-content" }} // ativa scroll horizontal se necessário
      />

      <Modal
        title={`Informações - ${nomeEntidade}`}
        open={modalDetalhesVisivel}
        onCancel={() => setModalDetalhesVisivel(false)}
        footer={null}
        centered
        destroyOnClose
        style={{ minWidth: "40vw", maxWidth: "80vw" }}
        styles={{
          body: { maxHeight: "80vh", overflowY: "auto" },
        }}
      >
        {registroSelecionado && (
          <FormDetalhesEntidade
            entidade={entidade}
            valores={registroSelecionado}
            onCancel={() => setModalDetalhesVisivel(false)}
          />
        )}
      </Modal>
      <Modal
        title={`Edição - ${nomeEntidade}`}
        open={modalEdicaoVisivel}
        onCancel={() => setModalEdicaoVisivel(false)}
        footer={null}
        centered
        destroyOnClose
        style={{ minWidth: "40vw", maxWidth: "80vw" }}
        styles={{
          body: { maxHeight: "80vh", overflowY: "auto" },
        }}
      >
        {/* Garante que o formulário só renderize quando tiver dados */}
        {registroSelecionado && (
          // 3. Chamada ao componente com a sintaxe correta e as props bem definidas
          <FormEdicaoEntidade
            entidade={entidade}
            valoresIniciais={registroSelecionado}
            onSubmit={editarRegistro} // Passa a função de edição
            onSuccess={handleSucessoEdicao} // Passa a nova função de sucesso
            onCancel={() => setModalEdicaoVisivel(false)}
          />
        )}
      </Modal>
      <Modal
        title={`Acessar Clínica`}
        open={modalAcessoAberto}
        onCancel={fecharModalAcessoClinica}
        footer={null}
        style={{ minWidth: "40vw", maxWidth: "80vw" }} // importante  
        styles={{
          body: { maxHeight: "80vh", overflowY: "auto" }, // importante
        }}
      >
        <FormAcessarClinica
          clinica={clinicaSelecionada}
          onSucesso={fecharModalAcessoClinica}
        />
      </Modal>
    </div>
  );
};

export default TabelaEntidade;
