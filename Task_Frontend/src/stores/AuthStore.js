import { createStore } from 'vuex';
import authService from "@/services/AuthService.js"; // Make sure the path is correct

export default createStore({
  state() {
    return {
      isAuthenticated: !!localStorage.getItem('accessToken'),
    };
  },
  getters: {
    isAuthenticated(state) {
      return state.isAuthenticated;
    }
  },
  mutations: {
    authenticateUser(state) {
      state.isAuthenticated = true;
    },
    logoutUser(state) {
      state.isAuthenticated = false;
    },
  },
  actions: {
    login({ commit }, tokens) {
      localStorage.setItem('accessToken', tokens.accessToken);
      localStorage.setItem('refreshToken', tokens.refreshToken);
      commit('authenticateUser');
    },
    async logout({ commit }) {
      try {
        await authService.logout(); // Ensure logout logic is processed on the server
      } catch (error) {
        console.error('Logout failed', error);
      }
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      commit('logoutUser');
      window.location.reload();
    },
    tryAutoLogin({ commit }) {
      const accessToken = localStorage.getItem('accessToken');
      if (accessToken) {
        commit('authenticateUser');
      }
    }
  },
});
