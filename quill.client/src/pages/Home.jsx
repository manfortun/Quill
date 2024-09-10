import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { backupNotes, deleteNote, getLastBackupDate, getNotes } from "../../utils/api";
import { getLink } from "../../utils/linkHelper";
import { LuTrash, LuPencil, LuDatabaseBackup } from "react-icons/lu";
import "../css/home.css";
import BasicModal from "../modals/BasicModal";

const Home = () => {
    const [notes, setNotes] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [modalMessage, setModalMessage] = useState('');
    const [targetDelete, setTargetDelete] = useState();
    const [orderCondition, setOrderCondition] = useState('Title');
    const navigate = useNavigate();

    useEffect(() => {
        const trySetNotes = async () => {
            const notes = await getNotes();

            setNotes(notes);
        }

        trySetNotes();
    }, []);

    useEffect(() => {
        if (!isModalOpen) {
            setTargetDelete(null);
            setModalMessage('');
        }
    }, [isModalOpen]);

    const handleNoteClicked = async (note) => {
        try {
            const link = getLink(note.links, 'self');

            if (link) {
                navigate(`/note/${link.url}`);
            }
        } catch (err) {
            console.error(err.toString());
        }
    }

    const handleShowDeleteModal = (note) => {
        setModalMessage(`Are you sure you want to delete ${note.title}?`);
        setTargetDelete(note);
        setIsModalOpen(true);
    }

    const handleDelete = (note) => {
        const response = deleteNote(note);

        if (response) {
            setIsModalOpen(false);
            window.location.reload();
        } else {
            setModalMessage('Could not delete this note. Do you want to try again?');
        }
    }

    const BackupButton = () => {
        const [ backupDate, setBackupDate ] = useState('');

        const handleBackup = async () => {
            setBackupDate('Backing up...');
            const response = await backupNotes();

            if (response) {
                getBackupDate();
            }
        }

        useEffect(() => {
            getBackupDate();
        }, []);

        const getBackupDate = async () => {
            const response = await getLastBackupDate();

            setBackupDate(response);
        }

        return (
            <div className="position-fixed top-0 end-0 m-3 d-flex flex-row-reverse align-items-center backup-div">
                <button className="btn backup-button" title="Backup" onClick={() => handleBackup() }><LuDatabaseBackup /></button>
                <small className="text-secondary">{backupDate}</small>
            </div>
        );
    }

    return (
        <div className="container">
            <BackupButton />
            <table className="table">
                <thead>
                    <tr>
                        <th>Title</th>
                        <td>Date modified</td>
                        <td></td>
                    </tr>
                </thead>
                <tbody>
                    {notes && notes.map(note => (
                        <tr key={note.title} className="note-record">
                            <td onClick={() => handleNoteClicked(note)}>{note.title}</td>
                            <td onClick={() => handleNoteClicked(note)}><small>{note.lastWriteString}</small></td>
                            <td className="item-options">
                                <div>
                                    <button className="btn text-hover-primary" title="Edit" onClick={() => navigate(`/editor`, { state: { note } }) }><LuPencil /></button>
                                    <button className="btn text-hover-danger" title="Delete" onClick={() => handleShowDeleteModal(note) }><LuTrash /></button>
                                </div>
                            </td>
                        </tr>
                    )) }
                </tbody>
            </table>
            <BasicModal isOpen={isModalOpen} closeModal={() => setIsModalOpen(false)}>
                <div>
                    <div className="my-3">
                        {modalMessage}
                    </div>
                    <div className="text-end">
                        <button className="btn btn-success me-2" onClick={() => setIsModalOpen(false) }>No</button>
                        <button className="btn btn-danger" onClick={() => handleDelete(targetDelete) }>Yes</button>
                    </div>
                </div>
            </BasicModal>
        </div>
    )
}

export default Home;