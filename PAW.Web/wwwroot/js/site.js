// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Reusable modal: Details / Add / Edit / Delete are partial views loaded into #crudModal (no new page).
(function () {
    const modalEl = document.getElementById('crudModal');
    const content = document.getElementById('crudModalContent');
    if (!modalEl || !content || typeof bootstrap === 'undefined') return;

    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    const ajaxHeaders = { 'X-Requested-With': 'XMLHttpRequest' };

    async function openModal(url) {
        try {
            const response = await fetch(url, { headers: ajaxHeaders });
            if (!response.ok) throw new Error('HTTP ' + response.status);
            content.innerHTML = await response.text();
            modal.show();
        } catch (error) {
            alert('The requested information could not be loaded.');
        }
    }

    // Links marked with data-modal open their partial view inside the modal.
    document.addEventListener('click', function (e) {
        const link = e.target.closest('a[data-modal]');
        if (!link) return;
        e.preventDefault();
        openModal(link.href);
    });

    // Forms marked with data-modal-form are posted with fetch.
    // Success -> JSON { success: true } and the list is reloaded; otherwise the partial is re-rendered with its errors.
    document.addEventListener('submit', async function (e) {
        const form = e.target.closest('form[data-modal-form]');
        if (!form) return;
        e.preventDefault();

        const submit = form.querySelector('[type="submit"]');
        if (submit) submit.disabled = true;

        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: ajaxHeaders
            });
            const type = response.headers.get('content-type') || '';

            if (type.includes('application/json')) {
                const result = await response.json();
                if (result.success) {
                    modal.hide();
                    window.location.reload();
                    return;
                }
            } else if (response.ok) {
                content.innerHTML = await response.text();
                return;
            }
            throw new Error('HTTP ' + response.status);
        } catch (error) {
            alert('The operation could not be completed.');
            if (submit) submit.disabled = false;
        }
    });
})();

/* ==========================================================================
   Exchange rate (Hacienda) - loaded from the browser, cached in localStorage
   ========================================================================== */

const EXCHANGE_RATE_URL = "https://api.hacienda.go.cr/indicadores/tc";
const EXCHANGE_RATE_CACHE_KEY = "paw.exchangeRate.v1";
const EXCHANGE_RATE_COLLAPSED_KEY = "paw.exchangeRate.collapsed";
// Hacienda publishes one rate per day, so one hour of cache is plenty.
const EXCHANGE_RATE_TTL_MS = 60 * 60 * 1000;

// Shared promise so several callers at the same time produce a single request.
let exchangeRateRequest = null;

function readExchangeRateCache() {
    try {
        const raw = localStorage.getItem(EXCHANGE_RATE_CACHE_KEY);
        if (!raw) return null;
        const parsed = JSON.parse(raw);
        return parsed && parsed.data && parsed.savedAt ? parsed : null;
    } catch {
        return null;
    }
}

function writeExchangeRateCache(data) {
    try {
        localStorage.setItem(EXCHANGE_RATE_CACHE_KEY, JSON.stringify({ savedAt: Date.now(), data }));
    } catch {
        // Storage full or disabled: the page keeps working, just without cache.
    }
}

/**
 * Returns a Promise<{ data, source, savedAt }>.
 *   source = "cache"        -> fresh value read from localStorage (no request made)
 *   source = "network"      -> value just fetched from the API
 *   source = "stale-cache"  -> API failed, expired cached value used instead
 */
