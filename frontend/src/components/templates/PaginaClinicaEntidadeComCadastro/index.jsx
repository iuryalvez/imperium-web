import React, { useCallback, useEffect, useState } from "react";
import { Modal, Button } from "antd";
import { PlusOutlined } from "@ant-design/icons";

import LoadingSpinner from "../../atoms/LoadingSpinner.jsx";
import TabelaEntidade from "../../organisms/TabelaEntidade.jsx";
import FormCadastroEntidade from "../../molecules/FormCadastroEntidade.jsx";
import { sanitizePayload, sanitizeToNumbers } from "../../../utils/sanitizers.js";

const PaginaClinicaEntidadeComCadastro = ({
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
  const [modalRacasVisivel, setModalRacasVisivel] = useState(false);

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
        <div className="flex justify-between items-center mb-4">
          <h1 className="text-2xl font-bold text-[var(--verde-medio)]">
            {titulo}
          </h1>
          <div className="flex gap-2">
            {entidade === "animaisDaClinica" && (
              <Button
                icon={<PlusOutlined />}
                onClick={() => setModalRacasVisivel(true)}
              >
                Gerenciar Raças
              </Button>
            )}

            <Button
              type="primary"
              icon={<PlusOutlined />}
              className="bg-[var(--verde-medio)] hover:bg-[var(--verde-relce)] text-white"
              onClick={() => setModalVisivel(true)}
            >
              {nomeBotao}
            </Button>
          </div>
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
        destroyOnClose // Boa prática: destrói os estados internos do form ao fechar
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

export default PaginaClinicaEntidadeComCadastro;
