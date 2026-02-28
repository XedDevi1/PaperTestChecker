import { useState, useEffect } from 'react'
import { useParams, Link } from 'react-router-dom'
import { get } from '../api/client'

export default function SubmissionDetailPage() {
    const { id } = useParams()
    const [submission, setSubmission] = useState(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        get(`/submissions/${id}`).then(async (res) => {
            if (res.ok) {
                setSubmission(await res.json())
            } else {
                setError('Submission not found')
            }
            setLoading(false)
        })
    }, [id])

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>
    if (error) return <div className="alert alert-error">{error}</div>
    if (!submission) return null

    return (
        <>
            <div style={{ marginBottom: 16 }}>
                <Link to="/dashboard" style={{ fontSize: 14, color: 'var(--text-secondary)' }}>← Back to Dashboard</Link>
            </div>

            <div className="page-header">
                <h1>Submission Detail</h1>
                <p>{new Date(submission.createdAt).toLocaleString()}</p>
            </div>

            <div className="card" style={{ textAlign: 'center', marginBottom: 24 }}>
                <div className="score-display" style={{ color: submission.totalScore >= submission.maxScore / 2 ? 'var(--success)' : 'var(--error)' }}>
                    {submission.totalScore} / {submission.maxScore}
                </div>
                <div className="score-label">Total Score</div>
            </div>

            {submission.questions.map((q) => (
                <div key={q.questionNumber} className={`question-card ${q.isCorrect ? 'correct' : 'incorrect'}`}>
                    <div className="question-header">
                        <h3>Question {q.questionNumber}</h3>
                        <span className={`badge ${q.isCorrect ? 'badge-success' : 'badge-error'}`}>
                            {q.isCorrect ? '✓ Correct' : '✗ Wrong'}
                        </span>
                    </div>

                    <p style={{ marginBottom: 12, fontSize: 14 }}>{q.questionText}</p>

                    <div className="question-body">
                        <div>
                            <div className="label">Student's Answer</div>
                            <div>{q.studentAnswer}</div>
                        </div>
                        <div>
                            <div className="label">Correct Answer</div>
                            <div>{q.correctAnswer}</div>
                        </div>
                    </div>

                    {!q.isCorrect && q.feedback && (
                        <div className="feedback-section">
                            <strong>Feedback:</strong> {q.feedback}
                            {q.recommendedReadings?.length > 0 && (
                                <ul className="readings-list">
                                    {q.recommendedReadings.map((r, i) => <li key={i}>{r}</li>)}
                                </ul>
                            )}
                        </div>
                    )}
                </div>
            ))}
        </>
    )
}
