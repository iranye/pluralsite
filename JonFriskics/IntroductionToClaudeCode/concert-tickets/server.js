const express = require('express');
const path = require('path');
const { DatabaseSync } = require('node:sqlite'); // built into Node 22.5+, no native build needed

const app = express();
const PORT = process.env.PORT || 3000;
const db = new DatabaseSync(path.join(__dirname, 'tickets.db'));

// ---------- Database setup + seed ----------
db.exec(`
  CREATE TABLE IF NOT EXISTS events (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    artist TEXT NOT NULL,
    venue TEXT NOT NULL,
    city TEXT NOT NULL,
    event_date TEXT NOT NULL,
    price REAL NOT NULL,
    description TEXT NOT NULL
  );
  CREATE TABLE IF NOT EXISTS cart_items (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    cart_id TEXT NOT NULL,
    event_id INTEGER NOT NULL REFERENCES events(id),
    quantity INTEGER NOT NULL DEFAULT 1,
    UNIQUE(cart_id, event_id)
  );
`);

if (db.prepare('SELECT COUNT(*) AS n FROM events').get().n === 0) {
  const seed = [
    ['The Velvet Static', 'Neon Harbor Arena', 'Port Alder', '2026-12-05T20:00', 89.5, 'Dream-pop shimmer meets razor-sharp synths on their sold-out Afterglow tour.'],
    ['Crimson Lanterns', 'The Gilded Moth Theater', 'Marlow Falls', '2026-12-12T19:30', 64.0, 'Folk-rock storytellers performing their full "Paper Moons" album front to back.'],
    ['DJ Okapi', 'Subterra Warehouse', 'Brickton', '2026-12-19T22:00', 45.0, 'A six-hour deep house marathon with visuals by the Lumen Collective.'],
    ['Marigold & The Fuses', 'Copperline Amphitheater', 'Sunder Bay', '2027-01-09T18:00', 72.25, 'High-energy indie anthems under the winter stars. Bring a blanket.'],
    ['Idle Giants', 'Foghorn Hall', 'Port Alder', '2027-01-23T20:00', 55.0, 'Heavy, cinematic post-rock. Expect walls of sound and very few words.'],
    ['Luna Quartz', 'The Rusty Compass', 'Marlow Falls', '2027-02-06T21:00', 38.0, 'Intimate late-night set of soulful acoustic songs in a 200-seat room.'],
    ['Thunder Biscuit', 'Hollow Pine Pavilion', 'Cedar Reach', '2027-02-20T19:00', 59.99, 'Southern-fried blues rock with a horn section that does not quit.'],
    ['Echo Parade', 'Neon Harbor Arena', 'Port Alder', '2027-03-13T20:30', 95.0, 'Electro-pop spectacle: confetti cannons, choreography, and a 12-piece band.'],
  ];
  const ins = db.prepare('INSERT INTO events (artist, venue, city, event_date, price, description) VALUES (?,?,?,?,?,?)');
  seed.forEach(r => ins.run(...r));
}

// ---------- Cookie helpers (no extra deps) ----------
// The cart lives in a cookie named "cart" holding JSON like [{"id":1,"qty":2}]
function parseCookies(header = '') {
  return Object.fromEntries(
    header.split(';').map(c => c.trim()).filter(Boolean).map(c => {
      const i = c.indexOf('=');
      return [c.slice(0, i), decodeURIComponent(c.slice(i + 1))];
    })
  );
}

function readCart(req) {
  try {
    const cart = JSON.parse(parseCookies(req.headers.cookie).cart || '[]');
    return Array.isArray(cart)
      ? cart.filter(i => Number.isInteger(i.id) && Number.isInteger(i.qty) && i.qty > 0)
      : [];
  } catch {
    return [];
  }
}

function writeCart(res, cart) {
  const value = encodeURIComponent(JSON.stringify(cart));
  res.append('Set-Cookie', `cart=${value}; Path=/; Max-Age=${60 * 60 * 24 * 7}; SameSite=Lax`);
}

// Mirror the cookie cart into SQLite (keyed by a visitor id cookie) so it's also stored server-side.
function syncCartToDb(req, res, cart) {
  let cartId = parseCookies(req.headers.cookie).cart_id;
  if (!cartId) {
    cartId = Math.random().toString(36).slice(2) + Date.now().toString(36);
    res.append('Set-Cookie', `cart_id=${cartId}; Path=/; Max-Age=${60 * 60 * 24 * 365}; SameSite=Lax; HttpOnly`);
  }
  db.prepare('DELETE FROM cart_items WHERE cart_id = ?').run(cartId);
  const ins = db.prepare('INSERT INTO cart_items (cart_id, event_id, quantity) VALUES (?,?,?)');
  cart.forEach(i => ins.run(cartId, i.id, i.qty));
}

function hydrateCart(cart) {
  const get = db.prepare('SELECT * FROM events WHERE id = ?');
  const items = cart
    .map(i => {
      const e = get.get(i.id);
      return e ? { ...e, quantity: i.qty, subtotal: +(e.price * i.qty).toFixed(2) } : null;
    })
    .filter(Boolean);
  return {
    items,
    count: items.reduce((n, i) => n + i.quantity, 0),
    total: +items.reduce((t, i) => t + i.subtotal, 0).toFixed(2),
  };
}

// ---------- API ----------
app.use(express.json());

app.get('/api/events', (req, res) => {
  res.json(db.prepare('SELECT * FROM events ORDER BY event_date').all());
});

app.get('/api/events/:id', (req, res) => {
  const event = db.prepare('SELECT * FROM events WHERE id = ?').get(Number(req.params.id));
  if (!event) return res.status(404).json({ error: 'Event not found' });
  res.json(event);
});

app.get('/api/cart', (req, res) => {
  res.json(hydrateCart(readCart(req)));
});

app.post('/api/cart', (req, res) => {
  const id = Number(req.body.eventId);
  const qty = Math.min(Math.max(parseInt(req.body.quantity, 10) || 1, 1), 10);
  if (!db.prepare('SELECT 1 FROM events WHERE id = ?').get(id)) {
    return res.status(404).json({ error: 'Event not found' });
  }
  const cart = readCart(req);
  const existing = cart.find(i => i.id === id);
  if (existing) existing.qty = Math.min(existing.qty + qty, 10);
  else cart.push({ id, qty });
  writeCart(res, cart);
  syncCartToDb(req, res, cart);
  res.json(hydrateCart(cart));
});

app.delete('/api/cart/:id', (req, res) => {
  const id = Number(req.params.id);
  const cart = readCart(req).filter(i => i.id !== id);
  writeCart(res, cart);
  syncCartToDb(req, res, cart);
  res.json(hydrateCart(cart));
});

// ---------- Static pages ----------
app.use(express.static(path.join(__dirname, 'public')));

app.listen(PORT, () => console.log(`Concert tickets running at http://localhost:${PORT}`));
