import React, { useCallback, useEffect, useState } from "react";
import { Modal, Button } from "antd";
import { PlusOutlined } from "@ant-design/icons";

import LoadingSpinner from "../../atoms/LoadingSpinner.jsx";
import TabelaEntidade from "../../organisms/TabelaEntidade.jsx";
import FormCadastroEntidade from "../../molecules/FormCadastroEntidade.jsx";

const PaginaClinicaEntidadeComCadastro = ({
  titulo,
  entidade,
  nomeEntidade,
  api,
  nomeBotao,
  colunasBase,
  camposSimplesEditar, // A prop de campos de cadastro foi removida
}) => {
  const [dados, setDados] = useState([]);
  const [loading, setLoading] = useState(true);
  const [modalVisivel, setModalVisivel] = useState(false);

  const buscarDados = useCallback(async () => {
    try {
      setLoading(true);
      const response = await api.listar();
      setDados(response);
    } catch (err) {
      console.error(`Erro ao buscar ${entidade}: `, err);
    } finally {
      setLoading(false);
    }
  }, [api, entidade]);

  useEffect(() => {
    buscarDados();
  }, [buscarDados]);

  const handleCadastrar = async (formData) => {
    // Esta função permanece a mesma, ela é a ponte com a API
    return await api.cadastrar(formData);
  };

  const handleSucesso = () => {
    // Esta também permanece a mesma, define o que acontece após o sucesso
    setModalVisivel(false);
    buscarDados();
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div>
        <div className="flex justify-between items-center mb-4">
          <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
            {titulo}
          </h1>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            className="bg-[var(--verde-medio)] hover:bg-[var(--verde-relce)] text-white"
            onClick={() => setModalVisivel(true)}
          >
            {nomeBotao}
          </Button>
        </div>

        <TabelaEntidade
          entidade={entidade}
          nomeEntidade={nomeEntidade}
          entidadeAPI={api}
          dados={dados}
          onAtualizar={buscarDados}
          colunasBase={colunasBase}
          camposSimplesEditar={camposSimplesEditar}
        />
      </div>
      
      {/* O Modal continua aqui, agindo como a "casca" */}
      <Modal
        open={modalVisivel}
        onCancel={() => setModalVisivel(false)}
        footer={null}
        centered
        width={600}
        destroyOnClose // Boa prática: destrói os estados internos do form ao fechar
        className="modal-detalhes-entidade" // Classe para estilização responsiva
      >
        {/* 2. A chamada ao formulário agora está com a sintaxe correta e as props necessárias */}
        <FormCadastroEntidade
          entidade={entidade}
          onSubmit={handleCadastrar}
          onSuccess={handleSucesso}
          onCancel={() => setModalVisivel(false)} // Passa a função para fechar o modal
          tituloModal={`Cadastro de ${nomeEntidade}`}
        />
      </Modal>
    </div>
  );
};

export default PaginaClinicaEntidadeComCadastro;