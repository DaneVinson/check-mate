<template>
  <q-page padding>
    <div class="row items-center q-mb-md">
      <div class="text-h5 col">{{ auth.userName ? `${auth.userName}${auth.userName.endsWith('s') ? '\'' : '\'s'} Lists` : 'Check Lists' }}</div>
      <q-btn round flat icon="add" color="primary" @click="addNew">
        <q-tooltip>New checkable list</q-tooltip>
      </q-btn>
    </div>

    <div v-if="editableLists.length > 0" class="q-gutter-sm">
      <q-banner
        v-for="list in editableLists"
        :key="list.id"
        dense
        rounded
        inline-actions
        :class="['bg-grey-9', !list.editing && 'cursor-pointer']"
        @click="!list.editing && router.push(`/checklists/${list.id}`)"
      >
        <span v-if="!list.editing">{{ list.name }}</span>
        <q-input
          v-else
          v-model="list.editName"
          dense
          autofocus
          @blur="onNameBlur(list)"
          @click.stop
        />
        <template #action>
          <q-btn
            v-if="!list.editing"
            flat
            round
            dense
            icon="edit"
            @click.stop="startEdit(list)"
          />
          <q-btn
            v-else
            flat
            round
            dense
            icon="save"
            :disable="!list.editName.trim()"
            @mousedown.prevent
            @click.stop="saveList(list)"
          />
        </template>
      </q-banner>
    </div>

    <div v-else class="text-grey-6 q-mt-lg text-center">No check lists yet.</div>
  </q-page>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from 'src/stores/auth';
import { useCheckListsStore } from 'src/stores/checkLists';
import type { CheckListDto } from 'src/types/CheckListDto';

interface EditableCheckList extends CheckListDto {
  editing: boolean;
  isNew: boolean;
  editName: string;
}

const auth = useAuthStore();
const router = useRouter();
const store = useCheckListsStore();

const editableLists = ref<EditableCheckList[]>([]);

onMounted(async () => {
  if (auth.userId) {
    await store.load(auth.userId);
    editableLists.value = store.checkLists.map((l) => ({
      ...l,
      editing: false,
      isNew: false,
      editName: '',
    }));
  }
});

watch(
  () => auth.isAuthenticated,
  async (authenticated) => {
    if (!authenticated) {
      editableLists.value = [];
    } else if (auth.userId) {
      await store.load(auth.userId);
      editableLists.value = store.checkLists.map((l) => ({
        ...l,
        editing: false,
        isNew: false,
        editName: '',
      }));
    }
  },
);

function addNew() {
  editableLists.value.unshift({
    id: crypto.randomUUID(),
    name: '',
    userId: auth.userId ?? '',
    created: new Date().toISOString(),
    editing: true,
    isNew: true,
    editName: '',
  });
}

function startEdit(list: EditableCheckList) {
  list.editName = list.name;
  list.editing = true;
}

function onNameBlur(list: EditableCheckList) {
  if (list.isNew) {
    editableLists.value = editableLists.value.filter((l) => l.id !== list.id);
  } else {
    list.editing = false;
  }
}

async function saveList(list: EditableCheckList) {
  if (!list.editName.trim() || !auth.userId) return;
  if (list.isNew) {
    await store.create(list.editName.trim(), auth.userId);
    editableLists.value = editableLists.value.filter((l) => l.id !== list.id);
    editableLists.value = [
      ...store.checkLists.map((l) => ({
        ...l,
        editing: false,
        isNew: false,
        editName: '',
      })),
    ];
  } else {
    await store.updateName(list.id, list.editName.trim());
    list.name = list.editName.trim();
    list.editing = false;
  }
}
</script>
