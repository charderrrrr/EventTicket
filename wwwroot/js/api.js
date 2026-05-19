const API_URL = 'http://localhost:5003/api';

function showLoader() {
    document.getElementById('globalLoader').classList.remove('hidden-loader');
}

function hideLoader() {
    document.getElementById('globalLoader').classList.add('hidden-loader');
}

async function getEvents() {
    const response = await fetch(`${API_URL}/events`);
    if (!response.ok) throw new Error('Не удалось получить список событий');
    return await response.json();
}

async function getVenueLayout(eventId) {
    const response = await fetch(`${API_URL}/events/${eventId}/seats`);
    if (!response.ok) throw new Error('Не удалось получить схему зала');
    return await response.json();
}

async function calculatePrice(eventId, seatId) {
    const response = await fetch(`${API_URL}/events/${eventId}/seats/${seatId}/price`);
    if (!response.ok) throw new Error('Не удалось рассчитать цену');
    return await response.json();
}

async function purchaseTicket(eventId, seatId) {
    const response = await fetch(`${API_URL}/tickets/purchase`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            userId: 1,
            eventId: parseInt(eventId),
            seatId: parseInt(seatId)
        })
    });
    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.error || 'Не удалось купить билет');
    }
    return await response.json();
}

async function refundTicket(ticketId) {
    const response = await fetch(`${API_URL}/tickets/${ticketId}/refund`, { method: 'POST' });
    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.error || 'Не удалось вернуть билет');
    }
    return await response.json();
}

async function getUserTickets() {
    const response = await fetch(`${API_URL}/tickets`);
    if (!response.ok) throw new Error('Не удалось получить билеты');
    return await response.json();
}

async function calculateRefundCommission(ticketId) {
    const response = await fetch(`${API_URL}/tickets/${ticketId}/refund-commission`);
    if (!response.ok) throw new Error('Не удалось рассчитать комиссию');
    return await response.json();
}

async function getCategories() {
    const response = await fetch(`${API_URL}/categories`);
    if (!response.ok) throw new Error('Не удалось получить категории');
    return await response.json();
}