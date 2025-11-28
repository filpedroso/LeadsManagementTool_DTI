import React from 'react'
import { LeadCard } from './LeadCard'
import './LeadsList.css'

export function LeadsList({ leads, status, onAccept, onDecline, loading, error, actionLoading }) {
  if (loading) {
    return <div className="leads-list-empty">Carregando leads...</div>
  }

  if (error) {
    return <div className="leads-list-error">Erro ao carregar leads: {error}</div>
  }

  if (leads.length === 0) {
    return (
      <div className="leads-list-empty">
        <p>Nenhum lead encontrado com o status "{status}"</p>
      </div>
    )
  }

  return (
    <div className="leads-list">
      {leads.map((lead) => (
        <LeadCard
          key={lead.id}
          lead={lead}
          status={status}
          onAccept={onAccept}
          onDecline={onDecline}
          actionLoading={actionLoading}
        />
      ))}
    </div>
  )
}
