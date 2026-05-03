import { useAuthStore } from 'src/stores/auth';

async function apiFetch(path: string, body: unknown): Promise<Response> {
  const auth = useAuthStore();
  const headers: Record<string, string> = { 'Content-Type': 'application/json' };
  if (auth.accessToken) {
    headers['Authorization'] = `Bearer ${auth.accessToken}`;
  }
  return fetch(path, { method: 'POST', headers, body: JSON.stringify(body) });
}

export async function postCommand(type: string, payload: Record<string, unknown>): Promise<void> {
  await apiFetch('/commands', { type, payload });
}

export async function postQuery<T>(type: string, payload: Record<string, unknown>): Promise<T> {
  const response = await apiFetch('/queries', { type, payload });
  if (!response.ok) throw new Error(`Query failed: ${response.status}`);
  return response.json() as Promise<T>;
}

export async function postAuth(
  email: string,
  name: string,
): Promise<{ accessToken: string; expiresIn: number }> {
  const response = await fetch('/auth/token', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, name }),
  });
  if (!response.ok) throw new Error('Invalid credentials');
  return response.json() as Promise<{ accessToken: string; expiresIn: number }>;
}
