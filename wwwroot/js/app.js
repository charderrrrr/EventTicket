let currentEvent = null;
let seats = [];
let selectedSeat = null;
let categories = [];

async function loadEventData() {
    try {
        showLoader();
        const events = await getEvents();
        if (events.length === 0) {
            document.getElementById('venueTitle').textContent = 'Нет активных событий';
            hideLoader();
            return;
        }

        currentEvent = events[0];
        document.getElementById('venueTitle').textContent = currentEvent.name;

        categories = await getCategories();
        updateLegend();

        await loadSeats();
        hideLoader();
    } catch (error) {
        document.getElementById('venueTitle').textContent = 'Ошибка загрузки';
        hideLoader();
        console.error(error);
    }
}

function updateLegend() {
    categories.forEach(category => {
        const elementId = category.name.toLowerCase() + 'Legend';
        const element = document.getElementById(elementId);
        if (element) {
            element.textContent = `${category.name} - ${category.basePrice} руб.`;
        }
    });
}

async function loadSeats() {
    try {
        const data = await getVenueLayout(currentEvent.id);
        seats = data.seats;
        updateDemandInfo(data.demand);
        renderSeats();
    } catch (error) {
        console.error('Ошибка загрузки мест:', error);
    }
}

function updateDemandInfo(demand) {
    document.getElementById('soldCount').textContent = demand.soldSeats;
    document.getElementById('totalCount').textContent = demand.totalSeats;
    document.getElementById('demandCoefficient').textContent = demand.coefficient.toFixed(1);
}

function getCategoryName(categoryId) {
    const category = categories.find(c => c.id === categoryId);
    return category ? category.name : 'Неизвестно';
}

function getCategoryColor(categoryId) {
    const category = categories.find(c => c.id === categoryId);
    if (!category) return '';
    return category.name.toLowerCase();
}

function getStatusText(status) {
    switch (status) {
        case 0: return 'Available';
        case 1: return 'Sold';
        case 2: return 'Blocked';
        default: return 'Available';
    }
}

function renderSeats() {
    const grid = document.getElementById('seatGrid');
    if (!currentEvent) return;

    const maxNumber = Math.max(...seats.map(s => s.number));
    grid.style.gridTemplateColumns = `repeat(${maxNumber}, 52px)`;
    grid.innerHTML = '';

    seats.forEach(seat => {
        const seatEl = document.createElement('div');
        seatEl.className = 'seat';
        seatEl.dataset.seatId = seat.id;
        seatEl.innerHTML = `<span>${seat.row}</span><span>${seat.number}</span>`;

        const statusText = getStatusText(seat.status);

        if (statusText === 'Blocked') {
            seatEl.classList.add('blocked');
        } else if (statusText === 'Sold') {
            seatEl.classList.add('sold');
        } else {
            seatEl.classList.add('available');
            const colorClass = getCategoryColor(seat.categoryId);
            if (colorClass) seatEl.classList.add(colorClass);
        }

        seatEl.addEventListener('click', () => onSeatClick(seat));
        grid.appendChild(seatEl);
    });
}

function onSeatClick(seat) {
    if (getStatusText(seat.status) === 'Blocked') return;

    selectedSeat = seat;
    showSeatInfo(seat);

    document.querySelectorAll('.seat').forEach(el => el.classList.remove('selected'));
    const seatEl = document.querySelector(`[data-seat-id="${seat.id}"]`);
    if (seatEl) seatEl.classList.add('selected');
}

