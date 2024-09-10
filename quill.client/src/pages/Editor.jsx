import { useEffect, useState } from "react";
import ReactQuill from "react-quill";
import { LuCheck } from "react-icons/lu";
import { useLocation, useNavigate } from "react-router-dom";
import { autoSave, checkAutoSaved, getAutosavedContent, getNoteByHyperMedia, saveNote } from "../../utils/api";
import "../css/editor.css";
import LoadingPage from "./LoadingPage";
import BasicModal from "../modals/BasicModal";

const Editor = () => {
    const navigate = useNavigate();
    const [title, setTitle] = useState('');
    const [value, setValue] = useState('');
    const [autoSavedValue, setAutoSavedValue] = useState('');
    const [loading, setLoading] = useState(true);
    const [saveMessage, setSaveMessage] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const location = useLocation();
    const note = location.state?.note;

    useEffect(() => {
        if (note) {
            const tryGetNote = async () => {
                const response = await getNoteByHyperMedia(note);

                if (response) {
                    setTitle(response.title);
                    setValue(response.content);

                    const syncAutoSaved = async () => {
                        const hasAutosaved = await checkAutoSaved(response.title);

                        if (hasAutosaved) {
                            const content = await getAutosavedContent(response.title);

                            if (content && content !== '' && content != response.content) {
                                setAutoSavedValue(content);
                                setIsModalOpen(true);
                            }
                        }
                    }

                    syncAutoSaved();
                }
            }

            tryGetNote();
        }

        setLoading(false);
    }, []);

    useEffect(() => {
        document.querySelectorAll('p > code').forEach(code => {
            if (code.previousSibling && code.previousSibling.nodeName === 'CODE') {
                code.classList.add('inner-forward-code');
            }

            if (code.nextSibling && code.nextSibling.nodeName === 'CODE') {
                code.classList.add('inner-backward-code');
            }
        })
    }, [value]);

    useEffect(() => {
        const autoSaveData = async () => {
            if (!isModalOpen) {
                const response = await autoSave(title, value);
                setSaveMessage(response);
                return response;
            }
        };

        const interval = setInterval(() => {
            autoSaveData();
        }, 3000);

        return () => clearInterval(interval);
    }, [title, value]);

    useEffect(() => {
        const interval = setInterval(() => {
            if (saveMessage && saveMessage.length > 0) {
                setSaveMessage('');
            }
        }, 2000);

        return () => clearInterval(interval);
    }, [saveMessage]);

    const modules = {
        toolbar: [
            [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
            ['bold', 'italic', 'underline', 'strike', 'code'],
            ['blockquote'],
            [{ 'color': [] }, { 'background': [] }],
            [{'align': []}],
            [{ 'list': 'ordered' }, { 'list': 'bullet' }],
            ['link', 'image'],
            ['clean']  // remove formatting button
        ],
    };

    const handleSave = async (e) => {
        e.preventDefault();
        const save = async () => {
            const response = await saveNote(title, note?.title, value);

            if (response) {
                navigate(`/note/${response}`);
            }
        }

        save();
    }

    const handleOnTitleChanged = (event) => {
        const forbiddenCharacter = '-';
        const newValue = event.target.value;

        if (newValue.includes(forbiddenCharacter)) {
            setTitle(newValue.replace(forbiddenCharacter, ''));
        } else {
            setTitle(newValue);
        }
    }

    if (loading) {
        return <LoadingPage />;
    }

    const handleSyncAutoSaved = () => {
        setValue(autoSavedValue);
        setIsModalOpen(false);
    }

    return (
        <form className="container pt-3" onSubmit={handleSave}>
            <div className="position-fixed bottom-0 end-0 m-3">
                {saveMessage && <span className="text-white save-message">{ saveMessage === "Autosaved" ? <LuCheck /> : "" } {saveMessage}</span> }
            </div>
            <div className="d-flex flex-row align-items-center mb-3">
                <input className="form-control" value={title} onChange={handleOnTitleChanged} placeholder="Title" required />
                <button className="btn btn-success ms-2" type="submit">Save</button>
            </div>
            <div className="editor">
                <ReactQuill theme="bubble" value={value} onChange={setValue} modules={modules} />
            </div>
            <BasicModal isOpen={isModalOpen} closeModal={() => setIsModalOpen(false) }>
                <div>
                    <p className="text-black">We detected unsaved changes to this file. Do you want to load the autosaved changes?</p>
                    <div>
                        <button className="btn btn-danger me-2" onClick={() => setIsModalOpen(false) }>No</button>
                        <button className="btn btn-success" onClick={() => handleSyncAutoSaved() }>Yes</button>
                    </div>
                </div>
            </BasicModal>
        </form>
    )
}

export default Editor;