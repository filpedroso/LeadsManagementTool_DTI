import React, { useState, useCallback, useEffect } from 'react'
import { useLeads, useLeadActions, useLeadCounts } from '../../hooks/useLeads'
import { LeadsList } from './LeadsList'
import { TabNavigation } from '../common/TabNavigation'
import { toast } from 'react-toastify'

export function LeadsContainer() {
  const [activeTab, setActiveTab] = useState('Invited')

  const { leads, loading, error, refetch } = useLeads(activeTab)
  const { counts, refetchCounts } = useLeadCounts()
  const { acceptLead, declineLead, actionLoading, actionError } = useLeadActions()

  const handleAccept = useCallback(
    async (leadId) => {
      const success = await acceptLead(leadId)
      if (success) {
        toast.success('Lead accepted succesfully!')
        refetch()
        refetchCounts()
      } else {
        toast.error(actionError || 'Error on accepting lead')
      }
    },
    [acceptLead, actionError, refetch, refetchCounts]
  )

  const handleDecline = useCallback(
    async (leadId) => {
      const success = await declineLead(leadId)
      if (success) {
        toast.success('Lead declined succesfully.')
        refetch()
        refetchCounts()
      } else {
        toast.error(actionError || 'Error on declining lead')
      }
    },
    [declineLead, actionError, refetch, refetchCounts]
  )

  const tabs = [
    { id: 'Invited', label: 'Invited', icon: '📨', count: counts.Invited },
    { id: 'Accepted', label: 'Accepted', icon: '✓', count: counts.Accepted },
  ]

  return (
    <div className="leads-container">
      <TabNavigation tabs={tabs} activeTab={activeTab} onTabChange={setActiveTab} />
      <LeadsList
        leads={leads}
        status={activeTab}
        onAccept={handleAccept}
        onDecline={handleDecline}
        loading={loading}
        error={error}
        actionLoading={actionLoading}
      />
    </div>
  )
}
