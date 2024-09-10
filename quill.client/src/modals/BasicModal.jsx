import "../css/modal.css";

const BasicModal = ({ isOpen, closeModal, children }) => {
    if (!isOpen) return null;

    return (
        <div className="modal-overlay">
            <div className="modal">
                {children }
            </div>
        </div>
    )
}

export default BasicModal;