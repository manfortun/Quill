import { useEffect, useRef, useState } from "react";
import { LuPencil } from "react-icons/lu";
import { useNavigate, useParams } from "react-router-dom";
import { getNote } from "../../utils/api";
import LoadingPage from "./LoadingPage";
import TableOfContents from "./TableOfContents";
import "../css/view.css";

const View = () => {
    const navigate = useNavigate();
    const { noteId } = useParams();
    const [note, setNote] = useState();
    const [loading, setLoading] = useState(true);
    const containerRef = useRef(null);

    useEffect(() => {
        const tryGetNote = async () => {
            const note = await getNote(noteId);

            setNote(note);
        }

        tryGetNote();
        setLoading(false);
    }, []);

    useEffect(() => {
        let codeList = [];
        if (containerRef.current) {
            const childElements = containerRef.current.children;

            Array.from(childElements).forEach(paragraph => {
                if (Array.from(paragraph.childNodes).every(child => child.nodeName === 'CODE' && child.nodeType === Node.ELEMENT_NODE)) {
                    codeList.push(paragraph);
                } else {
                    replaceCodeWithPre(codeList);

                    codeList = [];
                }
            });

            if (codeList.length > 0) {
                replaceCodeWithPre(codeList);

                codeList = [];
            }
        }

        document.querySelectorAll('* > code').forEach(code => {
            if (code.previousSibling && code.previousSibling.nodeName === 'CODE') {
                code.classList.add('inner-forward-code');
            }

            if (code.nextSibling && code.nextSibling.nodeName === 'CODE') {
                code.classList.add('inner-backward-code');
            }
        })
    }, [note]);

    useEffect(() => {
        const handleScroll = () => {
            if (containerRef.current) {
                const childElements = containerRef.current.children;

                Array.from(childElements).forEach(element => {
                    setVisibility(element);
                });
            }
        }

        window.addEventListener('scroll', handleScroll);
        window.addEventListener('resize', handleScroll);

        handleScroll();

        return () => {
            window.removeEventListener('scroll', handleScroll);
            window.removeEventListener('resize', handleScroll);
        }
    }, [])

    const setVisibility = (element) => {
        if (element.childNodes) {
            element.childNodes.forEach(child => setVisibility(child));
        }

        if (element.nodeType === Node.ELEMENT_NODE) {
            if (!element.classList.contains('cvc')) {
                element.classList.add('cvc');
            }

            const rect = element.getBoundingClientRect();
            const isVisible = (rect.top >= 0 && rect.bottom <= (window.innerHeight || document.documentElement.clientHeight))
                || rect.bottom > 0 && rect.top < (window.innerHeight || document.documentElement.clientHeight);

            if (isVisible) {
                element.classList.remove('invisible');
            } else if (!element.classList.contains('invisible')) {
                element.classList.add('invisible');
            }
        }

    }

    useEffect(() => {
        document.querySelectorAll('pre').forEach(pre => {
            if (pre.firstChild && pre.firstChild.nodeName === 'CODE' && pre.firstChild.nextSibling.nodeName === 'BR' && pre.firstChild.innerText.startsWith('[Lang]:')) {
                const body_pre = document.createElement('pre');

                for (let i = 2; i < pre.childNodes.length; i++) {
                    body_pre.appendChild(pre.childNodes[i].cloneNode(true));
                }

                const header_content = document.createElement('p');
                header_content.innerHTML = pre.firstChild.innerHTML.replace("[Lang]:", "").trim();

                const header = document.createElement('div');
                header.classList.add('header');
                header.appendChild(header_content);

                const body = document.createElement('div');
                body.classList.add('card-body');
                body.appendChild(body_pre);

                const div = document.createElement('div');
                div.classList.add('card');
                div.appendChild(header);
                div.appendChild(body);

                pre.parentNode.replaceChild(div, pre);
            }
        })
    }, [note]);

    const replaceCodeWithPre = (codeList) => {
        let contents = [];
        if (codeList.length > 1) {
            codeList.forEach(p => {
                Array.from(p.childNodes).forEach(code => {
                    if (code.nodeName === 'CODE') {
                        contents.push(code);
                    }
                });
                contents.push(document.createElement('br'));
            });

            const pre = document.createElement('pre');

            contents.forEach(element => pre.appendChild(element));

            codeList.forEach(p => p.parentNode.replaceChild(pre, p));
        }
    }

    const handleEditClicked = () => {
        navigate(`/editor`, { state: { note } });
    }

    const handleScroll = (id) => {
        console.log('scrolling');
        const element = document.getElementById(id);

        const removeHighlights = document.getElementsByClassName('highlight');

        for (let i = 0; i < removeHighlights.length; i++) {
            removeHighlights[i].classList.remove('highlight');
        }

        if (element) {
            element.scrollIntoView({ behavior: 'smooth' });

            element.classList.add('highlight');
        }
    }

    if (loading || note == null) {
        return <LoadingPage />;
    }

    return (
        <div className="container-fluid position-relative">
            {note && (
                <>
                    <div className="container">
                        <div>
                            <div className="d-flex flex-row">
                                <h3>{note.title}</h3>
                                <button className="btn" title="Edit" onClick={() => handleEditClicked() }><LuPencil /></button>
                            </div>
                            <div className="ql-container ql-bubble">
                                <div dangerouslySetInnerHTML={{ __html: note.content }} className="ql-editor" id="ql-editor" ref={containerRef} />
                            </div>
                        </div>
                    </div>
                    <div className="position-absolute top-0">
                        <TableOfContents note={note} onClick={handleScroll} />
                    </div>
                </>
            )}
        </div>
    )
};

export default View;