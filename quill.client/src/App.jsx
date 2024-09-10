import { BrowserRouter, Route, Routes } from 'react-router-dom';
import './App.css';
import Home from './pages/Home';
import Layout from './pages/Layout';
import Editor from './pages/Editor';
import View from './pages/View';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Layout /> }>
                    <Route index element={<Home /> } />
                    <Route path="editor" element={<Editor />} />
                    <Route path="note/:noteId" element={<View /> } />
                </Route>
            </Routes>
        </BrowserRouter>
    )
}

export default App;