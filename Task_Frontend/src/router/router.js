// router.js
import { createRouter, createWebHistory } from 'vue-router';
import Documents from '@/components/Documents.vue';
import Login from "@/components/Login.vue";
import Register from "@/components/Register.vue";
import User from "@/components/User.vue";
import AuthStore from '@/stores/AuthStore.js';
import Home from "@/components/Home.vue";

const routes = [
  {
    path: '/documents',
    name: 'Documents',
    component: Documents,
    meta: { requiresAuth: true } // dodajemy meta dane wskazujące, że ta trasa wymaga autoryzacji
  },

  {
    path: '/login',
    name: 'Login',
    component: Login,
    meta: { requiresGuest: true } // dodajemy meta dane wskazujące, że na tą trasę mogą wejść niezalogowani użytkownicy
  },

  {
    path: '/register',
    name: 'Register',
    component: Register,
    meta: { requiresGuest: true }
  },

  {
    path: '/user',
    name: 'User',
    component: User,
    meta: { requiresAuth: true }
  },

  {
    path: '/',
    name: 'Home',
    component: Home
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Navigation guards
router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
  const requiresGuest = to.matched.some(record => record.meta.requiresGuest);
  const isAuthenticated = AuthStore.getters.isAuthenticated;

  if (requiresAuth && !isAuthenticated) {
    next({ name: 'Login' });
  } else if (requiresGuest && isAuthenticated) {
    next({ name: 'Documents' });
  } else {
    next();
  }
});

export default router;
