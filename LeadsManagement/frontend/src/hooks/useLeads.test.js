import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor } from '@testing-library/react'
import { useLeads, useLeadCounts, useLeadActions } from './useLeads'
import { leadsApi } from '../api/leadsApi'

vi.mock('../api/leadsApi')

describe('useLeads', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should fetch leads by status successfully', async () => {
    const mockLeads = [
      { id: 1, contactFirstName: 'John', status: 0 },
      { id: 2, contactFirstName: 'Jane', status: 0 }
    ]

    vi.mocked(leadsApi.getLeadsByStatus).mockResolvedValue({ data: mockLeads })

    const { result } = renderHook(() => useLeads('Invited'))

    expect(result.current.loading).toBe(true)
    expect(result.current.leads).toEqual([])

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.leads).toEqual(mockLeads)
    expect(result.current.error).toBeNull()
    expect(leadsApi.getLeadsByStatus).toHaveBeenCalledWith('Invited')
  })

  it('should handle fetch error', async () => {
    const errorMessage = 'Failed to fetch'
    vi.mocked(leadsApi.getLeadsByStatus).mockRejectedValue(new Error(errorMessage))

    const { result } = renderHook(() => useLeads('Invited'))

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.leads).toEqual([])
    expect(result.current.error).toBe(errorMessage)
  })

  it('should refetch leads when refetch is called', async () => {
    const mockLeads = [{ id: 1, contactFirstName: 'John' }]
    vi.mocked(leadsApi.getLeadsByStatus).mockResolvedValue({ data: mockLeads })

    const { result } = renderHook(() => useLeads('Invited'))

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(leadsApi.getLeadsByStatus).toHaveBeenCalledTimes(1)

    result.current.refetch()

    await waitFor(() => {
      expect(leadsApi.getLeadsByStatus).toHaveBeenCalledTimes(2)
    })
  })
})

describe('useLeadCounts', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should fetch counts for all statuses', async () => {
    const mockInvited = [{ id: 1 }, { id: 2 }]
    const mockAccepted = [{ id: 3 }]
    const mockDeclined = []

    vi.mocked(leadsApi.getLeadsByStatus)
      .mockResolvedValueOnce({ data: mockInvited })
      .mockResolvedValueOnce({ data: mockAccepted })
      .mockResolvedValueOnce({ data: mockDeclined })

    const { result } = renderHook(() => useLeadCounts())

    await waitFor(() => {
      expect(result.current.counts).toEqual({
        Invited: 2,
        Accepted: 1,
        Declined: 0
      })
    })
  })

  it('should handle fetch counts error', async () => {
    vi.mocked(leadsApi.getLeadsByStatus).mockRejectedValue(new Error('Failed'))

    const { result } = renderHook(() => useLeadCounts())

    // Should maintain initial state on error
    await waitFor(() => {
      expect(result.current.counts).toEqual({
        Invited: 0,
        Accepted: 0,
        Declined: 0
      })
    })
  })
})

describe('useLeadActions', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should accept lead successfully', async () => {
    vi.mocked(leadsApi.acceptLead).mockResolvedValue({})

    const { result } = renderHook(() => useLeadActions())

    expect(result.current.actionLoading).toBe(false)

    // Call acceptLead and wait for it to complete
    const successPromise = result.current.acceptLead(1)
    
    // Wait for the loading state and success result
    const success = await successPromise

    await waitFor(() => {
      expect(result.current.actionLoading).toBe(false)
    })

    expect(success).toBe(true)
    expect(leadsApi.acceptLead).toHaveBeenCalledWith(1)
    expect(result.current.actionError).toBeNull()
  })

  it('should decline lead successfully', async () => {
    vi.mocked(leadsApi.declineLead).mockResolvedValue({})

    const { result } = renderHook(() => useLeadActions())

    const success = await result.current.declineLead(1)

    expect(success).toBe(true)
    expect(leadsApi.declineLead).toHaveBeenCalledWith(1)
    expect(result.current.actionError).toBeNull()
  })

  it('should handle accept error', async () => {
    const errorMessage = 'Accept failed'
    vi.mocked(leadsApi.acceptLead).mockRejectedValue(new Error(errorMessage))

    const { result } = renderHook(() => useLeadActions())

    const success = await result.current.acceptLead(1)

    await waitFor(() => {
      expect(result.current.actionError).toBe(errorMessage)
    })

    expect(success).toBe(false)
  })

  it('should handle decline error', async () => {
    const errorMessage = 'Decline failed'
    vi.mocked(leadsApi.declineLead).mockRejectedValue(new Error(errorMessage))

    const { result } = renderHook(() => useLeadActions())

    const success = await result.current.declineLead(1)

    await waitFor(() => {
      expect(result.current.actionError).toBe(errorMessage)
    })

    expect(success).toBe(false)
  })
})
