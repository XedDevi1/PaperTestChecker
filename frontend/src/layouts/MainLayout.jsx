import { Outlet } from 'react-router-dom'
import Navbar from '../components/Navbar'

export default function MainLayout() {
    return (
        <>
            <Navbar />
            <main className="container" style={{ paddingTop: 32, paddingBottom: 48 }}>
                <Outlet />
            </main>
        </>
    )
}
