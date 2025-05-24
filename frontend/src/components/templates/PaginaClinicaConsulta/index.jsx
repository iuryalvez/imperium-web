import React, { useCallback, useEffect, useState } from "react";
import { Modal, Button } from "antd";
import { PlusOutlined } from "@ant-design/icons";

import LoadingSpinner from "../../atoms/LoadingSpinner.jsx";
import FormCadastrarEntidade from "../../molecules/FormCadastrarEntidade.jsx";
import TabelaEntidade from "../../organisms/TabelaEntidade.jsx";

const camposSimplesCadastrar = {
  
};

const PaginaClinicaConsulta = ({
  titulo,
  entidade,
  nomeEntidade,
  api,
  nomeBotao,
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
    return await api.cadastrar(formData);
  };

  const handleSucesso = () => {
    setModalVisivel(false);
    buscarDados();
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div>
        {/* <button
          onClick={() => window.history.back()}
          className="flex items-center font-bold text-[var(--verde-medio)] mb-2"
        >
          <ArrowLeftOutlined className="mr-2" /> Voltar
        </button> */}
        <div className="flex justify-between items-center mb-4">
          <h1 className="text-2xl font-bold text-[var(--verde-medio)]">{titulo}</h1>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            className="bg-[var(--verde-medio)] hover:bg-[var(--verde-realce)] text-white"
            onClick={() => setModalVisivel(true)}
          >
            {nomeBotao}
          </Button>
        </div>

        <TabelaEntidade entidade={entidade} nomeEntidade={nomeEntidade} entidadeAPI={api} dados={dados} onAtualizar={buscarDados}/>
      </div>
      <div className="max-h-[80vh] overflow-y-auto">
        <Modal
          open={modalVisivel}
          onCancel={() => setModalVisivel(false)}
          footer={null}
          centered
          width={600}
          style={{ paddingTop: 24, paddingBottom: 24 }}
        >
          <FormCadastrarEntidade
            entidade={entidade}
            onSubmit={handleCadastrar}
            onSuccess={handleSucesso}
            setModalVisivel={setModalVisivel}
            tituloModal={`Cadastro de ${nomeEntidade}`}
            camposSimplesCadastrar={camposSimplesCadastrar}
          />
        </Modal>
      </div>
    </div>
  );
};

export default PaginaClinicaConsulta;