function getExchangeRate(forceRefresh = false) {
    const cached = readExchangeRateCache();
    const isFresh = cached && (Date.now() - cached.savedAt) < EXCHANGE_RATE_TTL_MS;

    if (!forceRefresh && isFresh) {
        return Promise.resolve({ data: cached.data, source: "cache", savedAt: cached.savedAt });
    }

    if (exchangeRateRequest) {
        return exchangeRateRequest;
    }

    // No custom headers on purpose: keeps it a "simple" CORS request (no preflight).
    exchangeRateRequest = fetch(EXCHANGE_RATE_URL, { method: "GET", redirect: "follow" })
        .then((response) => {
            if (!response.ok) throw new Error("Exchange rate API returned HTTP " + response.status);
            return response.json();
        })
        .then((data) => {
            writeExchangeRateCache(data);
            return { data, source: "network", savedAt: Date.now() };
        })
        .catch((error) => {
            console.error("Could not load the exchange rate:", error);
            if (cached) {
                return { data: cached.data, source: "stale-cache", savedAt: cached.savedAt };
            }
            throw error;
        })
        .finally(() => {
            exchangeRateRequest = null;
        });

    return exchangeRateRequest;
}

function formatColones(value) {
    return new Intl.NumberFormat("es-CR", { style: "currency", currency: "CRC", minimumFractionDigits: 2 }).format(value);
}

function formatRateDate(value) {
    return value ? String(value).substring(0, 10) : "";
}

function renderExchangeRate(result) {
    const rate = result.data;
    const set = (id, text) => {
        const el = document.getElementById(id);
        if (el) el.textContent = text;
    };

    set("fxUsdBuy", formatColones(rate.dolar.compra.valor));
    set("fxUsdSell", formatColones(rate.dolar.venta.valor));
    set("fxUsdDate", formatRateDate(rate.dolar.venta.fecha));
    set("fxEurColones", formatColones(rate.euro.colones));
    set("fxEurDollars", "$" + Number(rate.euro.dolares).toFixed(4));
    set("fxEurDate", formatRateDate(rate.euro.fecha));

    const labels = {
        "cache": "Cached",
        "network": "Updated just now",
        "stale-cache": "Offline - last saved value"
    };
    set("fxStatus", labels[result.source] || "");

    const widget = document.getElementById("exchangeRateWidget");
    if (widget) {
        widget.classList.remove("is-loading", "is-error");
        widget.classList.toggle("is-stale", result.source === "stale-cache");
    }
}

function renderExchangeRateError() {
    const widget = document.getElementById("exchangeRateWidget");
    if (widget) {
        widget.classList.remove("is-loading");
        widget.classList.add("is-error");
    }
    const status = document.getElementById("fxStatus");
    if (status) status.textContent = "Exchange rate unavailable";
}

function loadExchangeRate(forceRefresh = false) {
    const widget = document.getElementById("exchangeRateWidget");
    if (!widget) return Promise.resolve();

    widget.classList.add("is-loading");
    return getExchangeRate(forceRefresh)
        .then(renderExchangeRate)
        .catch(renderExchangeRateError);
}

function initExchangeRateWidget() {
    const widget = document.getElementById("exchangeRateWidget");
    if (!widget) return;

    // Minimize / expand (remembered between pages)
    const toggle = document.getElementById("fxToggle");
    const applyCollapsed = (collapsed) => {
        widget.classList.toggle("is-collapsed", collapsed);
        if (toggle) {
            toggle.textContent = collapsed ? "+" : "\u2212";
            toggle.setAttribute("aria-expanded", String(!collapsed));
        }
    };
    let collapsed = false;
    try { collapsed = localStorage.getItem(EXCHANGE_RATE_COLLAPSED_KEY) === "1"; } catch { }
    applyCollapsed(collapsed);

    if (toggle) {
        toggle.addEventListener("click", () => {
            const next = !widget.classList.contains("is-collapsed");
            applyCollapsed(next);
            try { localStorage.setItem(EXCHANGE_RATE_COLLAPSED_KEY, next ? "1" : "0"); } catch { }
        });
    }

    const refresh = document.getElementById("fxRefresh");
    if (refresh) {
        refresh.addEventListener("click", () => loadExchangeRate(true));
    }

    loadExchangeRate();
}

document.addEventListener("DOMContentLoaded", initExchangeRateWidget);
