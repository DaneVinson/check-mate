import { defineStore } from 'pinia';
import type { CheckListDto } from 'src/types/CheckListDto';

export const useCheckListsStore = defineStore('checkLists', {
  state: () => ({
    checkLists: [] as CheckListDto[],
  }),

  actions: {
    // TODO: call POST /queries { type: 'GetCheckListsByUser', payload: { userId } }
    async load(_userId: string): Promise<void> {
      // placeholder
    },
  },
});
