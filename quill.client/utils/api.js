import axios from "axios";
import { BASE_API } from "./constants";
import { getLink } from "./linkHelper";

const notes_controller = `${BASE_API}/Notes`
const editor_controller = `${BASE_API}/Editor`

const execute = async (callback) => {
    try {
        return await callback();
    } catch (err) {
        console.error(err.response.data);
    }
}

export async function getNotes() {
    return execute(async () => {
        const response = await axios.get(notes_controller);

        if (response.data) {
            return response.data;
        } else {
            return null;
        }
    })
}

export async function getNote(noteId) {
    return execute(async () => {
        const request = `${notes_controller}/${noteId}`;
        const response = await axios.get(request);

        if (response.data) {
            return response.data;
        } else {
            return null;
        }
    })
}

export async function getNoteByHyperMedia(note) {
    return execute(async () => {
        const request = getLink(note.links, 'self');
        const response = await axios.get(request.href);

        if (response.data) {
            return response.data;
        } else {
            return null;
        }
    })
}

export async function saveNote(title, oldTitle, content) {
    return execute(async () => {
        const header = { 'Content-Type': 'application/json' };
        const response = await axios.post(notes_controller, { title, oldTitle, content }, { header });

        if (response.data) {
            return response.data;
        } else {
            return null;
        }
    })
}

export async function deleteNote(note) {
    return execute(async () => {
        const request = getLink(note.links, 'delete');
        const response = await axios.delete(request.href);

        return response;
    })
}

export async function getTableOfContents(note) {
    return execute(async () => {
        const request = getLink(note.links, 'toc');
        const response = await axios.get(request.href);

        return response.data;
    })
}

export async function backupNotes() {
    return execute(async () => {
        const response = await axios.post(`${notes_controller}/backup`);

        return response.data;
    })
}

export async function getLastBackupDate() {
    return execute(async () => {
        const response = await axios.get(`${notes_controller}/get-last-backup-date`);

        return response.data;
    })
}

export async function autoSave(title, content) {
    return execute(async () => {
        const header = { 'Content-Type': 'application/json' };
        const response = await axios.post(`${editor_controller}`, { title, content }, { header });

        return response.data;
    })
}

export async function checkAutoSaved(title) {
    return execute(async () => {
        const response = await axios.get(`${editor_controller}/${title}`);

        return response.data;
    })
}

export async function getAutosavedContent(title) {
    return execute(async () => {
        const response = await axios.get(`${editor_controller}/get-content/${title}`);

        return response.data;
    })
}