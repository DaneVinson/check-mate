import { defineStore } from 'pinia';
import type { CheckableDto } from 'src/types/CheckableDto';

export const useCheckablesStore = defineStore('checkables', {
  state: () => ({
    checkables: [] as CheckableDto[],
  }),

  actions: {
    // TODO: call POST /queries { type: 'GetCheckablesByCheckList', payload: { checkListId } }
    async loadByCheckList(_checkListId: string): Promise<void> {
      // placeholder
    },

    // TODO: call POST /commands { type: 'CheckCheckable', payload: { id } }
    async check(_id: string): Promise<void> {
      // placeholder
    },

    // TODO: call POST /commands { type: 'UncheckCheckable', payload: { id } }
    async uncheck(_id: string): Promise<void> {
      // placeholder
    },
  },
});
