import React, { useCallback, useEffect, useState } from "react";
import { Modal, Button, message } from "antd";
import { ArrowLeftOutlined, PlusOutlined } from "@ant-design/icons";

import LoadingSpinner from "../../atoms/LoadingSpinner.jsx";
import TabelaEntidade from "../../organisms/TabelaEntidade.jsx";
import FormCadastroEntidade from "../../molecules/FormCadastroEntidade.jsx";
import { sanitizePayload, sanitizeToNumbers } from "../../../utils/sanitizers.js";

const PaginaSistemaEntidadeComCadastro = ({
  titulo,
  entidade,
  nomeEntidade,
  api,
  nomeBotao,
  colunasBase,
}) => {
  const [dados, setDados] = useState([]);
  const [loading, setLoading] = useState(true);
  const [modalVisivel, setModalVisivel] = useState(false);

  // A lógica de busca e estado está correta e permanece a mesma
  const buscarDados = useCallback(async () => {
    try {
      setLoading(true);
      const response = await api.listar();
      setDados(response);
    } catch (err) {
      console.error(`Erro ao buscar ${entidade}: `, err);
      message.error(`Falha ao carregar ${nomeEntidade}`);
    } finally {
      setLoading(false);
    }
  }, [api, entidade, nomeEntidade]);

  useEffect(() => {
    buscarDados();
  }, [buscarDados]);

  const handleCadastrar = async (formData) => {
    let formValues = { ...formData };

    formValues = sanitizePayload({
      ...formValues,
      cnpj: sanitizeToNumbers(formValues.cnpj),
      cep: sanitizeToNumbers(formValues.cep),
      telefone: sanitizeToNumbers(formValues.telefone),
      whatsapp: sanitizeToNumbers(formValues.whatsapp),
    });

    return await api.cadastrar(formValues);
  };

  const handleSucesso = () => {
    setModalVisivel(false);
    buscarDados();
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div>
        <button
          onClick={() => window.history.back()}
          className="flex items-center font-bold text-[var(--verde-medio)] mb-2"
        >
          <ArrowLeftOutlined className="mr-2" /> Voltar
        </button>
        <div className="flex justify-between items-center mb-4">
          <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
            {titulo}
          </h1>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            className="bg-[var(--verde-medio)] hover:bg-[var(--verde-realce)] text-white"
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
        />
      </div>

      <Modal
        title={`Cadastro - ${nomeEntidade}`}
        open={modalVisivel}
        onCancel={() => setModalVisivel(false)}
        footer={null}
        centered
        destroyOnClose
        style={{ minWidth: "40vw", maxWidth: "80vw" }}
        styles={{
          body: {
            maxHeight: "80vh",
            overflowY: "auto",
            overflowX: "hidden",
            padding: "1px",
          },
        }}
      >
        <FormCadastroEntidade
          entidade={entidade}
          onSubmit={handleCadastrar}
          onSuccess={handleSucesso}
          onCancel={() => setModalVisivel(false)}
        />
      </Modal>
    </div>
  );
};

export default PaginaSistemaEntidadeComCadastro;
