import { defineStore } from 'pinia';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    accessToken: null as string | null,
    expiresAt: null as number | null,
  }),

  getters: {
    isAuthenticated: (state) => !!state.accessToken && Date.now() < (state.expiresAt ?? 0),
  },

  actions: {
    // TODO: call POST /auth/token
    async login(_email: string, _name: string): Promise<void> {
      // placeholder
    },

    logout() {
      this.accessToken = null;
      this.expiresAt = null;
    },
  },
});
