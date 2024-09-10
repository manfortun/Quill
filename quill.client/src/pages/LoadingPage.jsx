import { SyncLoader } from 'react-spinners';

const LoadingPage = () => {
    return (
        <div className="position-fixed top-50 start-50 translate-middle">
            <div className="loading-page h-content">
                <SyncLoader size={10} color={"white"} loading={true }/>
            </div>
        </div>
    )
}

export default LoadingPage;