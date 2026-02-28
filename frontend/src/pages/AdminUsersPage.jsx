import { useState, useEffect } from 'react'
import { get, del, post, put } from '../api/client'

export default function AdminUsersPage() {
    const [stats, setStats] = useState(null)
    const [users, setUsers] = useState([])
    const [loading, setLoading] = useState(true)
    const [pwModal, setPwModal] = useState(null) // { userId, name }
    const [newPassword, setNewPassword] = useState('')
    const [msg, setMsg] = useState('')

    const fetchData = async () => {
        setLoading(true)
        const [statsRes, usersRes] = await Promise.all([
            get('/admin/stats'),
            get('/admin/users'),
        ])
        if (statsRes.ok) setStats(await statsRes.json())
        if (usersRes.ok) setUsers(await usersRes.json())
        setLoading(false)
    }

    useEffect(() => { fetchData() }, [])

    const handleDelete = async (id, name) => {
        if (!confirm(`Delete user "${name}"? This will also delete all their data.`)) return
        const res = await del(`/admin/users/${id}`)
        if (res.ok) {
            setMsg(`User "${name}" deleted`)
            fetchData()
        }
    }

    const handleChangePassword = async () => {
        if (!pwModal || newPassword.length < 6) return
        const res = await post('/admin/users/change-password', {
            userId: pwModal.userId,
            newPassword
        })
        if (res.ok) {
            setMsg(`Password changed for "${pwModal.name}"`)
            setPwModal(null)
            setNewPassword('')
        }
    }

    const handleRoleChange = async (userId, role) => {
        const res = await put(`/admin/users/${userId}/role`, { role })
        if (res.ok) {
            setMsg('Role changed')
            fetchData()
        }
    }

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>

    return (
        <>
            <div className="page-header">
                <h1>Admin Dashboard</h1>
                <p>Manage users, view statistics, and administer the system</p>
            </div>

            {msg && (
                <div className="alert alert-success" style={{ marginBottom: 16 }}>
                    {msg}
                    <button onClick={() => setMsg('')} style={{ float: 'right', background: 'none', border: 'none', color: 'inherit', cursor: 'pointer' }}>✕</button>
                </div>
            )}

            {/* Stats cards */}
            {stats && (
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(130px, 1fr))', gap: 12, marginBottom: 24 }}>
                    {[
                        { label: 'Users', value: stats.totalUsers, icon: '👥' },
                        { label: 'Students', value: stats.studentCount, icon: '🎓' },
                        { label: 'Teachers', value: stats.teacherCount, icon: '📚' },
                        { label: 'Admins', value: stats.adminCount, icon: '🛡️' },
                        { label: 'Submissions', value: stats.totalSubmissions, icon: '📷' },
                        { label: 'Tests', value: stats.totalGeneratedTests, icon: '📝' },
                        { label: 'Attempts', value: stats.totalTestAttempts, icon: '✍️' },
                    ].map(s => (
                        <div key={s.label} className="card" style={{ textAlign: 'center', padding: '16px 12px' }}>
                            <div style={{ fontSize: 24, marginBottom: 4 }}>{s.icon}</div>
                            <div style={{ fontSize: 22, fontWeight: 700 }}>{s.value}</div>
                            <div style={{ fontSize: 12, color: 'var(--text-secondary)' }}>{s.label}</div>
                        </div>
                    ))}
                </div>
            )}

            {/* Users table */}
            <h2 style={{ fontSize: 18, fontWeight: 600, marginBottom: 12 }}>Users</h2>
            <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
                <div className="table-wrapper">
                    <table>
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Role</th>
                                <th>Submissions</th>
                                <th>Attempts</th>
                                <th>Joined</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map(u => (
                                <tr key={u.id}>
                                    <td style={{ fontWeight: 500 }}>{u.name}</td>
                                    <td style={{ color: 'var(--text-secondary)', fontSize: 13 }}>{u.email}</td>
                                    <td>
                                        <select
                                            value={u.role}
                                            onChange={e => handleRoleChange(u.id, e.target.value)}
                                            style={{
                                                background: 'var(--bg)',
                                                border: '1px solid var(--border)',
                                                borderRadius: 4,
                                                color: 'var(--text)',
                                                padding: '2px 6px',
                                                fontSize: 13,
                                                fontFamily: 'inherit',
                                            }}
                                        >
                                            <option value="student">student</option>
                                            <option value="teacher">teacher</option>
                                            <option value="admin">admin</option>
                                        </select>
                                    </td>
                                    <td><span className="badge badge-info">{u.submissionCount}</span></td>
                                    <td><span className="badge badge-info">{u.testAttemptCount}</span></td>
                                    <td style={{ color: 'var(--text-secondary)', fontSize: 13 }}>
                                        {new Date(u.createdAt).toLocaleDateString()}
                                    </td>
                                    <td>
                                        <div style={{ display: 'flex', gap: 6 }}>
                                            <button
                                                onClick={() => { setPwModal({ userId: u.id, name: u.name }); setNewPassword('') }}
                                                className="btn btn-secondary"
                                                style={{ padding: '3px 8px', fontSize: 12 }}
                                            >
                                                🔑
                                            </button>
                                            <button
                                                onClick={() => handleDelete(u.id, u.name)}
                                                className="btn btn-secondary"
                                                style={{ padding: '3px 8px', fontSize: 12, color: 'var(--error)' }}
                                            >
                                                🗑️
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {/* Password modal */}
            {pwModal && (
                <div style={styles.overlay} onClick={() => setPwModal(null)}>
                    <div className="card" style={styles.modal} onClick={e => e.stopPropagation()}>
                        <h3 style={{ marginBottom: 16 }}>Change Password — {pwModal.name}</h3>
                        <input
                            type="password"
                            placeholder="New password (min 6 chars)"
                            value={newPassword}
                            onChange={e => setNewPassword(e.target.value)}
                            style={styles.input}
                            autoFocus
                        />
                        <div style={{ display: 'flex', gap: 8, justifyContent: 'flex-end', marginTop: 16 }}>
                            <button onClick={() => setPwModal(null)} className="btn btn-secondary" style={{ fontSize: 13 }}>Cancel</button>
                            <button onClick={handleChangePassword} className="btn btn-primary" style={{ fontSize: 13 }} disabled={newPassword.length < 6}>
                                Change Password
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </>
    )
}

const styles = {
    overlay: {
        position: 'fixed',
        inset: 0,
        background: 'rgba(0,0,0,0.6)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        zIndex: 1000
    },
    modal: {
        width: 400,
        maxWidth: '90vw',
    },
    input: {
        width: '100%',
        padding: '10px 14px',
        background: 'var(--bg)',
        border: '1px solid var(--border)',
        borderRadius: 'var(--radius-sm)',
        color: 'var(--text)',
        fontSize: 14,
        fontFamily: 'inherit',
        boxSizing: 'border-box',
    },
}
