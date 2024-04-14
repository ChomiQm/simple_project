<script>
import authService from "@/services/AuthService.js";
import axios from "axios";

export default {
  name: "Login",
  data() {
    return {
      email: '',
      password: '',
      loading: false,
      error: null
    };
  },
  methods: {
    async handleSubmit() {
      this.loading = true;
      try {
        const response = await authService.login({
          email: this.email,
          password: this.password
        });
        this.loading = false;
        localStorage.setItem('accessToken', response.data.accessToken);
        localStorage.setItem('refreshToken', response.data.refreshToken);
        axios.defaults.headers.common['Authorization'] = `Bearer ${response.data.accessToken}`;
        window.alert("Successfully logged in");
        this.$router.push('/');
        window.location.reload();
      } catch (error) {
        this.error = 'An error occurred during login. Please try again.';
        this.loading = false;
      }
    }
  }
};
</script>

<template>
  <div class="login-container">
    <div class="login-form">
      <h1>Login</h1>
      <form @submit.prevent="handleSubmit">
        <input type="email" v-model="email" placeholder="Email" required />
        <input type="password" v-model="password" placeholder="Password" required />
        <button type="submit" :disabled="loading">
          {{ loading ? "Logging in..." : "Login" }}
        </button>
      </form>
      <div v-if="error" class="error-message">{{ error }}</div>
    </div>
  </div>
</template>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 0.9rem;
  animation: fadeIn 2s ease;
}

.login-form {
  max-width: 400px;
  padding: 2rem;
  margin: 1rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  border-radius: 10px;
  background: linear-gradient(145deg, #36013F, #3F3F3F);
  color: #EEE;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.error-message {
  color: #FF6B6B;
  margin-top: 1rem;
  text-align: center;
}

input[type="email"],
input[type="password"] {
  width: 80%;
  padding: 12px;
  margin-bottom: 1rem;
  border: 2px solid #ccc;
  border-radius: 6px;
  background-color: #2D2F36;
  color: #EEE;
  font-size: 1rem;
}

input::placeholder {
  color: #999;
}

button {
  width: 70%;
  padding: 12px;
  background: linear-gradient(145deg, #5CB85C, #151515);
  border: none;
  color: white;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.2s, transform 0.1s;
  font-size: 1rem;
  font-weight: bold;
}

button:disabled {
  background-color: #A8A8A8;
  cursor: not-allowed;
}

button:hover:not(:disabled) {
  background-color: #4CAE4C;
  transform: translateY(-2px);
}

@media (max-width: 768px) {
  .login-container {
    align-items: center;
  }

  .login-form {
    width: 90%;
  }
}
</style>
