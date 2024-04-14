<script setup>
import { useStore } from 'vuex';
import { RouterLink, RouterView, useRouter } from 'vue-router';
import { computed } from 'vue';

const store = useStore();
const router = useRouter();
const isAuthenticated = computed(() => store.getters.isAuthenticated);

const logout = async () => {
  await store.dispatch('logout');
  await router.push('/login');
};
</script>

<template>
  <div>
    <nav>
      <RouterLink to="/">Home</RouterLink>
      <span v-if="isAuthenticated"> | </span>
      <RouterLink to="/documents" v-if="isAuthenticated">Documents</RouterLink>
      <span v-if="isAuthenticated && !isLoginOrRegisterRoute"> | </span>
      <RouterLink to="/user" v-if="isAuthenticated">User</RouterLink>
      <span v-if="isAuthenticated"> | </span>
      <a href="#" v-if="isAuthenticated" @click.prevent="logout">Logout</a>
      <span v-if="!isAuthenticated"> | </span>
      <RouterLink to="/login" v-if="!isAuthenticated">Login</RouterLink>
      <span v-if="!isAuthenticated"> | </span>
      <RouterLink to="/register" v-if="!isAuthenticated">Register</RouterLink>
    </nav>
    <RouterView/>
  </div>
</template>

<style scoped>
nav span {
  color: #fff; /* adjust your color for the separator */
}
</style>
