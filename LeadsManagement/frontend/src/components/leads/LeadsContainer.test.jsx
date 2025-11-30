import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { LeadsContainer } from './LeadsContainer'
import * as useLeadsHook from '../../hooks/useLeads'

vi.mock('../../hooks/useLeads')

describe('LeadsContainer', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  const mockLeads = [
    {
      id: 1,
      contactFirstName: 'John',
      contactLastName: 'Doe',
      suburb: 'Sydney',
      category: 'Plumbing',
      description: 'Fix leak',
      price: 150,
      dateCreated: '2025-11-30T10:00:00Z'
    },
    {
      id: 2,
      contactFirstName: 'Jane',
      contactLastName: 'Smith',
      suburb: 'Melbourne',
      category: 'Electrical',
      description: 'Install lights',
      price: 200,
      dateCreated: '2025-11-30T11:00:00Z'
    }
  ]

  it('should render tabs with counts', () => {
    vi.spyOn(useLeadsHook, 'useLeads').mockReturnValue({
      leads: mockLeads,
      loading: false,
      error: null,
      refetch: vi.fn()
    })

    vi.spyOn(useLeadsHook, 'useLeadCounts').mockReturnValue({
      counts: { Invited: 5, Accepted: 3, Declined: 2 },
      loading: false,
      refetchCounts: vi.fn()
    })

    vi.spyOn(useLeadsHook, 'useLeadActions').mockReturnValue({
      acceptLead: vi.fn(),
      declineLead: vi.fn(),
      actionLoading: false,
      actionError: null
    })

    render(<LeadsContainer />)

    // Use more specific queries to avoid matching status badges
    const tabButtons = screen.getAllByRole('button', { name: /invited|accepted/i })
    expect(tabButtons).toHaveLength(2)
    expect(screen.getByText('5')).toBeInTheDocument()
    expect(screen.getByText('3')).toBeInTheDocument()
  })

  it('should switch tabs when clicked', () => {
    const refetch = vi.fn()
    
    vi.spyOn(useLeadsHook, 'useLeads').mockReturnValue({
      leads: mockLeads,
      loading: false,
      error: null,
      refetch
    })

    vi.spyOn(useLeadsHook, 'useLeadCounts').mockReturnValue({
      counts: { Invited: 5, Accepted: 3, Declined: 2 },
      loading: false,
      refetchCounts: vi.fn()
    })

    vi.spyOn(useLeadsHook, 'useLeadActions').mockReturnValue({
      acceptLead: vi.fn(),
      declineLead: vi.fn(),
      actionLoading: false,
      actionError: null
    })

    render(<LeadsContainer />)

    const acceptedTab = screen.getByText(/Accepted/)
    fireEvent.click(acceptedTab.closest('button'))

    expect(acceptedTab.closest('button')).toHaveClass('active')
  })

  it('should display loading state', () => {
    vi.spyOn(useLeadsHook, 'useLeads').mockReturnValue({
      leads: [],
      loading: true,
      error: null,
      refetch: vi.fn()
    })

    vi.spyOn(useLeadsHook, 'useLeadCounts').mockReturnValue({
      counts: { Invited: 0, Accepted: 0, Declined: 0 },
      loading: false,
      refetchCounts: vi.fn()
    })

    vi.spyOn(useLeadsHook, 'useLeadActions').mockReturnValue({
      acceptLead: vi.fn(),
      declineLead: vi.fn(),
      actionLoading: false,
      actionError: null
    })

    render(<LeadsContainer />)

    expect(screen.getByText(/loading/i)).toBeInTheDocument()
  })

  it('should display error state', () => {
    vi.spyOn(useLeadsHook, 'useLeads').mockReturnValue({
      leads: [],
      loading: false,
      error: 'Failed to fetch leads',
      refetch: vi.fn()
    })

    vi.spyOn(useLeadsHook, 'useLeadCounts').mockReturnValue({
      counts: { Invited: 0, Accepted: 0, Declined: 0 },
      loading: false,
      refetchCounts: vi.fn()
    })

    vi.spyOn(useLeadsHook, 'useLeadActions').mockReturnValue({
      acceptLead: vi.fn(),
      declineLead: vi.fn(),
      actionLoading: false,
      actionError: null
    })

    render(<LeadsContainer />)

    expect(screen.getByText(/error/i)).toBeInTheDocument()
    expect(screen.getByText(/failed to fetch leads/i)).toBeInTheDocument()
  })

  it('should display empty state when no leads', () => {
    vi.spyOn(useLeadsHook, 'useLeads').mockReturnValue({
      leads: [],
      loading: false,
      error: null,
      refetch: vi.fn()
    })

    vi.spyOn(useLeadsHook, 'useLeadCounts').mockReturnValue({
      counts: { Invited: 0, Accepted: 0, Declined: 0 },
      loading: false,
      refetchCounts: vi.fn()
    })

    vi.spyOn(useLeadsHook, 'useLeadActions').mockReturnValue({
      acceptLead: vi.fn(),
      declineLead: vi.fn(),
      actionLoading: false,
      actionError: null
    })

    render(<LeadsContainer />)

    expect(screen.getByText(/No lead with/i)).toBeInTheDocument()
  })

  it('should render lead cards', () => {
    vi.spyOn(useLeadsHook, 'useLeads').mockReturnValue({
      leads: mockLeads,
      loading: false,
      error: null,
      refetch: vi.fn()
    })

    vi.spyOn(useLeadsHook, 'useLeadCounts').mockReturnValue({
      counts: { Invited: 2, Accepted: 0, Declined: 0 },
      loading: false,
      refetchCounts: vi.fn()
    })

    vi.spyOn(useLeadsHook, 'useLeadActions').mockReturnValue({
      acceptLead: vi.fn(),
      declineLead: vi.fn(),
      actionLoading: false,
      actionError: null
    })

    render(<LeadsContainer />)

    expect(screen.getByText('John')).toBeInTheDocument()
    expect(screen.getByText('Jane')).toBeInTheDocument()
  })

  it('should call handleAccept and refetch on accept', async () => {
    const acceptLead = vi.fn().mockResolvedValue(true)
    const refetch = vi.fn()
    const refetchCounts = vi.fn()

    vi.spyOn(useLeadsHook, 'useLeads').mockReturnValue({
      leads: mockLeads,
      loading: false,
      error: null,
      refetch
    })

    vi.spyOn(useLeadsHook, 'useLeadCounts').mockReturnValue({
      counts: { Invited: 2, Accepted: 0, Declined: 0 },
      loading: false,
      refetchCounts
    })

    vi.spyOn(useLeadsHook, 'useLeadActions').mockReturnValue({
      acceptLead,
      declineLead: vi.fn(),
      actionLoading: false,
      actionError: null
    })

    render(<LeadsContainer />)

    const acceptButtons = screen.getAllByText('Accept')
    fireEvent.click(acceptButtons[0])

    await waitFor(() => {
      expect(acceptLead).toHaveBeenCalledWith(1)
      expect(refetch).toHaveBeenCalled()
      expect(refetchCounts).toHaveBeenCalled()
    })
  })

  it('should call handleDecline and refetch on decline', async () => {
    const declineLead = vi.fn().mockResolvedValue(true)
    const refetch = vi.fn()
    const refetchCounts = vi.fn()

    vi.spyOn(useLeadsHook, 'useLeads').mockReturnValue({
      leads: mockLeads,
      loading: false,
      error: null,
      refetch
    })

    vi.spyOn(useLeadsHook, 'useLeadCounts').mockReturnValue({
      counts: { Invited: 2, Accepted: 0, Declined: 0 },
      loading: false,
      refetchCounts
    })

    vi.spyOn(useLeadsHook, 'useLeadActions').mockReturnValue({
      acceptLead: vi.fn(),
      declineLead,
      actionLoading: false,
      actionError: null
    })

    render(<LeadsContainer />)

    const declineButtons = screen.getAllByText('Decline')
    fireEvent.click(declineButtons[0])

    await waitFor(() => {
      expect(declineLead).toHaveBeenCalledWith(1)
      expect(refetch).toHaveBeenCalled()
      expect(refetchCounts).toHaveBeenCalled()
    })
  })
})
