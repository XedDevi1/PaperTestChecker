import { useState, useEffect } from 'react'
import { useParams, Link, useNavigate } from 'react-router-dom'
import { get, post } from '../api/client'

export default function TeacherStudentQuestionsPage() {
    const { id } = useParams()
    const navigate = useNavigate()
    const [questions, setQuestions] = useState([])
    const [selected, setSelected] = useState(new Set())
    const [loading, setLoading] = useState(true)
    const [title, setTitle] = useState('')
    const [generating, setGenerating] = useState(false)
    const [error, setError] = useState('')

    useEffect(() => {
        get(`/teacher/students/${id}/questions`).then(async (res) => {
            if (res.ok) setQuestions(await res.json())
            setLoading(false)
        })
    }, [id])

    const toggle = (qId) => {
        setSelected(prev => {
            const next = new Set(prev)
            next.has(qId) ? next.delete(qId) : next.add(qId)
            return next
        })
    }

    const selectAll = () => {
        if (selected.size === questions.length) {
            setSelected(new Set())
        } else {
            setSelected(new Set(questions.map(q => q.id)))
        }
    }

    const handleGenerate = async () => {
        if (selected.size === 0 || !title.trim()) return
        setError('')
        setGenerating(true)
        const res = await post('/teacher/generate-test', {
            title: title.trim(),
            studentId: id,
            questionResultIds: Array.from(selected)
        })
        setGenerating(false)
        if (res.ok) {
            const test = await res.json()
            navigate(`/teacher/tests/${test.id}`)
        } else {
            const data = await res.json()
            setError(data.error || 'Failed to generate test')
        }
    }

    if (loading) return <div className="spinner-page"><div className="spinner"></div></div>

    return (
        <>
            <div style={{ marginBottom: 16 }}>
                <Link to="/teacher/students" style={{ fontSize: 14, color: 'var(--text-secondary)' }}>← Back to Students</Link>
            </div>

            <div className="page-header">
                <h1>Student Questions</h1>
                <p>Select questions to include in a generated test</p>
            </div>

            {error && <div className="alert alert-error">{error}</div>}

            {questions.length === 0 ? (
                <div className="empty-state">
                    <div className="icon">📋</div>
                    <h3>No questions yet</h3>
                    <p>This student hasn't submitted any tests yet</p>
                </div>
            ) : (
                <>
                    <div className="card" style={{ marginBottom: 20, display: 'flex', alignItems: 'center', gap: 16, flexWrap: 'wrap' }}>
                        <button onClick={selectAll} className="btn btn-secondary" style={{ fontSize: 13 }}>
                            {selected.size === questions.length ? 'Deselect All' : 'Select All'}
                        </button>
                        <span style={{ fontSize: 14, color: 'var(--text-secondary)' }}>
                            {selected.size} of {questions.length} selected
                        </span>
                        <div style={{ flex: 1 }}></div>
                        <input
                            placeholder="Test title..."
                            value={title}
                            onChange={e => setTitle(e.target.value)}
                            style={{
                                padding: '8px 14px',
                                background: 'var(--bg)',
                                border: '1px solid var(--border)',
                                borderRadius: 'var(--radius-sm)',
                                color: 'var(--text)',
                                fontSize: 14,
                                fontFamily: 'inherit',
                                width: 200,
                            }}
                        />
                        <button
                            onClick={handleGenerate}
                            className="btn btn-primary"
                            disabled={selected.size === 0 || !title.trim() || generating}
                            style={{ fontSize: 13 }}
                        >
                            {generating ? 'Generating...' : `Generate Test (${selected.size})`}
                        </button>
                    </div>

                    {questions.map((q) => (
                        <div
                            key={q.id}
                            className={`question-card ${q.isCorrect ? 'correct' : 'incorrect'}`}
                            onClick={() => toggle(q.id)}
                            style={{ cursor: 'pointer', opacity: selected.has(q.id) ? 1 : 0.6, transition: 'opacity 0.15s' }}
                        >
                            <div className="question-header">
                                <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                                    <input
                                        type="checkbox"
                                        checked={selected.has(q.id)}
                                        onChange={() => toggle(q.id)}
                                        onClick={e => e.stopPropagation()}
                                        style={{ width: 16, height: 16 }}
                                    />
                                    <h3>Q{q.questionNumber}: {q.questionText}</h3>
                                </div>
                                <span className={`badge ${q.isCorrect ? 'badge-success' : 'badge-error'}`}>
                                    {q.isCorrect ? '✓' : '✗'}
                                </span>
                            </div>
                            <div className="question-body">
                                <div>
                                    <div className="label">Correct Answer</div>
                                    <div>{q.correctAnswer}</div>
                                </div>
                                <div>
                                    <div className="label">Student's Answer</div>
                                    <div>{q.studentAnswer}</div>
                                </div>
                            </div>
                            {q.options?.length > 0 && (
                                <div style={{ marginTop: 8, fontSize: 12, color: 'var(--text-secondary)' }}>
                                    Options: {q.options.join(' · ')}
                                </div>
                            )}
                        </div>
                    ))}
                </>
            )}
        </>
    )
}
