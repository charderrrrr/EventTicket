let currentEvent = null;
let seats = [];
let selectedSeat = null;
let categories = [];
let isProcessing = false;
let allEvents = [];
let reservedSeats = new Set();

async function loadEvents() {
    try {
        const events = await getEvents();
        allEvents = events;
        
        const select = document.getElementById('eventSelect');
        select.innerHTML = '';
        
        if (events.length === 0) {
            select.innerHTML = '<option value="">Нет активных событий</option>';
            document.getElementById('venueTitle').textContent = 'Нет активных событий';
            return;
        }

        events.forEach(event => {
            const option = document.createElement('option');
            option.value = event.id;
            const eventDate = new Date(event.date).toLocaleDateString('ru-RU', {
                day: 'numeric',
                month: 'long',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            });
            option.textContent = `${event.name} — ${eventDate}`;
            select.appendChild(option);
        });

        select.value = events[0].id;
        select.addEventListener('change', onEventChange);
        
        await loadEventData(events[0].id);
    } catch (error) {
        document.getElementById('venueTitle').textContent = 'Ошибка загрузки событий';
        console.error(error);
    }
}

async function onEventChange() {
    const eventId = parseInt(document.getElementById('eventSelect').value);
    if (!eventId) return;
    
    selectedSeat = null;
    reservedSeats.clear();
    document.getElementById('infoPanel').innerHTML = '<div class="info-placeholder">Выберите место на схеме</div>';
    document.getElementById('actions').style.display = 'none';
    document.getElementById('result').innerHTML = '';
    
    await loadEventData(eventId);
}

