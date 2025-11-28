import React from 'react'
import './LeadCard.css'

export function LeadCard({ lead, status, onAccept, onDecline, actionLoading }) {
  const formattedDate = new Date(lead.dateCreated).toLocaleDateString('pt-BR', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  })

  const isInvitedStatus = status === 'Invited'

  return (
    <div className="lead-card">
      <div className="lead-card-header">
        <div className="lead-info-header">
          <h3 className="lead-name">
            {lead.contactFirstName} {lead.contactLastName || ''}
          </h3>
          <span className="lead-id">ID: {lead.id}</span>
        </div>
        <span className={`lead-status ${status.toLowerCase()}`}>
          {status}
        </span>
      </div>

      <div className="lead-card-body">
        <div className="lead-field">
          <label>Data de Criação</label>
          <p>{formattedDate}</p>
        </div>

        <div className="lead-field">
          <label>Região</label>
          <p>{lead.suburb}</p>
        </div>

        <div className="lead-field">
          <label>Categoria</label>
          <p>{lead.category}</p>
        </div>

        <div className="lead-field">
          <label>Descrição</label>
          <p>{lead.description}</p>
        </div>

        <div className="lead-field">
          <label>Preço</label>
          <p className="price">${lead.price.toFixed(2)}</p>
        </div>

        {!isInvitedStatus && (
          <>
            {lead.contactPhoneNumber && (
              <div className="lead-field">
                <label>Telefone</label>
                <p>{lead.contactPhoneNumber}</p>
              </div>
            )}

            {lead.contactEmail && (
              <div className="lead-field">
                <label>Email</label>
                <p>{lead.contactEmail}</p>
              </div>
            )}
          </>
        )}
      </div>

      {isInvitedStatus && (
        <div className="lead-card-footer">
          <button
            className="btn btn-accept"
            onClick={() => onAccept(lead.id)}
            disabled={actionLoading}
          >
            ✓ Aceitar
          </button>
          <button
            className="btn btn-decline"
            onClick={() => onDecline(lead.id)}
            disabled={actionLoading}
          >
            ✕ Recusar
          </button>
        </div>
      )}
    </div>
  )
}
