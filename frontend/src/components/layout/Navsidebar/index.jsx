import React from "react";
import Header from "../Header/Authenticated";
import Sidebar from "../Sidebar/Sidebar";

function Navsidebar() {
    return (
        <div>
            <header>
                <Header />
            </header>
            <div>
                <Sidebar />
            </div>
        </div>
    )
}

export default Navsidebar;