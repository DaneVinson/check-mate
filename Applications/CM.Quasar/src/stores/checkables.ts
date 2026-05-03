import { defineStore } from 'pinia';
import { postCommand, postQuery } from 'src/api';
import type { CheckableDto } from 'src/types/CheckableDto';

export const useCheckablesStore = defineStore('checkables', {
  state: () => ({
    checkables: [] as CheckableDto[],
  }),

  actions: {
    async loadByCheckList(checkListId: string): Promise<void> {
      this.checkables = await postQuery<CheckableDto[]>('GetCheckablesByCheckList', { checkListId });
    },

    async check(id: string): Promise<void> {
      await postCommand('CheckCheckable', { checkableId: id });
      const item = this.checkables.find((c) => c.id === id);
      if (item) item.checked = true;
    },

    async uncheck(id: string): Promise<void> {
      await postCommand('UncheckCheckable', { checkableId: id });
      const item = this.checkables.find((c) => c.id === id);
      if (item) item.checked = false;
    },

    async create(checkListId: string, userId: string, description: string): Promise<void> {
      await postCommand('CreateCheckable', { checkListId, description, userId });
      await this.loadByCheckList(checkListId);
    },

    async updateDescription(id: string, description: string): Promise<void> {
      await postCommand('UpdateCheckable', { checkableId: id, description });
      const item = this.checkables.find((c) => c.id === id);
      if (item) item.description = description;
    },

    async deleteCheckable(id: string): Promise<void> {
      await postCommand('DeleteCheckable', { checkableId: id });
      this.checkables = this.checkables.filter((c) => c.id !== id);
    },
  },
});
