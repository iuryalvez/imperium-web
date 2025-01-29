import React, { useEffect, useState } from 'react';
import TemplateListaUsuarios from '../components/template/TemplateListaUsuarios';
import { usuario } from '../api/usuariosAPI';

const ListaUsuarios = () => {
  const [usuarios, setUsuarios] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUsuarios = async () => {
      try {
        const response = await usuario.listar();
        console.log('Usuários na Page: ', response);
        setUsuarios(response);
      } catch (err) {
        setError('Erro ao carregar os usuários.');
      } finally {
        setLoading(false);
      }
    };

    fetchUsuarios();
  }, []);

  if (loading) {
    return <p>Carregando usuários...</p>;
  }

  if (error) {
    return <p className="text-red-500">{error}</p>;
  }

  return <TemplateListaUsuarios usuarios={usuarios} />;
};

export default ListaUsuarios;
