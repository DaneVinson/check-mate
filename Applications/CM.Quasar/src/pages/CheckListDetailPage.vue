<template>
  <q-page padding>
    <div class="text-h5 q-mb-md">{{ checkList?.name }}</div>
    <q-list bordered separator>
      <q-item v-for="item in checkables" :key="item.id">
        <q-item-section avatar>
          <q-checkbox v-model="item.checked" />
        </q-item-section>
        <q-item-section>
          <q-item-label>{{ item.description }}</q-item-label>
        </q-item-section>
      </q-item>
    </q-list>
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { CheckListDto } from 'src/types/CheckListDto';
import type { CheckableDto } from 'src/types/CheckableDto';

const checkList = ref<CheckListDto | null>(null);
const checkables = ref<CheckableDto[]>([]);

// TODO: load via checkLists + checkables stores
</script>
