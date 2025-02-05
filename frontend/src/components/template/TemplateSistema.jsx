import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext'; 
import BotaoMenu from '../atoms/BotaoMenu';
import BotaoOff from '../atoms/BotaoOff';

const TemplateSistema = () => {
    const navigate = useNavigate();
    const { userName } = useAuth(); 
    
    const botoesGerais = [
        { texto: 'Vínculos', rota: '/sistema/vinculos', ativo: true },
        { texto: 'Clínicas', rota: '/sistema/clinicas', ativo: true },
        { texto: 'Usuários', rota: '/sistema/usuarios', ativo: true },
        { texto: 'Animais', rota: '/sistema/animais', ativo: false },
        { texto: 'Perfis', rota: '/sistema/perfis', ativo: false }
    ];

    const renderBotoes = () => {
        let botoes = [...botoesGerais];

        return botoes.map(({ texto, rota, ativo }) =>
            ativo ? (
                <BotaoMenu key={rota} texto={texto} onClick={() => navigate(rota)} />
            ) : (
                <BotaoOff key={rota} texto={texto} onClick={() => navigate(rota)} />
            )
        );
    };

    return (
        <div className="sistema-content">
            <h1 className="text-2xl font-bold mb-4 text-green-dark">
                Bem-vindo, {userName || 'Usuário'}!
            </h1>
            <div className="flex flex-wrap justify-center gap-10 pt-8">
                {renderBotoes()}
            </div>
        </div>
    );
};

export default TemplateSistema;
