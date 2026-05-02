<template>
  <q-page padding>
    <div class="text-h5 q-mb-md">Check Lists</div>
    <q-list bordered separator>
      <q-item v-for="list in checkLists" :key="list.id" clickable v-ripple :to="`/checklists/${list.id}`">
        <q-item-section>
          <q-item-label>{{ list.name }}</q-item-label>
        </q-item-section>
      </q-item>
    </q-list>
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { CheckListDto } from 'src/types/CheckListDto';

const checkLists = ref<CheckListDto[]>([]);

// TODO: load via checkLists store
</script>
