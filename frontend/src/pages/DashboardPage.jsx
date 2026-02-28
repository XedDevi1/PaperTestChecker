import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { get } from '../api/client'

export default function DashboardPage() {
    const [submissions, setSubmissions] = useState([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        get('/submissions').then(async (res) => {
            if (res.ok) setSubmissions(await res.json())
            setLoading(false)
        })
    }, [])

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>

    return (
        <>
            <div className="page-header">
                <h1>Dashboard</h1>
                <p>Your test submission history</p>
            </div>

            {submissions.length === 0 ? (
                <div className="empty-state">
                    <div className="icon">📄</div>
                    <h3>No submissions yet</h3>
                    <p>Upload a test photo to get started</p>
                    <Link to="/upload" className="btn btn-primary" style={{ marginTop: 16 }}>
                        Upload Test
                    </Link>
                </div>
            ) : (
                <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
                    <div className="table-wrapper">
                        <table>
                            <thead>
                                <tr>
                                    <th>Date</th>
                                    <th>Score</th>
                                    <th>Result</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {submissions.map((s) => (
                                    <tr key={s.id}>
                                        <td>{new Date(s.createdAt).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric', hour: '2-digit', minute: '2-digit' })}</td>
                                        <td style={{ fontWeight: 600 }}>{s.totalScore} / {s.maxScore}</td>
                                        <td>
                                            {s.totalScore === s.maxScore ? (
                                                <span className="badge badge-success">Perfect</span>
                                            ) : s.totalScore >= s.maxScore / 2 ? (
                                                <span className="badge badge-info">Passed</span>
                                            ) : (
                                                <span className="badge badge-error">Needs Work</span>
                                            )}
                                        </td>
                                        <td>
                                            <Link to={`/submissions/${s.id}`} className="btn btn-secondary" style={{ padding: '4px 12px', fontSize: 13 }}>
                                                View Details
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
