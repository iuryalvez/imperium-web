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
          className="flex items-center font-bold text-green-dark mb-2"
        >
          <ArrowLeftOutlined className="mr-2" /> Voltar
        </button> */}
        <div className="flex justify-between items-center mb-4">
          <h1 className="text-2xl font-bold text-green-dark">{titulo}</h1>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            className="bg-green-700 hover:bg-green-800 text-white focus:bg-green-300 active:bg-green-300 focus:shadow-none border-none"
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