async function loadEventData(eventId) {
    try {
        showLoader();
        
        const event = allEvents.find(e => e.id === eventId);
        if (!event) {
            document.getElementById('venueTitle').textContent = 'Событие не найдено';
            hideLoader();
            return;
        }

        currentEvent = event;
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

function getDisplayStatus(seat) {
    if (reservedSeats.has(seat.id)) return 'Reserved';
    return getStatusText(seat.status);
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

        const displayStatus = getDisplayStatus(seat);

        if (displayStatus === 'Blocked') {
            seatEl.classList.add('blocked');
        } else if (displayStatus === 'Sold') {
            seatEl.classList.add('sold');
        } else if (displayStatus === 'Reserved') {
            seatEl.classList.add('reserved');
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
    const displayStatus = getDisplayStatus(seat);
    if (displayStatus === 'Blocked') return;

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

    const displayStatus = getDisplayStatus(seat);

    try {
        const priceInfo = await calculatePrice(currentEvent.id, seat.id);
        const categoryName = getCategoryName(seat.categoryId);

        let refundInfo = '';
        if (displayStatus === 'Sold') {
            const tickets = await getUserTickets();
            const ticket = tickets.find(t => t.seatId === seat.id && t.status === 0) || null;
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

        let statusText;
        if (displayStatus === 'Reserved') {
            statusText = 'Забронировано';
        } else if (displayStatus === 'Available') {
            statusText = 'Доступно';
        } else {
            statusText = 'Продано';
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
                    <span class="seat-detail-value">${statusText}</span>
                </div>
                ${refundInfo}
            </div>
        `;

        actions.style.display = 'flex';
            document.getElementById('reserveBtn').style.display = displayStatus === 'Available' ? 'block' : 'none';
            document.getElementById('buyBtn').style.display = (displayStatus === 'Available' || displayStatus === 'Reserved') ? 'block' : 'none';
            document.getElementById('refundBtn').style.display = displayStatus === 'Sold' ? 'block' : 'none';

            if (displayStatus === 'Reserved') {
                document.getElementById('buyBtn').textContent = 'Выкупить бронь';
                document.getElementById('buyBtn').style.backgroundColor = '#2c4a2e';
                document.getElementById('buyBtn').style.borderColor = '#4a7a4e';
            } else {
                document.getElementById('buyBtn').textContent = 'Купить билет';
                document.getElementById('buyBtn').style.backgroundColor = '#2c2760';
                document.getElementById('buyBtn').style.borderColor = '#5f5bb5';
            }
    } catch (error) {
        console.error('Ошибка получения информации о месте:', error);
    }
}

function showPaymentModal() {
    const modal = document.getElementById('paymentModal');
    modal.classList.remove('hidden-loader');
    
    const priceInfo = selectedSeat.price || 0;
    const categoryName = getCategoryName(selectedSeat.categoryId);
    const displayStatus = getDisplayStatus(selectedSeat);
    const isReserved = displayStatus === 'Reserved';
    
    document.getElementById('modalSeatInfo').innerHTML = `
        ${categoryName}, Ряд ${selectedSeat.row}, Место ${selectedSeat.number}<br>
        <strong>Цена: ${priceInfo} руб.</strong>
        ${isReserved ? '<br><span style="color: #ffad5c;">Выкуп брони</span>' : ''}
    `;
    
    document.getElementById('firstName').value = '';
    document.getElementById('lastName').value = '';
    document.getElementById('email').value = '';
    document.getElementById('paymentMethod').value = 'card';
}

function hidePaymentModal() {
    document.getElementById('paymentModal').classList.add('hidden-loader');
}

async function handleBuyTicket() {
    if (isProcessing) return;
    const displayStatus = getDisplayStatus(selectedSeat);
    if (!selectedSeat || (displayStatus !== 'Available' && displayStatus !== 'Reserved')) return;

    try {
        const priceInfo = await calculatePrice(currentEvent.id, selectedSeat.id);
        selectedSeat.price = priceInfo.price;
        showPaymentModal();
    } catch (error) {
        document.getElementById('result').className = 'result error';
        document.getElementById('result').innerHTML = error.message;
    }
}

async function confirmPayment() {
    const firstName = document.getElementById('firstName').value.trim();
    const lastName = document.getElementById('lastName').value.trim();
    const email = document.getElementById('email').value.trim();

    if (!firstName) {
        alert('Введите имя');
        return;
    }
    if (!lastName) {
        alert('Введите фамилию');
        return;
    }
    if (!email) {
        alert('Введите email');
        return;
    }

    hidePaymentModal();
    const displayStatus = getDisplayStatus(selectedSeat);

    if (displayStatus === 'Reserved') {
        try {
            isProcessing = true;
            showLoader();
            
            reservedSeats.delete(selectedSeat.id);
            const ticket = await purchaseTicket(currentEvent.id, selectedSeat.id);

            const data = await getVenueLayout(currentEvent.id);
            seats = data.seats;
            updateDemandInfo(data.demand);
            selectedSeat = seats.find(s => s.id === selectedSeat.id);

            renderSeats();
            if (selectedSeat) await showSeatInfo(selectedSeat);

            document.getElementById('result').className = 'result success';
            document.getElementById('result').innerHTML = `Успешная оплата :) Бронь выкуплена! Ряд ${selectedSeat.row}, Место ${selectedSeat.number}, Цена: ${ticket.price} руб.`;
            hideLoader();
        } catch (error) {
            reservedSeats.add(selectedSeat.id);
            document.getElementById('result').className = 'result error';
            document.getElementById('result').innerHTML = error.message;
            hideLoader();
        } finally {
            isProcessing = false;
        }
        return;
    }

    try {
        isProcessing = true;
        showLoader();
        
        const ticket = await purchaseTicket(currentEvent.id, selectedSeat.id);

        const data = await getVenueLayout(currentEvent.id);
        seats = data.seats;
        updateDemandInfo(data.demand);
        selectedSeat = seats.find(s => s.id === selectedSeat.id);

        renderSeats();
        if (selectedSeat) await showSeatInfo(selectedSeat);

        document.getElementById('result').className = 'result success';
        document.getElementById('result').innerHTML = `Успешная оплата :) Билет куплен! Ряд ${selectedSeat.row}, Место ${selectedSeat.number}, Цена: ${ticket.price} руб.`;
        hideLoader();
    } catch (error) {
        document.getElementById('result').className = 'result error';
        document.getElementById('result').innerHTML = error.message;
        hideLoader();
    } finally {
        isProcessing = false;
    }
}

function reserveSeat() {
    if (!selectedSeat || getDisplayStatus(selectedSeat) !== 'Available') return;
    
    reservedSeats.add(selectedSeat.id);
    renderSeats();
    
    selectedSeat = seats.find(s => s.id === selectedSeat.id);
    if (selectedSeat) showSeatInfo(selectedSeat);
    
    document.getElementById('result').className = 'result success';
    document.getElementById('result').innerHTML = `Место Ряд ${selectedSeat.row}, Место ${selectedSeat.number} забронировано! У вас есть 15 минут для оплаты.`;
}

async function handleRefundTicket() {
    if (isProcessing) return;
    if (!selectedSeat || getDisplayStatus(selectedSeat) !== 'Sold') return;

    try {
        isProcessing = true;
        showLoader();
        
        const tickets = await getUserTickets();
        const ticket = tickets.find(t => t.seatId === selectedSeat.id && t.status === 0) || null;
        
        if (!ticket) {
            const data = await getVenueLayout(currentEvent.id);
            seats = data.seats;
            updateDemandInfo(data.demand);
            selectedSeat = seats.find(s => s.id === selectedSeat.id);
            renderSeats();
            if (selectedSeat) await showSeatInfo(selectedSeat);
            throw new Error('Билет не найден. Возможно, место уже освобождено.');
        }

        const refund = await refundTicket(ticket.id);

        const data = await getVenueLayout(currentEvent.id);
        seats = data.seats;
        updateDemandInfo(data.demand);
        selectedSeat = seats.find(s => s.id === selectedSeat.id);

        renderSeats();
        if (selectedSeat) await showSeatInfo(selectedSeat);

        document.getElementById('result').className = 'result success';
        document.getElementById('result').innerHTML = `Возврат оформлен! Сумма: ${refund.refundAmount} руб. (удержана комиссия ${refund.commission} руб.)`;
        hideLoader();
    } catch (error) {
        console.error('Refund error:', error);
        document.getElementById('result').className = 'result error';
        document.getElementById('result').innerHTML = error.message;
        hideLoader();
    } finally {
        isProcessing = false;
    }
}

document.addEventListener('DOMContentLoaded', () => {
    loadEvents();
    document.getElementById('reserveBtn').addEventListener('click', reserveSeat);
    document.getElementById('buyBtn').addEventListener('click', handleBuyTicket);
    document.getElementById('refundBtn').addEventListener('click', handleRefundTicket);
    document.getElementById('confirmPaymentBtn').addEventListener('click', confirmPayment);
    document.getElementById('cancelPaymentBtn').addEventListener('click', hidePaymentModal);
});