async function showSeatInfo(seat) {
    const panel = document.getElementById('infoPanel');
    const actions = document.getElementById('actions');
    const result = document.getElementById('result');
    result.innerHTML = '';

    const statusText = getStatusText(seat.status);

    try {
        const priceInfo = await calculatePrice(currentEvent.id, seat.id);
        const categoryName = getCategoryName(seat.categoryId);

        let refundInfo = '';
        if (statusText === 'Sold') {
            const ticket = await getSeatTicket(seat.id);
            if (ticket) {
                const commissionInfo = await calculateRefundCommission(ticket.id);
                refundInfo = `
                    <div class="seat-detail-row">
                        <span class="seat-detail-label">Комиссия за возврат</span>
                        <span class="seat-detail-value">${commissionInfo.commission} руб.</span>
                    </div>
                    <div class="seat-detail-row">
                        <span class="seat-detail-label">Сумма к возврату</span>
                        <span class="seat-detail-value">${commissionInfo.refundAmount} руб.</span>
                    </div>
                `;
            }
        }

        panel.innerHTML = `
            <div class="seat-detail">
                <div style="font-size: 1.2rem; font-weight: 700; color: #cfdfff; margin-bottom: 0.5rem;">${categoryName}</div>
                <div class="seat-detail-row">
                    <span class="seat-detail-label">Ряд</span>
                    <span class="seat-detail-value">${seat.row}</span>
                </div>
                <div class="seat-detail-row">
                    <span class="seat-detail-label">Место</span>
                    <span class="seat-detail-value">${seat.number}</span>
                </div>
                <div class="seat-detail-row">
                    <span class="seat-detail-label">Текущая цена</span>
                    <span class="seat-detail-value price-value">${priceInfo.price} руб.</span>
                </div>
                <div class="seat-detail-row">
                    <span class="seat-detail-label">Статус</span>
                    <span class="seat-detail-value">${statusText === 'Available' ? 'Доступно' : 'Продано'}</span>
                </div>
                ${refundInfo}
            </div>
        `;

        actions.style.display = 'flex';
        document.getElementById('buyBtn').style.display = statusText === 'Available' ? 'block' : 'none';
        document.getElementById('refundBtn').style.display = statusText === 'Sold' ? 'block' : 'none';
    } catch (error) {
        console.error('Ошибка получения информации о месте:', error);
    }
}

async function getSeatTicket(seatId) {
    try {
        const tickets = await getUserTickets();
        return tickets.find(t => t.seatId === seatId && t.status === 0) || null;
    } catch {
        return null;
    }
}

async function buyTicket() {
    if (!selectedSeat || getStatusText(selectedSeat.status) !== 'Available') return;

    try {
        showLoader();
        const ticket = await purchaseTicket(currentEvent.id, selectedSeat.id);

        const data = await getVenueLayout(currentEvent.id);
        seats = data.seats;
        updateDemandInfo(data.demand);
        selectedSeat = seats.find(s => s.id === selectedSeat.id);

        renderSeats();
        if (selectedSeat) showSeatInfo(selectedSeat);

        document.getElementById('result').className = 'result success';
        document.getElementById('result').innerHTML = `Билет куплен! Ряд ${selectedSeat.row}, Место ${selectedSeat.number}, Цена: ${ticket.price} руб.`;
        hideLoader();
    } catch (error) {
        document.getElementById('result').className = 'result error';
        document.getElementById('result').innerHTML = error.message;
        hideLoader();
    }
}

async function refundTicket() {
    if (!selectedSeat || getStatusText(selectedSeat.status) !== 'Sold') return;

    try {
        showLoader();
        const ticket = await getSeatTicket(selectedSeat.id);
        if (!ticket) throw new Error('Билет не найден');

        const refund = await refundTicket(ticket.id);

        const data = await getVenueLayout(currentEvent.id);
        seats = data.seats;
        updateDemandInfo(data.demand);
        selectedSeat = seats.find(s => s.id === selectedSeat.id);

        renderSeats();
        if (selectedSeat) showSeatInfo(selectedSeat);

        document.getElementById('result').className = 'result success';
        document.getElementById('result').innerHTML = `Возврат оформлен! Сумма: ${refund.refundAmount} руб. (удержана комиссия ${refund.commission} руб.)`;
        hideLoader();
    } catch (error) {
        document.getElementById('result').className = 'result error';
        document.getElementById('result').innerHTML = error.message;
        hideLoader();
    }
}

document.addEventListener('DOMContentLoaded', () => {
    loadEventData();
    document.getElementById('buyBtn').addEventListener('click', buyTicket);
    document.getElementById('refundBtn').addEventListener('click', refundTicket);
});