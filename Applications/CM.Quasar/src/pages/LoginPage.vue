<template>
  <q-page class="flex flex-center column q-pa-lg">
    <div class="text-h4 q-mb-lg">Check Mate</div>
    <q-card style="min-width: 320px">
      <q-card-section>
        <div class="text-h6">Sign In</div>
      </q-card-section>
      <q-card-section class="q-pt-none">
        <q-input v-model="email" label="Email" type="email" class="q-mb-sm" />
        <q-input v-model="name" label="Name" class="q-mb-sm" />
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Sign In" color="primary" @click="signIn" />
      </q-card-actions>
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const email = ref('');
const name = ref('');

function signIn() {
  // TODO: call POST /auth/token via auth store
  void router.push('/checklists');
}
</script>
