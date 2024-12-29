<template>
  <AppPageWrapper :maxWidth="1600">
    <div class="quote-page">
      <QuoteLogin class="login" :onSubmit="onLogin" />
      <QuoteCreate v-if="user" :onSubmit="onQuote" />
      <QuoteList class="fg-1" v-if="user" :accountId="user.accountId" />
      <DxButton v-if="user" text="Refresh" type="success" @click="onReload" />
      <QuoteInventory v-if="user" ref="quoteInventory" :accountId="user.accountId" />
    </div>
  </AppPageWrapper>
</template>

<script setup lang="ts">
import QuoteLogin, { type QuoteLoginModel } from './components/quote-login.vue';
import AppPageWrapper from '@/components/wrappers/app-page-wrapper.vue';
import { DxButton } from 'devextreme-vue/button';
import { ref } from 'vue';
import QuoteCreate, { type QuoteCreateModel } from './components/quote-create.vue';
import QuoteInventory from './components/quote-inventory.vue';
import QuoteList from './components/quote-list.vue';
import { quoteService } from '@/shared/services/api/quote-service';
import { v4 as uuid } from 'uuid';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import { DateFormats } from '@/constants';
dayjs.extend(utc);

const quoteInventory = ref<InstanceType<typeof QuoteInventory> | null>(null);
const user = ref<QuoteLoginModel | null>(null);

const onLogin = (info: QuoteLoginModel | null) => {
  user.value = info;
};

const onQuote = async (info: QuoteCreateModel) => {
  const now = dayjs.utc();
  await quoteService.quote({ ...info, ...user.value!, ...{ id: uuid(), requestType: 'QuoteRequest', time: now.format(DateFormats.DAYJS_DATETIME) } });

  onReload();
};

function onReload() {
  quoteInventory.value?.reload();
}

</script>

<style lang="scss">
.quote-page {
  width: 100%;
  display: flex;
  flex-direction: column;
  flex-grow: 1;
  gap: 15px;

  .login {
    margin: -5px;
  }
}

.dx-texteditor.dx-editor-filled.dx-state-disabled .dx-texteditor-input,
.dx-texteditor.dx-editor-filled.dx-state-readonly .dx-texteditor-input,
.dx-texteditor.dx-editor-filled.dx-state-readonly.dx-state-hover .dx-texteditor-input {
  color: rgb(229, 230, 231);
}
</style>
