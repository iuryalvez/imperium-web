import React from "react";

import logoUfg from "../assets/UFG_logo.png";
import logoEvz from "../assets/EVZ_UFG.svg";
import logoInf from "../assets/INF_UFG.svg";

const Logos = () => {
    return (
        <div>
            <img className='logos' src={logoUfg} alt='Logo da Universidade Federal de Goiás'/>
            <img className='logos' src={logoEvz} alt='Logo da Escola de Veterinária e Zootecnia'/>
            <img className='logos' src={logoInf} alt='Logo do Instituto de Informática'/>
        </div>
    )
}

export default Logos;
