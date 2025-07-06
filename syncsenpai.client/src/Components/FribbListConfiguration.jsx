import React, { useState } from "react";
import { ToastContainer, toast } from 'react-toastify';

export default function FribbListConfiguration() {
    const [fileSelected, setFileSelected] = useState();

    const saveFileSelected = (e) => {
        setFileSelected(e.target.files[0]);
    };

    const importFile = async (e) => {
        const formData = new FormData();
        formData.append("file", fileSelected);

        try {
            const response = await fetch('FribbList', {
                method: "POST",
             
                body: formData
            });

            if (response.ok) {
                toast('Saved FribbList');
            }
            else {
                toast('Failed to save FribbList');
            }
        } catch (ex) {
            console.log(ex);
        }

    };

    return (
        <>
            <ToastContainer
                position="bottom-right"
            />
            <input type="file" onChange={saveFileSelected} />
            <input type="button" value="upload" onClick={importFile} />
        </>
    );
};