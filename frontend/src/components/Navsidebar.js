import React from "react";
import Navbar from "./HeadbarLogged";
import Sidebar from "./Sidebar";

function Navsidebar() {
    return (
        <div>
            <header>
                <Navbar />
            </header>
            <div>
                <Sidebar />
            </div>
        </div>
    )
}

export default Navsidebar;