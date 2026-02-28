import { Outlet } from 'react-router-dom'
import Navbar from '../components/Navbar'

export default function AdminLayout() {
    return (
        <>
            <Navbar />
            <main className="container" style={{ paddingTop: 32, paddingBottom: 48 }}>
                <div style={styles.adminBanner}>
                    <span>🔒 Admin Panel</span>
                </div>
                <Outlet />
            </main>
        </>
    )
}

const styles = {
    adminBanner: {
        background: 'rgba(79, 142, 247, 0.08)',
        border: '1px solid rgba(79, 142, 247, 0.2)',
        borderRadius: 'var(--radius-sm)',
        padding: '8px 16px',
        fontSize: 13,
        fontWeight: 600,
        color: 'var(--accent)',
        marginBottom: 24,
    },
}
