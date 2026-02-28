import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { get } from '../api/client'

export default function TeacherStudentsPage() {
    const [students, setStudents] = useState([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        get('/teacher/students').then(async (res) => {
            if (res.ok) setStudents(await res.json())
            setLoading(false)
        })
    }, [])

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>

    return (
        <>
            <div className="page-header">
                <h1>Students</h1>
                <p>Select a student to view their test history and generate tests</p>
            </div>

            {students.length === 0 ? (
                <div className="empty-state">
                    <div className="icon">👥</div>
                    <h3>No students yet</h3>
                    <p>Students will appear here after they register</p>
                </div>
            ) : (
                <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
                    <div className="table-wrapper">
                        <table>
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Email</th>
                                    <th>Submissions</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {students.map((s) => (
                                    <tr key={s.id}>
                                        <td style={{ fontWeight: 500 }}>{s.name}</td>
                                        <td style={{ color: 'var(--text-secondary)' }}>{s.email}</td>
                                        <td>
                                            <span className="badge badge-info">{s.submissionCount}</span>
                                        </td>
                                        <td>
                                            <Link to={`/teacher/students/${s.id}`} className="btn btn-secondary" style={{ padding: '4px 12px', fontSize: 13 }}>
                                                View Questions
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
