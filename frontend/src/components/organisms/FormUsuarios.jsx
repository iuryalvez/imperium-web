import React, { useState } from 'react';
import axios from 'axios';

const FormUsuario = ({ usuario, onSubmitSuccess, closeModal }) => {
  const [nome, setNome] = useState(usuario?.nome || '');
  const [email, setEmail] = useState(usuario?.email || '');
  const [perfil, setPerfil] = useState(usuario?.perfil || 'GESTOR');
  const [senha, setSenha] = useState(''); // Apenas para criação

  const handleSubmit = async (e) => {
    e.preventDefault();

    const endpoint = usuario
      ? `http://localhost:5050/api/clinica/usuarios/usuario_${usuario.id}/editar`
      : `http://localhost:5050/api/clinica/usuarios/criar-usuario`;

    const method = usuario ? 'put' : 'post';

    try {
      await axios[method](
        endpoint,
        { nome, email, perfil, senha: usuario ? undefined : senha }, // Só envia senha na criação
        { withCredentials: true }
      );
      alert('Usuário salvo com sucesso!');
      onSubmitSuccess(); // Atualiza a lista de usuários
      closeModal(); // Fecha o modal
    } catch (error) {
      console.error('Erro ao salvar usuário:', error);
      alert('Erro ao salvar usuário. Verifique os dados.');
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label>Nome:</label>
        <input value={nome} onChange={(e) => setNome(e.target.value)} required />
      </div>
      <div>
        <label>Email:</label>
        <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
      </div>
      <div>
        <label>Perfil:</label>
        <select value={perfil} onChange={(e) => setPerfil(e.target.value)}>
          <option value="ADMINISTRADOR">Administrador</option>
          <option value="GESTOR">Gestor</option>
          <option value="SECRETARIO">Secretário</option>
          <option value="VETERINARIO">Veterinário</option>
          <option value="VISUALIZADOR">Veterinário</option>
          <option value="PENDENTE">Veterinário</option>
          <option value="INATIVO">Veterinário</option>
        </select>
      </div>
      {!usuario && ( // Apenas na criação de um novo usuário
        <div>
          <label>Senha:</label>
          <input
            type="password"
            value={senha}
            onChange={(e) => setSenha(e.target.value)}
            required
          />
        </div>
      )}
      <button type="submit">Salvar</button>
    </form>
  );
};

export default FormUsuario;
