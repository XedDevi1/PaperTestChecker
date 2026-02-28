import { useState, useEffect } from 'react'
import { useParams, useNavigate, Link } from 'react-router-dom'
import { get, post } from '../api/client'

export default function TakeTestPage() {
    const { id } = useParams()
    const navigate = useNavigate()
    const [test, setTest] = useState(null)
    const [answers, setAnswers] = useState({})
    const [loading, setLoading] = useState(true)
    const [submitting, setSubmitting] = useState(false)
    const [error, setError] = useState('')

    useEffect(() => {
        get(`/student-tests/${id}`).then(async (res) => {
            if (res.ok) {
                const data = await res.json()
                setTest(data)
            } else {
                setError('Test not found or not assigned to you')
            }
            setLoading(false)
        })
    }, [id])

    const selectAnswer = (questionNumber, answer) => {
        setAnswers(prev => ({ ...prev, [questionNumber]: answer }))
    }

    const handleSubmit = async () => {
        if (!test) return
        const unanswered = test.questions.filter(q => !answers[q.questionNumber])
        if (unanswered.length > 0) {
            setError(`Please answer all questions. ${unanswered.length} remaining.`)
            return
        }

        setError('')
        setSubmitting(true)
        const res = await post(`/student-tests/${id}/submit`, {
            answers: test.questions.map(q => ({
                questionNumber: q.questionNumber,
                selectedAnswer: answers[q.questionNumber]
            }))
        })
        setSubmitting(false)

        if (res.ok) {
            const result = await res.json()
            navigate(`/tests/results/${result.id}`, { replace: true })
        } else {
            const data = await res.json()
            setError(data.error || 'Failed to submit test')
        }
    }

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>
    if (!test) return <div className="alert alert-error">{error || 'Test not found'}</div>

    const answeredCount = Object.keys(answers).length

    return (
        <>
            <div style={{ marginBottom: 16 }}>
                <Link to="/tests" style={{ fontSize: 14, color: 'var(--text-secondary)' }}>← Back to My Tests</Link>
            </div>

            <div className="page-header">
                <h1>{test.title}</h1>
                <p>By {test.teacherName} · {test.questions.length} questions</p>
            </div>

            {error && <div className="alert alert-error">{error}</div>}

            {test.questions.map((q) => (
                <div key={q.questionNumber} className="question-card" style={{
                    borderLeft: `3px solid ${answers[q.questionNumber] ? 'var(--accent)' : 'var(--border)'}`,
                }}>
                    <div className="question-header">
                        <h3>Question {q.questionNumber}</h3>
                        {answers[q.questionNumber] && (
                            <span className="badge badge-info">Answered</span>
                        )}
                    </div>

                    <p style={{ marginBottom: 16, fontSize: 15 }}>{q.questionText}</p>

                    <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
                        {q.options.map((opt, i) => (
                            <label
                                key={i}
                                style={{
                                    display: 'flex',
                                    alignItems: 'center',
                                    gap: 10,
                                    padding: '10px 14px',
                                    borderRadius: 'var(--radius-sm)',
                                    cursor: 'pointer',
                                    fontSize: 14,
                                    background: answers[q.questionNumber] === opt ? 'rgba(79, 142, 247, 0.1)' : 'var(--bg)',
                                    border: answers[q.questionNumber] === opt ? '1px solid var(--accent)' : '1px solid var(--border)',
                                    transition: 'all 0.15s',
                                }}
                            >
                                <input
                                    type="radio"
                                    name={`q${q.questionNumber}`}
                                    checked={answers[q.questionNumber] === opt}
                                    onChange={() => selectAnswer(q.questionNumber, opt)}
                                    style={{ accentColor: 'var(--accent)' }}
                                />
                                {String.fromCharCode(65 + i)}) {opt}
                            </label>
                        ))}
                    </div>
                </div>
            ))}

            <div className="card" style={{ marginTop: 24, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <span style={{ fontSize: 14, color: 'var(--text-secondary)' }}>
                    {answeredCount} of {test.questions.length} answered
                </span>
                <button
                    onClick={handleSubmit}
                    className="btn btn-primary"
                    disabled={submitting}
                >
                    {submitting ? 'Submitting...' : 'Submit Test'}
                </button>
            </div>
        </>
    )
}
