<template>
  <q-page padding>
    <div class="row items-center q-mb-md">
      <q-btn flat round icon="arrow_back" @click="goBack" />
      <div class="col row items-center q-ml-sm">
        <div class="text-h5">{{ checkListName }}</div>
        <q-btn round flat dense icon="delete" size="sm" class="q-ml-xs" @click="showDeleteDialog = true">
          <q-tooltip>Delete the list</q-tooltip>
        </q-btn>
      </div>
      <q-btn round flat icon="add" color="primary" @click="addNew">
        <q-tooltip>Add checkable</q-tooltip>
      </q-btn>
    </div>

    <q-dialog v-model="showDeleteDialog">
      <q-card style="min-width: 320px">
        <q-card-section>
          <div class="text-h6">Delete "{{ checkListName }}"?</div>
        </q-card-section>
        <q-card-section class="q-pt-none text-grey-6">
          This will permanently delete the list and all its items.
        </q-card-section>
        <q-card-actions align="right">
          <q-btn flat label="Cancel" v-close-popup />
          <q-btn flat label="Confirm Delete" color="negative" @click="deleteList" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <div v-if="uncheckedItems.length === 0" class="text-grey-6 q-mb-sm">No outstanding items.</div>

    <div class="q-gutter-sm">
      <q-banner
        v-for="item in uncheckedItems"
        :key="item.id"
        dense
        rounded
        inline-actions
        class="bg-grey-9"
      >
        <template #avatar>
          <q-btn
            v-if="!item.editing"
            round
            flat
            dense
            icon="check_circle"
            color="secondary"
            @click="checkItem(item)"
          />
          <div v-else style="width: 40px" />
        </template>

        <span v-if="!item.editing">{{ item.description }}</span>
        <q-input
          v-else
          v-model="item.editDescription"
          dense
          autofocus
          @blur="onDescriptionBlur(item)"
        />

        <template #action>
          <q-btn
            v-if="!item.editing"
            flat
            round
            dense
            icon="edit"
            @click="startEdit(item)"
          />
          <q-btn
            v-else
            flat
            round
            dense
            icon="save"
            :disable="!item.editDescription.trim()"
            @mousedown.prevent
            @click="saveItem(item)"
          />
        </template>
      </q-banner>
    </div>

    <div class="q-mt-md" v-if="checkedItems.length > 0">
      <q-expansion-item
        :label="`Checked (${checkedItems.length})`"
        dense
        header-class="text-grey-6"
      >
        <div class="q-gutter-sm q-mt-sm">
          <q-banner
            v-for="item in checkedItems"
            :key="item.id"
            dense
            rounded
            inline-actions
            :class="item.pendingDelete ? 'bg-negative' : 'bg-grey-9'"
            @click="item.pendingDelete = false"
          >
            <template #avatar>
              <q-btn
                round
                flat
                dense
                icon="check_circle"
                color="grey-6"
                :disable="item.pendingDelete"
                @click.stop="uncheckItem(item)"
              />
            </template>

            <span class="text-grey-6">{{ item.description }}</span>

            <template #action>
              <q-btn
                v-if="!item.pendingDelete"
                flat
                round
                dense
                icon="delete"
                @click.stop="item.pendingDelete = true"
              />
              <q-btn
                v-else
                outline
                color="white"
                label="Confirm delete"
                dense
                @mousedown.prevent
                @click.stop="deleteItem(item)"
              />
            </template>
          </q-banner>
        </div>
      </q-expansion-item>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from 'src/stores/auth';
import { useCheckablesStore } from 'src/stores/checkables';
import { useCheckListsStore } from 'src/stores/checkLists';
import { postQuery } from 'src/api';
import type { CheckListDto } from 'src/types/CheckListDto';

interface EditableCheckable {
  id: string;
  checked: boolean;
  checkListId: string;
  description: string;
  userId: string;
  created: string;
  editing: boolean;
  isNew: boolean;
  editDescription: string;
  pendingDelete: boolean;
}

const route = useRoute();
const router = useRouter();
const auth = useAuthStore();
const checkablesStore = useCheckablesStore();
const checkListsStore = useCheckListsStore();

const checkListId = route.params['id'] as string;
const checkListName = ref('');
const editableItems = ref<EditableCheckable[]>([]);
const showDeleteDialog = ref(false);

const uncheckedItems = computed(() =>
  editableItems.value
    .filter((i) => !i.checked)
    .sort((a, b) => {
      if (a.isNew) return -1;
      if (b.isNew) return 1;
      return a.description.localeCompare(b.description);
    }),
);

const checkedItems = computed(() =>
  editableItems.value
    .filter((i) => i.checked)
    .sort((a, b) => a.description.localeCompare(b.description)),
);

onMounted(async () => {
  try {
    const list = await postQuery<CheckListDto>('GetCheckList', { checkListId });
    checkListName.value = list.name;
  } catch {
    // ignore — name stays empty
  }
  await loadCheckables();
});

async function loadCheckables() {
  await checkablesStore.loadByCheckList(checkListId);
  editableItems.value = checkablesStore.checkables.map((c) => ({
    ...c,
    editing: false,
    isNew: false,
    editDescription: c.description,
    pendingDelete: false,
  }));
}

function goBack() {
  void router.push('/checklists');
}

async function deleteList() {
  await checkListsStore.deleteCheckList(checkListId);
  void router.push('/checklists');
}

function addNew() {
  editableItems.value.unshift({
    id: crypto.randomUUID(),
    checked: false,
    checkListId,
    description: '',
    userId: auth.userId ?? '',
    created: new Date().toISOString(),
    editing: true,
    isNew: true,
    editDescription: '',
  });
}

function startEdit(item: EditableCheckable) {
  item.editing = true;
  item.editDescription = item.description;
}

async function saveItem(item: EditableCheckable) {
  if (!item.editDescription.trim()) return;
  if (item.isNew) {
    await checkablesStore.create(checkListId, auth.userId ?? '', item.editDescription.trim());
    editableItems.value = editableItems.value.filter((i) => i.id !== item.id);
    await loadCheckables();
  } else {
    await checkablesStore.updateDescription(item.id, item.editDescription.trim());
    item.description = item.editDescription.trim();
    item.editing = false;
  }
}

function cancelEdit(item: EditableCheckable) {
  if (item.isNew) {
    editableItems.value = editableItems.value.filter((i) => i.id !== item.id);
  } else {
    item.editDescription = item.description;
    item.editing = false;
  }
}

function onDescriptionBlur(item: EditableCheckable) {
  cancelEdit(item);
}

async function checkItem(item: EditableCheckable) {
  await checkablesStore.check(item.id);
  item.checked = true;
}

async function uncheckItem(item: EditableCheckable) {
  await checkablesStore.uncheck(item.id);
  item.checked = false;
}

async function deleteItem(item: EditableCheckable) {
  await checkablesStore.deleteCheckable(item.id);
  editableItems.value = editableItems.value.filter((i) => i.id !== item.id);
}
</script>
