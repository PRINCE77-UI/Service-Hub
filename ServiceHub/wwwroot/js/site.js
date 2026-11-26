// ServiceHub — Global JS

// ── Toast Notification System ───────────────────────────────
function showToast(message, type = 'success') {
    const container = document.getElementById('toast-container') || createToastContainer();
    const id = 'toast-' + Date.now();
    const icons = { success: 'fas fa-check-circle', error: 'fas fa-times-circle', info: 'fas fa-info-circle' };
    const toast = document.createElement('div');
    toast.id = id;
    toast.className = `toast-hub toast-${type} d-flex align-items-center p-3 mb-2`;
    toast.innerHTML = `
        <i class="${icons[type] || icons.info} me-2 fs-5"></i>
        <span class="flex-grow-1 fw-500">${message}</span>
        <button type="button" onclick="dismissToast('${id}')" class="btn-close btn-close-white ms-3" aria-label="Close"></button>
    `;
    container.appendChild(toast);
    setTimeout(() => dismissToast(id), 5000);
}

function dismissToast(id) {
    const el = document.getElementById(id);
    if (el) { el.style.opacity = '0'; el.style.transform = 'translateX(110%)'; setTimeout(() => el.remove(), 300); }
}

function createToastContainer() {
    const c = document.createElement('div');
    c.id = 'toast-container';
    c.className = 'toast-container';
    document.body.appendChild(c);
    return c;
}

// ── Auto-show TempData toasts ────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    const successMsg = document.getElementById('tempdata-success')?.textContent;
    const errorMsg = document.getElementById('tempdata-error')?.textContent;
    if (successMsg) showToast(successMsg, 'success');
    if (errorMsg) showToast(errorMsg, 'error');
});

// ── Star Rating Interactive ──────────────────────────────────
function initStarRating() {
    const stars = document.querySelectorAll('.star-input-group label');
    stars.forEach(star => {
        star.addEventListener('mouseover', function () {
            const val = parseInt(this.getAttribute('for').replace('star', ''));
            stars.forEach((s, i) => {
                const sVal = parseInt(s.getAttribute('for').replace('star', ''));
                s.style.color = sVal <= val ? '#F59E0B' : '#CBD5E1';
            });
        });
    });
}

// ── Booking Time Validation ──────────────────────────────────
function initBookingForm() {
    const startInput = document.getElementById('StartTime');
    const endInput = document.getElementById('EndTime');
    const priceDisplay = document.getElementById('priceDisplay');
    const pricePerUnit = parseFloat(document.getElementById('pricePerUnit')?.value || 0);
    const priceType = document.getElementById('priceType')?.value;

    function updatePrice() {
        if (!startInput || !endInput || !priceDisplay) return;
        if (startInput.value && endInput.value) {
            const [sh, sm] = startInput.value.split(':').map(Number);
            const [eh, em] = endInput.value.split(':').map(Number);
            const hours = ((eh * 60 + em) - (sh * 60 + sm)) / 60;

            if (hours <= 0) {
                priceDisplay.textContent = 'Invalid time range';
                priceDisplay.style.color = '#EF4444';
                return;
            }

            const total = priceType === 'Hourly' ? (pricePerUnit * hours).toFixed(2) : pricePerUnit.toFixed(2);
            priceDisplay.textContent = `₹${Math.round(total)}`;
            priceDisplay.style.color = '#4F46E5';
            const totalInput = document.getElementById('TotalAmount');
            if (totalInput) totalInput.value = total;
        }
    }

    if (startInput) startInput.addEventListener('change', updatePrice);
    if (endInput) endInput.addEventListener('change', updatePrice);
    updatePrice();
}

// ── Smooth Scroll for anchor links ──────────────────────────
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        const target = document.querySelector(this.getAttribute('href'));
        if (target) { e.preventDefault(); target.scrollIntoView({ behavior: 'smooth' }); }
    });
});

// ── Navbar scroll behavior ───────────────────────────────────
window.addEventListener('scroll', () => {
    const navbar = document.querySelector('.navbar-hub');
    if (navbar) {
        if (window.scrollY > 50) navbar.classList.add('scrolled');
        else navbar.classList.remove('scrolled');
    }
});

// ── Fade-in observer ─────────────────────────────────────────
const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.style.animationPlayState = 'running';
            observer.unobserve(entry.target);
        }
    });
}, { threshold: 0.1 });

document.querySelectorAll('.fade-in-up').forEach(el => {
    el.style.animationPlayState = 'paused';
    observer.observe(el);
});

// ── Delete confirmation ──────────────────────────────────────
document.querySelectorAll('[data-confirm]').forEach(el => {
    el.addEventListener('click', (e) => {
        if (!confirm(el.getAttribute('data-confirm'))) e.preventDefault();
    });
});

// ── Quick Search Modal (⌘K / Ctrl+K) ─────────────────────────
document.addEventListener('keydown', (e) => {
    if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
        e.preventDefault();
        const searchModalEl = document.getElementById('quickSearchModal');
        if (searchModalEl) {
            const modal = bootstrap.Modal.getOrCreateInstance(searchModalEl);
            modal.show();
        }
    }
});

// ── Global Image Error Fallback ──────────────────────────────
document.querySelectorAll('img').forEach(img => {
    img.addEventListener('error', function () {
        if (!this.getAttribute('data-has-fallback')) {
            this.setAttribute('data-has-fallback', 'true');
            this.src = '/images/default-service.svg';
        }
    });
});

// Init
initStarRating();
initBookingForm();
