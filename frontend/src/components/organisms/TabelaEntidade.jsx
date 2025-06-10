import { useState, useEffect } from "react";
import { Table, Button, Space, Modal, Popconfirm, message } from "antd";
import { DeleteOutlined, EditOutlined, LoginOutlined } from "@ant-design/icons";
import "../styles/TabelaEntidade.css";
import FormEditarEntidade from "../molecules/FormEditarEntidade";
import FormAcessarClinica from "../molecules/FormAcessarClinica";

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
  const [modalEdicaoVisivel, setModalEdicaoVisivel] = useState(false); // controla visualização do modal, inicialmente falsa
  const [registroSelecionado, setRegistroSelecionado] = useState(null); // guarda registro selecionado, inicialmente null (nenhum)
  const [modalAcessoAberto, setModalAcessoAberto] = useState(false); // controla visualização do modal, inicialmente falsa
  const [clinicaSelecionada, setClinicaSelecionada] = useState(null); // controla clinica selecionada, inicialmente null (nenhum) - no dto vem como clinicaId e não Id
  const [loading, setLoading] = useState(false); // controla o estado de loading, inicialmente falso

  useEffect(() => {
    setRegistros(dados);
  }, [dados]); // atualiza quando a entidade dados muda

  // edição otimista com rollback em caso de erro
  const editarRegistro = async (valores) => {
    setLoading(true);
    try {
      // salva estado atual para possível rollback
      setBackupRegistros(registros);

      // atualização otimista: já atualiza localmente antes de confirmar com o backend (?)
      setRegistros((prev) =>
        prev.map((reg) =>
          reg.id === valores.id ? { ...reg, ...valores } : reg
        )
      );

      await entidadeAPI.editar(valores);

      setModalEdicaoVisivel(false);
      message.success(`${nomeEntidade} atualizado com sucesso!`);
      // callback do componente pai
      if (onAtualizar) onAtualizar();
    } catch (erro) {
      // rollback em caso de erro
      setRegistros(backupRegistros);
      console.error("Erro ao editar registro:", erro);
      message.error(
        erro.response?.data?.message ||
          `Falha ao atualizar ${nomeEntidade.toLowerCase()}`
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
          <Button
            icon={<EditOutlined />}
            onClick={() => abrirModalEdicao(record)}
            style={{ backgroundColor: "var(--verde-medio)", color: "white" }}
          />
          {entidade === "clinicas" && (
            <Button
              icon={<LoginOutlined />}
              onClick={() => abrirModalAcessoClinica(record)}
              style={{ backgroundColor: "var(--verde-medio)", color: "white" }}
            >
              Acessar
            </Button>
          )}
          {!["usuariosDaClinica", "usuariosDoSistema"].includes(entidade) && (
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
          open={modalEdicaoVisivel}
          onCancel={() => setModalEdicaoVisivel(false)}
          footer={null}
          centered
          destroyOnClose
          style={{ paddingTop: 24, paddingBottom: 24 }}
          className="modal-edicao"
        >
          <FormEditarEntidade
            entidade={entidade}
            onSubmit={(valores) => editarRegistro(valores)}
            onSuccess={onAtualizar}
            setModalEdicaoVisivel={setModalEdicaoVisivel}
            tituloModal={`Edição de ${nomeEntidade}`}
            valoresIniciais={registroSelecionado}
            camposSimplesEditar={camposSimplesEditar}
          />
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
