// Shared helpers used by all pages
const fmtPrice = n => '$' + Number(n).toFixed(2);
const fmtDate = s =>
  new Date(s).toLocaleString(undefined, {
    weekday: 'short', month: 'short', day: 'numeric', year: 'numeric',
    hour: 'numeric', minute: '2-digit',
  });
// Placeholder image service; the seed (event id) keeps each image stable.
const imgUrl = (id, w, h) => `https://picsum.photos/seed/concert${id}/${w}/${h}`;
const esc = s => String(s).replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));

async function api(url, options) {
  const res = await fetch(url, options);
  if (!res.ok) throw new Error((await res.json().catch(() => ({}))).error || res.statusText);
  return res.json();
}

function updateBadge(count) {
  const el = document.getElementById('cart-count');
  if (el) el.textContent = count;
}

async function refreshBadge() {
  try { updateBadge((await api('/api/cart')).count); } catch { /* ignore */ }
}

// ---- Page: list ----
async function initList() {
  const grid = document.getElementById('events');
  const events = await api('/api/events');
  grid.innerHTML = events.map(e => `
    <a class="card" href="/event.html?id=${e.id}">
      <img src="${imgUrl(e.id, 400, 240)}" alt="${esc(e.artist)}" loading="lazy">
      <div class="body">
        <h3>${esc(e.artist)}</h3>
        <div class="muted">${esc(e.venue)} &middot; ${esc(e.city)}</div>
        <div class="muted">${fmtDate(e.event_date)}</div>
        <div class="price">${fmtPrice(e.price)}</div>
      </div>
    </a>`).join('');
}

// ---- Page: details ----
async function initDetail() {
  const id = new URLSearchParams(location.search).get('id');
  const box = document.getElementById('detail');
  let e;
  try { e = await api('/api/events/' + encodeURIComponent(id)); }
  catch { box.innerHTML = '<p class="empty">Event not found.</p>'; return; }
  document.title = `${e.artist} – Tickets`;
  box.innerHTML = `
    <img src="${imgUrl(e.id, 700, 500)}" alt="${esc(e.artist)}">
    <div>
      <h1>${esc(e.artist)}</h1>
      <p class="muted">${esc(e.description)}</p>
      <ul class="facts">
        <li><strong>Venue</strong> ${esc(e.venue)}, ${esc(e.city)}</li>
        <li><strong>Date</strong> ${fmtDate(e.event_date)}</li>
        <li><strong>Price</strong> <span class="price">${fmtPrice(e.price)}</span> each</li>
      </ul>
      <div class="qty">
        <label for="qty">Quantity</label>
        <select id="qty">${[1,2,3,4,5,6,7,8,9,10].map(n => `<option>${n}</option>`).join('')}</select>
      </div>
      <button class="btn" id="add">Add to Cart</button>
      <div class="notice" id="notice"></div>
    </div>`;
  document.getElementById('add').addEventListener('click', async () => {
    const quantity = Number(document.getElementById('qty').value);
    const cart = await api('/api/cart', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ eventId: e.id, quantity }),
    });
    updateBadge(cart.count);
    document.getElementById('notice').innerHTML =
      `Added ${quantity} ticket(s)! <a href="/cart.html" style="text-decoration:underline">View cart</a>`;
  });
}

// ---- Page: cart ----
async function initCart() {
  const box = document.getElementById('cart');
  async function render(cart) {
    updateBadge(cart.count);
    if (!cart.items.length) {
      box.innerHTML = '<p class="empty">Your cart is empty. <a href="/" style="text-decoration:underline">Browse concerts</a></p>';
      return;
    }
    box.innerHTML = `
      <table class="cart">
        <thead><tr><th>Event</th><th>Date</th><th>Price</th><th>Qty</th><th>Subtotal</th><th></th></tr></thead>
        <tbody>${cart.items.map(i => `
          <tr>
            <td><img src="${imgUrl(i.id, 160, 100)}" alt=""><a href="/event.html?id=${i.id}">${esc(i.artist)}</a>
              <div class="muted">${esc(i.venue)}</div></td>
            <td>${fmtDate(i.event_date)}</td>
            <td>${fmtPrice(i.price)}</td>
            <td>${i.quantity}</td>
            <td>${fmtPrice(i.subtotal)}</td>
            <td><button class="btn small" data-remove="${i.id}">Remove</button></td>
          </tr>`).join('')}
        </tbody>
      </table>
      <div class="total">Total: <strong>${fmtPrice(cart.total)}</strong></div>
      <p class="muted" style="text-align:right">Prototype only – checkout is not implemented.</p>`;
    box.querySelectorAll('[data-remove]').forEach(b =>
      b.addEventListener('click', async () =>
        render(await api('/api/cart/' + b.dataset.remove, { method: 'DELETE' }))));
  }
  render(await api('/api/cart'));
}

document.addEventListener('DOMContentLoaded', () => {
  const page = document.body.dataset.page;
  if (page !== 'cart') refreshBadge();
  if (page === 'list') initList();
  if (page === 'detail') initDetail();
  if (page === 'cart') initCart();
});
