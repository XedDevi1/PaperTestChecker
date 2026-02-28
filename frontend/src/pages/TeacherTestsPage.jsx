import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { get } from '../api/client'

export default function TeacherTestsPage() {
    const [tests, setTests] = useState([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        get('/teacher/tests').then(async (res) => {
            if (res.ok) setTests(await res.json())
            setLoading(false)
        })
    }, [])

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>

    return (
        <>
            <div className="page-header">
                <h1>Generated Tests</h1>
                <p>Tests you've created from student question pools</p>
            </div>

            {tests.length === 0 ? (
                <div className="empty-state">
                    <div className="icon">📝</div>
                    <h3>No tests generated yet</h3>
                    <p>Go to Students → select questions → generate a test</p>
                    <Link to="/teacher/students" className="btn btn-primary" style={{ marginTop: 16 }}>
                        View Students
                    </Link>
                </div>
            ) : (
                <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
                    <div className="table-wrapper">
                        <table>
                            <thead>
                                <tr>
                                    <th>Title</th>
                                    <th>Student</th>
                                    <th>Questions</th>
                                    <th>Created</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {tests.map((t) => (
                                    <tr key={t.id}>
                                        <td style={{ fontWeight: 500 }}>{t.title}</td>
                                        <td>{t.studentName}</td>
                                        <td><span className="badge badge-info">{t.questionCount}</span></td>
                                        <td style={{ color: 'var(--text-secondary)' }}>
                                            {new Date(t.createdAt).toLocaleDateString()}
                                        </td>
                                        <td>
                                            <Link to={`/teacher/tests/${t.id}`} className="btn btn-secondary" style={{ padding: '4px 12px', fontSize: 13 }}>
                                                View
                                            </Link>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            )}
        </>
    )
}
