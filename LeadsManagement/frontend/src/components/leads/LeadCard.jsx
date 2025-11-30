import React from 'react'
import './LeadCard.css'
import {
  calendarIcon,
  locationIcon,
  categoryIcon,
  documentIcon,
  priceIcon,
  phoneIcon,
  emailIcon
} from './icons'

export function LeadCard({ lead, status, onAccept, onDecline, actionLoading }) {
  const isInvitedStatus = status === 'Invited'
  
  // For Invited: show first name only
  // For Accepted/Declined: show full name
  const displayName = isInvitedStatus 
    ? lead.contactFirstName
    : `${lead.contactFirstName} ${lead.contactLastName || ''}`.trim()

  // Format date for Invited tab: "January 4 @ 2:37 pm - 2025"
  const formatInvitedDate = (dateString) => {
    const date = new Date(dateString)
    const months = ['January', 'February', 'March', 'April', 'May', 'June', 
                    'July', 'August', 'September', 'October', 'November', 'December']
    const month = months[date.getMonth()]
    const day = date.getDate()
    const year = date.getFullYear()
    let hours = date.getHours()
    const minutes = date.getMinutes().toString().padStart(2, '0')
    const ampm = hours >= 12 ? 'pm' : 'am'
    hours = hours % 12 || 12
    
    return `${month} ${day} @ ${hours}:${minutes} ${ampm} - ${year}`
  }

  // Get first letter for avatar
  const firstLetter = lead.contactFirstName.charAt(0).toUpperCase()

  const formattedDate = new Date(lead.dateCreated).toLocaleDateString('eng-US', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  })

  return (
    <div className="lead-card">
      {isInvitedStatus ? (
        <>
          {/* Invited Tab Layout */}
          <div className="lead-card-header-invited">
            <div className="name-section">
              <div className="avatar-circle">{firstLetter}</div>
              <div className="name-date">
                <h3 className="lead-name">{displayName}</h3>
                <p className="lead-datetime">{formatInvitedDate(lead.dateCreated)}</p>
              </div>
            </div>
            <span className={`lead-status ${status.toLowerCase()}`}>
              {status}
            </span>
          </div>

          <div className="lead-card-body-invited">
            <div className="icons-row">
              <div className="lead-field-inline">
                <label>{locationIcon}</label>
                <p>{lead.suburb}</p>
              </div>
              <div className="lead-field-inline">
                <label>{categoryIcon}</label>
                <p>{lead.category}</p>
              </div>
              <div className="lead-field-inline">
                <p>Job ID: {lead.id}</p>
              </div>
            </div>

            <div className="description-section">
              <p>{lead.description}</p>
            </div>
          </div>

          <div className="lead-card-footer-invited">
            <div className="action-buttons">
              <button
                className="btn btn-accept-small"
                onClick={() => onAccept(lead.id)}
                disabled={actionLoading}
              >
                Accept
              </button>
              <button
                className="btn btn-decline-small"
                onClick={() => onDecline(lead.id)}
                disabled={actionLoading}
              >
                Decline
              </button>
            </div>
            <div className="price-display">
              <span className="price">${lead.price.toFixed(2)} Lead Invitation</span>
            </div>
          </div>
        </>
      ) : (
        <>
          {/* Accepted/Declined Tab Layout */}
          <div className="lead-card-header-invited">
            <div className="name-section">
              <div className="avatar-circle">{firstLetter}</div>
              <div className="name-date">
                <h3 className="lead-name">{displayName}</h3>
                <p className="lead-datetime">{formatInvitedDate(lead.dateCreated)}</p>
              </div>
            </div>
            <span className={`lead-status ${status.toLowerCase()}`}>
              {status}
            </span>
          </div>

          <div className="lead-card-body-invited">
            <div className="icons-row">
              <div className="lead-field-inline">
                <label>{locationIcon}</label>
                <p>{lead.suburb}</p>
              </div>
              <div className="lead-field-inline">
                <label>{categoryIcon}</label>
                <p>{lead.category}</p>
              </div>
              <div className="lead-field-inline">
                <p>Job ID: {lead.id}</p>
              </div>
              <div className="lead-field-inline">
                <p className="price-inline">${lead.price.toFixed(2)} Lead Invitation</p>
              </div>
            </div>

            <div className="contact-row">
              {lead.contactPhoneNumber && (
                <div className="lead-field-inline">
                  <label>{phoneIcon}</label>
                  <p>{lead.contactPhoneNumber}</p>
                </div>
              )}
              {lead.contactEmail && (
                <div className="lead-field-inline">
                  <label>{emailIcon}</label>
                  <p>{lead.contactEmail}</p>
                </div>
              )}
            </div>

            <div className="description-section">
              <p>{lead.description}</p>
            </div>
          </div>
        </>
      )}
    </div>
  )
}
