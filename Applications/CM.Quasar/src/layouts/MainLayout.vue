<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated>
      <q-toolbar>
        <q-toolbar-title>Check Mate</q-toolbar-title>
        <q-btn flat round icon="logout" @click="logout">
          <q-tooltip>Logout</q-tooltip>
        </q-btn>
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useAuthStore } from 'src/stores/auth';

const auth = useAuthStore();
const router = useRouter();

function logout() {
  auth.logout();
  void router.push('/checklists');
}
</script>
