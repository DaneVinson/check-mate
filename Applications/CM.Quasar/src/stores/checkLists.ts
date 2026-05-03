import { defineStore } from 'pinia';
import { postCommand, postQuery } from 'src/api';
import type { CheckListDto } from 'src/types/CheckListDto';

export const useCheckListsStore = defineStore('checkLists', {
  state: () => ({
    checkLists: [] as CheckListDto[],
  }),

  actions: {
    async load(userId: string): Promise<void> {
      this.checkLists = await postQuery<CheckListDto[]>('GetCheckListsByUser', { userId });
    },

    async create(name: string, userId: string): Promise<void> {
      await postCommand('CreateCheckList', { name, userId });
      await this.load(userId);
    },

    async updateName(checkListId: string, name: string): Promise<void> {
      await postCommand('UpdateCheckList', { checkListId, name });
      const item = this.checkLists.find((c) => c.id === checkListId);
      if (item) item.name = name;
    },

    async deleteCheckList(checkListId: string): Promise<void> {
      await postCommand('DeleteCheckList', { checkListId });
      this.checkLists = this.checkLists.filter((c) => c.id !== checkListId);
    },
  },
});
