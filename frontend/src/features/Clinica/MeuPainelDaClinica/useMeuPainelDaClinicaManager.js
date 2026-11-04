import { useCallback, useEffect, useState } from "react";
import { message } from "antd";
import { useNavigate } from "react-router-dom";
import { meuPainelDaClinicaAPI } from "../../../api/clinica/meuPainelDaClinicaAPI";
import { consultasDaClinicaAPI } from "../../../api/clinica/consultasDaClinicaAPI";
import { usePaginacaoServidor } from "../../../utils/usePaginacaoServidor";
import { useAuth } from "../../../services/AuthContext";
import { usePermissao } from "../../../utils/usePermissao";

// Lista vazia para seções que o perfil não vê (o hook de paginação é chamado sempre, sem chamar a API)
const listaVazia = async () => ({ itens: [], total: 0 });

/**
 * Meu Painel: início da clínica para o usuário logado, com uma seção por perfil.
 * - Todos: meus dados.
 * - SECRETARIO: consultas que marquei.
 * - VETERINARIO: consultas que atendi (avaliação preenchida por mim) e consultas pendentes da clínica.
 * - GESTOR: atalho para o dashboard da clínica.
 */
export const useMeuPainelDaClinicaManager = () => {
  const navigate = useNavigate();
  const { perfis } = useAuth();
  const pode = usePermissao();
  const [painel, setPainel] = useState(null);
  const [loading, setLoading] = useState(true);

  const ehSecretario = (perfis || []).includes("SECRETARIO");
  const ehVeterinario = (perfis || []).includes("VETERINARIO");

  const listarMarcadas = useCallback(
    (filtro) => (ehSecretario ? consultasDaClinicaAPI.listar({ ...filtro, marcadasPorMim: true }) : listaVazia()),
    [ehSecretario],
  );
  const listarAtendidas = useCallback(
    (filtro) => (ehVeterinario ? consultasDaClinicaAPI.listar({ ...filtro, atendidasPorMim: true }) : listaVazia()),
    [ehVeterinario],
  );
  const listarPendentes = useCallback(
    (filtro) => (ehVeterinario ? consultasDaClinicaAPI.listar({ ...filtro, status: "Pendente", ordem: "asc" }) : listaVazia()),
    [ehVeterinario],
  );

  const consultasMarcadas = usePaginacaoServidor(listarMarcadas, { tamanhoPagina: 5 });
  const consultasAtendidas = usePaginacaoServidor(listarAtendidas, { tamanhoPagina: 5 });
  const consultasPendentes = usePaginacaoServidor(listarPendentes, { tamanhoPagina: 5 });

  useEffect(() => {
    const carregar = async () => {
      setLoading(true);
      try {
        setPainel(await meuPainelDaClinicaAPI.obter());
      } catch (error) {
        message.error(error.message || "Erro ao carregar o painel.");
      } finally {
        setLoading(false);
      }
    };
    carregar();
  }, []);

  const handleIrParaAcompanhamento = (acompanhamentoId) => {
    if (acompanhamentoId) navigate(`/clinica/animais/acompanhamento/${acompanhamentoId}`);
  };

  return {
    painel,
    loading,
    ehSecretario,
    ehVeterinario,
    podeVerDashboard: pode("gerenciaClinica"),
    consultasMarcadas,
    consultasAtendidas,
    consultasPendentes,
    handleIrParaAcompanhamento,
    handleIrParaDashboard: () => navigate("/clinica/editar"),
  };
};
