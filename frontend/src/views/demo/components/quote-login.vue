<template>
  <div class="quote-login">
    <DxTextBox v-model:value="model.accountId" :readOnly="isSignedIn" valueChangeEvent="input"
      placeholder="Account Id" />
    <DxButton v-if="!isSignedIn" :disabled="isInvalid" text="Login" type="success" @click="onLogin" />
    <DxButton v-if="isSignedIn" text="Logout" type="success" @click="onLogout" />
  </div>
</template>
<script setup lang="ts">
import { DxButton } from 'devextreme-vue/button';
import { DxTextBox } from 'devextreme-vue/text-box';
import { computed, ref } from 'vue';

export interface QuoteLoginModel {
  accountId: string;
}

const props = defineProps<{ onSubmit: (x: QuoteLoginModel | null) => void }>();
const isSignedIn = ref(false);
const model = ref({} as QuoteLoginModel);
const isInvalid = computed(() => !model.value.accountId);

const onLogin = () => {
  isSignedIn.value = true;
  props.onSubmit(model.value);
};

const onLogout = () => {
  isSignedIn.value = false;
  props.onSubmit(null);
};

</script>
<style lang="scss">
.quote-login {
  display: flex;

  .dx-dropdowneditor {
    width: 262px;
  }

  >* {
    margin: 5px;
  }
}
</style>
