import React from "react";
import Navsidebar from "./Navsidebar"; 
import '../layout/Animais.css';
import AnimalForm from "./AnimalForm";

function Animais() {
    return (
        <div>    
            <div>
                <Navsidebar />
            </div>
            <div className='animal-form'>
                <AnimalForm />
            </div>
        </div>
    )
}

export default Animais;