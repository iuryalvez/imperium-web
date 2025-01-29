// import React, { useEffect, useState } from "react";
// import { useNavigate } from "react-router-dom";
// import { FaPlus } from "react-icons/fa";

import '../App.css';
import '../components/styles/Listas.css';
import Navsidebar from "../components/organisms/Navsidebar";
// import CardInfo from "../components/molecules/CardInfo";

function Animais() {
    
    // const navigate = useNavigate();

    // const [animais, setAnimais] = useState([]);

    // useEffect(() => {
    //     const fetchAnimais = async () => {
    //     try {
    //         const response = await api.get('http://localhost:5050/api/clinica/animais');
    //         setAnimais(response.data);
    //     } catch (error) {
    //         console.error('Erro ao buscar os dados dos animais:', error);
    //     }
    //     };

    //     fetchAnimais();
    // }, []);

    // const handleEdit = (animalId) => {
    //     // Lógica para editar os dados do animal com o ID fornecido
    //     console.log('Editar dados do animal com ID:', animalId);
    // };

    return (
        <div>    
            <Navsidebar></Navsidebar>
            <main className='main-content'>
                <div className="titulo-animais">
                    <h2>Lista de animais</h2>
                    <div>
                        {/* <button onClick={() => navigate('/animais/cadastrar')} className="button-bar">
                            <FaPlus className='icon-button-bar'/>
                            Cadastrar novo animal
                        </button> */}
                    </div>
                </div>
                {/* {animais.map(animal => (
                    <div key={animal.id}>
                        <CardInfo
                            id={animal.id}
                            info={animal.info}
                            onEdit={() => handleEdit(animal.id)}
                        />
                    </div>
                ))} */}
            </main>
        </div>
    )
}

export default Animais;