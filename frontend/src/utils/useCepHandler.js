import { useState } from "react";
import { message } from "antd";
import { viaCepService } from "../services/viaCepService";

export const useCepHandler = (form) => {
  const [loadingCep, setLoadingCep] = useState(false);

  const handleCepSearch = async (cepValue) => {
    if (!cepValue) return;

    const cleanCep = cepValue.replace(/\D/g, "");

    if (cleanCep.length === 8) {
      setLoadingCep(true);
      try {
        const dadosEndereco = await viaCepService.buscarCep(cleanCep);

        form.setFieldsValue({
          logradouro: dadosEndereco.logradouro,
          bairro: dadosEndereco.bairro,
          cidade: dadosEndereco.localidade,
          uf: dadosEndereco.uf,
        });

        message.success("Endereço preenchido automaticamente.");
      } catch (error) {
        message.warning(error.message || "Erro ao buscar o CEP.");
      } finally {
        setLoadingCep(false);
      }
    }
  };

  return { handleCepSearch, loadingCep };
};
