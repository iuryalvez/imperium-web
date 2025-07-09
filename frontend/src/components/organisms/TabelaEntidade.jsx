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
} from "@ant-design/icons";
import "../styles/TabelaEntidade.css";
import FormAcessarClinica from "../molecules/FormAcessarClinica";
import FormDetalhesEntidade from "../molecules/FormDetalhesEntidade";
import FormEdicaoEntidade from "../molecules/FormEdicaoEntidade";

const TabelaEntidade = ({
  entidade,
  nomeEntidade,
  dados,
  entidadeAPI,
  onAtualizar,
  colunasBase,
  camposSimplesEditar,
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

    const editarRegistro = async (valores) => {
    setBackupRegistros(registros);
    setRegistros((prev) =>
      prev.map((reg) =>
        (reg.id === valores.id && reg.upcId === valores.upcId) ? { ...reg, ...valores } : reg
      )
    );
    // O try/catch foi movido para o container do formulário, 
    // mas a chamada da API permanece aqui, o que está correto.
    await entidadeAPI.editar(valores); 
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
      render: (_, record) => (
        <Space>
          <Tooltip title="Ver Detalhes">
            <Button
              icon={<EyeOutlined />}
              onClick={() => abrirModalDetalhes(record)}
              style={{ backgroundColor: "var(--verde-medio)", color: "white" }}
            />
          </Tooltip>
          <Tooltip title="Editar">
            <Button
              icon={<EditOutlined />}
              onClick={() => abrirModalEdicao(record)}
              style={{ backgroundColor: "var(--verde-medio)", color: "white" }}
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
      ),
    },
  ];

  return (
    <div>
      <Table
        dataSource={registros}
        columns={colunasComAcoes}
        rowKey={(record) => record.id}
        pagination={{ pageSize: 10 }}
        scroll={{ x: "max-content" }} // ativa scroll horizontal se necessário
      />
      <div className="max-h-[80vh] overflow-y-auto">
        <Modal
          open={modalDetalhesVisivel}
          onCancel={() => setModalDetalhesVisivel(false)}
          footer={[
            <Button key="back" onClick={() => setModalDetalhesVisivel(false)}>
              Fechar
            </Button>,
          ]}
          centered
          destroyOnClose
          width={"70vw"}
          style={{ maxWidth: "800px", paddingTop: 24, paddingBottom: 24 }}
        >
          {registroSelecionado && (
            <FormDetalhesEntidade
              registro={registroSelecionado}
              entidade={entidade}
              nomeEntidade={nomeEntidade}
              valores={registroSelecionado}
              camposDetalhes={camposSimplesEditar}
            />
          )}
        </Modal>
        <Modal
          open={modalEdicaoVisivel}
          onCancel={() => setModalEdicaoVisivel(false)}
          footer={null}
          centered
          destroyOnClose
          width={700}
          className="modal-edicao"
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
              tituloModal={`Edição de ${nomeEntidade}`}
            />
          )}
        </Modal>
        <Modal
          title={`Acessar Clínica`}
          open={modalAcessoAberto}
          onCancel={fecharModalAcessoClinica}
          footer={null}
        >
          <FormAcessarClinica
            clinica={clinicaSelecionada}
            onSucesso={fecharModalAcessoClinica}
          />
        </Modal>
      </div>
    </div>
  );
};

export default TabelaEntidade;
