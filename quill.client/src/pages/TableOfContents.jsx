import { useEffect, useState } from "react";
import { getTableOfContents } from "../../utils/api";
import "../css/tableofcontents.css";

const TableOfContents = ({ note, onClick }) => {
    const [content, setContent] = useState();
    useEffect(() => {
        const getToc = async () => {
            const response = await getTableOfContents(note);
            setContent(response);
        }

        getToc();
    }, []);

    const PrimaryContent = ({ node, level }) => {
        return (
            <div key={`${node.id}-div` }>
                <a onClick={() => onClick(node.id) }>{node.title}</a>
                <ul>
                    {node.children.map(child => (
                        <li key={`${child.id}-li` }>
                            <PrimaryContent node={child} level={level + 1} />
                        </li>
                    ))}
                </ul>
            </div>
        )
    }

    return (
        <div className="table-of-contents">
            {content && (
                <PrimaryContent node={content} level={0} />
            ) }
        </div>
    )
}

export default TableOfContents;