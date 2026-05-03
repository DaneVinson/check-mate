<template>
  <router-view />

  <q-dialog v-model="showAuthDialog" persistent>
    <q-card style="min-width: 360px">
      <q-card-section>
        <div class="text-h6">Welcome to Check Mate</div>
      </q-card-section>
      <q-card-section class="q-pt-none">
        <q-input v-model="name" label="Name" class="q-mb-sm" autofocus @keyup.enter="login" />
        <q-input v-model="email" label="Email" type="email" class="q-mb-sm" @keyup.enter="login" />
        <div v-if="error" class="text-negative text-caption q-mt-xs">{{ error }}</div>
      </q-card-section>
      <q-card-actions>
        <q-btn flat label="Login" color="primary" :loading="loading" @click="login" />
        <q-space />
        <q-btn flat label="Create Account" color="secondary" :loading="loading" @click="createAccount" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useAuthStore } from 'src/stores/auth';

const auth = useAuthStore();
auth.initialize();

const showAuthDialog = computed(() => !auth.isAuthenticated);
const email = ref('');
const name = ref('');
const error = ref('');
const loading = ref(false);

async function login() {
  error.value = '';
  loading.value = true;
  try {
    await auth.login(email.value, name.value);
  } catch {
    error.value = 'Login failed. Please check your credentials.';
  } finally {
    loading.value = false;
  }
}

async function createAccount() {
  error.value = '';
  loading.value = true;
  try {
    await auth.createAccount(email.value, name.value);
  } catch {
    error.value = 'Could not create account. Please try again.';
  } finally {
    loading.value = false;
  }
}
</script>
