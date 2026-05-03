import { defineStore } from 'pinia';
import { postAuth } from 'src/api';
import { useCheckListsStore } from 'src/stores/checkLists';
import { useCheckablesStore } from 'src/stores/checkables';

function decodeJwtPayload(token: string): Record<string, unknown> {
  const part = token.split('.')[1] ?? '';
  const base64 = part.replace(/-/g, '+').replace(/_/g, '/');
  return JSON.parse(atob(base64)) as Record<string, unknown>;
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    accessToken: null as string | null,
    expiresAt: null as number | null,
    userId: null as string | null,
    userName: null as string | null,
  }),

  getters: {
    isAuthenticated: (state) => !!state.accessToken && Date.now() < (state.expiresAt ?? 0),
  },

  actions: {
    initialize() {
      const token = localStorage.getItem('cm_access_token');
      const expiresAt = Number(localStorage.getItem('cm_expires_at') ?? '0');
      if (token && Date.now() < expiresAt) {
        this.accessToken = token;
        this.expiresAt = expiresAt;
        const payload = decodeJwtPayload(token);
        this.userId = (payload['userId'] as string | undefined) ?? (payload['sub'] as string | undefined) ?? null;
        this.userName = (payload['name'] as string | undefined) ?? null;
      }
    },

    async login(email: string, name: string): Promise<void> {
      const result = await postAuth(email, name);
      this._storeToken(result.accessToken, result.expiresIn);
    },

    async createAccount(email: string, name: string): Promise<void> {
      await fetch('/commands', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ type: 'CreateUser', payload: { email, name } }),
      });
      await this.login(email, name);
    },

    logout() {
      this.accessToken = null;
      this.expiresAt = null;
      this.userId = null;
      this.userName = null;
      localStorage.removeItem('cm_access_token');
      localStorage.removeItem('cm_expires_at');
      useCheckListsStore().$reset();
      useCheckablesStore().$reset();
    },

    _storeToken(token: string, expiresIn: number) {
      const expiresAt = Date.now() + expiresIn * 1000;
      this.accessToken = token;
      this.expiresAt = expiresAt;
      const payload = decodeJwtPayload(token);
      this.userId = (payload['userId'] as string | undefined) ?? (payload['sub'] as string | undefined) ?? null;
      this.userName = (payload['name'] as string | undefined) ?? null;
      localStorage.setItem('cm_access_token', token);
      localStorage.setItem('cm_expires_at', String(expiresAt));
    },
  },
});
