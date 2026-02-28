import { Outlet } from 'react-router-dom'
import Navbar from '../components/Navbar'

export default function TeacherLayout() {
    return (
        <>
            <Navbar />
            <main className="container" style={{ paddingTop: 32, paddingBottom: 48 }}>
                <div style={styles.banner}>
                    <span>🎓 Teacher Panel</span>
                </div>
                <Outlet />
            </main>
        </>
    )
}

const styles = {
    banner: {
        background: 'rgba(74, 222, 128, 0.08)',
        border: '1px solid rgba(74, 222, 128, 0.2)',
        borderRadius: 'var(--radius-sm)',
        padding: '8px 16px',
        fontSize: 13,
        fontWeight: 600,
        color: 'var(--success)',
        marginBottom: 24,
    },
}
