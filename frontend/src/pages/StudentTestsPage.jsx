import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { get } from '../api/client'

export default function StudentTestsPage() {
    const [tests, setTests] = useState([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        get('/student-tests').then(async (res) => {
            if (res.ok) setTests(await res.json())
            setLoading(false)
        })
    }, [])

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>

    return (
        <>
            <div className="page-header">
                <h1>My Tests</h1>
                <p>Tests assigned to you by teachers</p>
            </div>

            {tests.length === 0 ? (
                <div className="empty-state">
                    <div className="icon">📝</div>
                    <h3>No tests assigned</h3>
                    <p>Your teacher hasn't created any tests for you yet</p>
                </div>
            ) : (
                <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
                    {tests.map((t) => (
                        <div key={t.id} className="card" style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
                            <div style={{ flex: 1 }}>
                                <div style={{ fontWeight: 600, fontSize: 16, marginBottom: 4 }}>{t.title}</div>
                                <div style={{ fontSize: 13, color: 'var(--text-secondary)' }}>
                                    By {t.teacherName} · {t.questionCount} questions · {new Date(t.createdAt).toLocaleDateString()}
                                </div>
                            </div>
                            {t.alreadyTaken ? (
                                <span className="badge badge-success" style={{ fontSize: 13, padding: '6px 14px' }}>✓ Completed</span>
                            ) : (
                                <Link to={`/tests/${t.id}/take`} className="btn btn-primary" style={{ fontSize: 13 }}>
                                    Take Test
                                </Link>
                            )}
                        </div>
                    ))}
                </div>
            )}
        </>
    )
}
