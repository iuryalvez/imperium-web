import { Tag, Typography } from "antd";
import { FaCat, FaDog } from 'react-icons/fa';

export const perfisSistema = [
  { label: "ADMIN", value: "ADMIN", cor: "gold" },
  { label: "GESTOR", value: "GESTOR", cor: "blue" },
  { label: "VETERINÁRIO", value: "VETERINARIO", cor: "green" },
  { label: "SECRETÁRIO", value: "SECRETARIO", cor: "green" },
  { label: "TÉCNICO", value: "TECNICO", cor: "green" },
  { label: "INATIVO", value: "INATIVO", cor: "red" },
];

export const getCorDoPerfil = (perfil) => {
  const encontrado = perfisSistema.find(p => p.value === perfil.toUpperCase());
  return encontrado ? encontrado.cor : "default";
};

/**
 * Renderiza um conjunto de perfis como Tags coloridas.
 * @param {string[]} perfis - Uma lista de strings de perfil.
 * @returns {React.Component}
 */
export const renderPerfis = (perfis = []) => {
    if (!perfis || perfis.length === 0) {
        return <Typography.Text type="secondary">Nenhum perfil atribuído</Typography.Text>;
    }
    return (
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: '4px' }}>
            {perfis.map(perfil => (
                <Tag key={perfil} color={getCorDoPerfil(perfil)}>
                    {perfil.toUpperCase()}
                </Tag>
            ))}
        </div>
    );
};

export const renderAnimais = (animais = []) => {
    if (!animais || animais.length === 0) {
        return <Typography.Text type="secondary">Nenhum animal associado</Typography.Text>;
    }
    return (
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
            {animais.map(animal => (
                <Tag
                    key={animal.id}
                    icon={animal.especie === 1 ? <FaDog /> : <FaCat />}
                    style={{ display: 'flex', alignItems: 'center', gap: '4px', fontSize: '14px' }}
                >
                    {animal.nome}
                </Tag>
            ))}
        </div>
    );
};
