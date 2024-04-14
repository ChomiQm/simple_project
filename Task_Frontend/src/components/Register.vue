<script>
import authService from "@/services/AuthService.js";

export default {
  name: "Register",
  data() {
    return {
      email: '',
      password: '',
      confirmPassword: '',
      loading: false,
      error: null,
      passwordRegex: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/
    };
  },
  methods: {
    async handleSubmit() {
      if (!this.passwordRegex.test(this.password)) {
        this.error = 'Password must contain at least 8 characters, one uppercase, one lowercase, one number, and one special character.';
        return;
      }

      if (this.password !== this.confirmPassword) {
        this.error = 'Passwords do not match.';
        this.password = '';
        this.confirmPassword = '';
        return;
      }

      this.loading = true;
      try {
        const response = await authService.register({
          email: this.email,
          password: this.password
        });
        this.loading = false;
        window.alert("Successfully registered");
        this.$router.push('/');
      } catch (error) {
        this.error = 'An error occurred during registration. Please try again.';
        this.loading = false;
      }
    }
  }
};
</script>


<template>
  <div class="register-container">
    <div class="register-form">
      <h1>Register</h1>
      <form @submit.prevent="handleSubmit">
        <input type="email" v-model="email" placeholder="Email" required />
        <input type="password" v-model="password" placeholder="Password" required />
        <input type="password" v-model="confirmPassword" placeholder="Confirm Password" required />
        <button type="submit" :disabled="loading">
          {{ loading ? "Registering..." : "Register" }}
        </button>
      </form>
      <div v-if="error" class="error-message">{{ error }}</div>
    </div>
    <div class="register-info">
      <h1>Document Task App</h1>
      <p>To start using this application, please register or refer to the README.md.</p>
      <div class="requirements">
        <h2>Password requirements:</h2>
        <ul>
          <li>At least 8 characters</li>
          <li>At least one uppercase letter (A-Z)</li>
          <li>At least one lowercase letter (a-z)</li>
          <li>At least one number (0-9)</li>
          <li>At least one special character (!, @)</li>
        </ul>
      </div>
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

.register-container {
  display: flex;
  flex-direction: row;
  justify-content: center;
  gap: 1rem;
  font-size: 0.8rem;
  margin: 1rem;
  animation: fadeIn 2s ease;
}

.register-form,
.register-info {
  flex-basis: 100%;
  max-width: 400px;
  padding: 2rem;
  margin: 0.5rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  border-radius: 10px;
  background: linear-gradient(145deg, #36013F, #3F3F3F);
  display: flex;
  flex-direction: column;
  align-items: center;
}

.register-form {
  color: #EEE;
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

.register-info {
  color: #EEE;
  background: linear-gradient(145deg, #3E0252, #3F3F3F);
  border: none;
}

.requirements {
  text-align: left;
  padding: 0;
}

.requirements h2 {
  margin-bottom: 1rem;
  font-size: 1.2rem;
}

ul {
  list-style: none;
  padding: 0;
}

ul li:before {
  content: '•';
  color: #36013F;
  display: inline-block;
  width: 1em;
  margin-left: -1em;
}

@media (max-width: 768px) {
  .register-container {
    flex-direction: column;
    align-items: center;
  }

  .register-form,
  .register-info {
    flex-basis: auto;
    max-width: 90%;
  }
}
</style>
